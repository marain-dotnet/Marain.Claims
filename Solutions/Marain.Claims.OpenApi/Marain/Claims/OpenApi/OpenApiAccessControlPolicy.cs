// <copyright file="OpenApiAccessControlPolicy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Marain.Claims;
    using Marain.Claims.OpenApi.Internal;
    using Menes;
    using Menes.Exceptions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// An OpenApi access control policy that implements principal-based security on top of
    /// the <c>Endjin.Claims</c> access rule system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The behaviour when the evaluator produces conflicting answers can be configured - this policy can operate either
    /// in a mode where all the evaluations must grant permission, or where it is enough for one to do so.
    /// </para>
    /// <para>
    /// Note that we convert the request URL to a relative URL with no leading <c>/</c>, partly to
    /// avoid needing to change the names of the permissions depending on the service hostname, but
    /// also because until PBI #3364 is resolved, the Claims service cannot cope with resource names
    /// that start with a leading <c>/</c> or which are fully qualified URLs.
    /// </para>
    /// <para>
    /// To enable multiple services to share the same Claims service without interference, an
    /// optional resource prefix may be specified. So in the example above, if the resource prefix
    /// were <c>EndjinFrobnicatorService/</c> the <c>resourceUri</c> would be
    /// <c>EndjinFrobnicatorService/foo/bar/1234</c>.
    /// </para>
    /// </remarks>
    public class OpenApiAccessControlPolicy : IOpenApiAccessControlPolicy
    {
        private readonly string resourcePrefix;
        private readonly bool allowOnlyIfAll;
        private readonly IResourceAccessSubmissionBuilder resourceAccessSubmissionBuilder;
        private readonly IResourceAccessEvaluator resourceAccessEvaluator;
        private readonly ILogger<OpenApiAccessControlPolicy> logger;

        /// <summary>
        /// Create a <see cref="OpenApiAccessControlPolicy"/>.
        /// </summary>
        /// <param name="resourceAccessSubmissionBuilder">
        /// Builds the resource access submissions for evaluation. This will normally be either
        /// a role-based builder or an identity-based builder.
        /// </param>
        /// <param name="resourceAccessEvaluator">
        /// Evaluates the resource access requests. This will normally be a wrapper around the claims service
        /// client, but in the special case where we are running inside the claims service, this
        /// will use the OpenAPI service implementation directly.
        /// </param>
        /// <param name="logger">Diagnostic logger.</param>
        /// <param name="resourcePrefix">
        /// A prefix to add to the path when forming the resource URI.
        /// </param>
        /// <param name="allowOnlyIfAll">
        /// Configures the behaviour when multiple submissions are evaluated for a request, and the evaluator
        /// service reports different permissions for the different submissions. If false, permission
        /// will be granted as long as at least one submission evaluation grants access. If true, all evaluations must
        /// grant access (and at least one evaluation must be present in either case).
        /// </param>
        /// <remarks>
        /// <para>
        /// The mode where the policy grants access if any one evaluation grants
        /// access is the one that produces the least surprising 'group membership' behaviour.
        /// E.g., when using a role-based submission builder, if the claims include "reader"
        /// and "admin" roles, and the principal attempts an operation which is denied to "reader"
        /// members but granted to any "admin", the membership of the role granting access - "admin"
        /// in this case - trumps membership of any roles that do not.) This is consistent with how
        /// OS group membership generally works - administrators get to use their administrator privileges,
        /// and if they are also members of a more lowly 'users' group, they would not expect that to strip them of
        /// their privileges.
        /// </para>
        /// </remarks>
        internal OpenApiAccessControlPolicy(
            IResourceAccessSubmissionBuilder resourceAccessSubmissionBuilder,
            IResourceAccessEvaluator resourceAccessEvaluator,
            ILogger<OpenApiAccessControlPolicy> logger,
            string resourcePrefix,
            bool allowOnlyIfAll)
        {
            this.resourcePrefix = resourcePrefix;
            this.allowOnlyIfAll = allowOnlyIfAll;
            this.resourceAccessSubmissionBuilder = resourceAccessSubmissionBuilder;
            this.resourceAccessEvaluator = resourceAccessEvaluator;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<IDictionary<AccessCheckOperationDescriptor, AccessControlPolicyResult>> ShouldAllowAsync(
            IOpenApiContext context,
            params AccessCheckOperationDescriptor[] requests)
        {
            // If current principal isn't authenticated, then build and return a dictionary containing a
            // NotAuthenticated response for each operation descriptor.
            if (context.CurrentPrincipal?.Identity?.IsAuthenticated != true)
            {
                return requests.ToDictionary(x => x, _ => new AccessControlPolicyResult(AccessControlPolicyResultType.NotAuthenticated));
            }

            // We don't evaluate claims for the paths that are supplied in the parameters; we do it for the combination of
            // resource prefix and path, which we call the Resource Uri. In order to simplify this, we create a mapping of
            // requested path to the resource Uri
            var pathToResourceUriMap = requests.Select(x => x.Path).Distinct().ToDictionary(x => x, x => (this.resourcePrefix?.Replace("{tenantId}", context.CurrentTenantId) ?? string.Empty) + x.TrimStart('/'));

            List<ResourceAccessSubmission> submissions = this.resourceAccessSubmissionBuilder.BuildResourceAccessSubmissions(context, requests, pathToResourceUriMap);

            if (submissions.Count == 0)
            {
                // No submissions to evaluate, no need to continue. Return NotAllowed for each request.
                return requests.ToDictionary(x => x, _ => new AccessControlPolicyResult(AccessControlPolicyResultType.NotAllowed));
            }

            var claimPermissionsIds = submissions.Select(s => s.ClaimPermissionsId).Distinct().ToList();

            List<ResourceAccessEvaluation> evaluations;

            try
            {
                evaluations = await this.resourceAccessEvaluator.EvaluateAsync(context.CurrentTenantId, submissions);
            }
            catch (Exception ex)
            {
                throw new OpenApiAccessControlPolicyEvaluationFailedException(
                   nameof(OpenApiAccessControlPolicy),
                   requests,
                   "Permission evaluation failed.",
                   ex).AddProblemDetailsExtension("ClaimPermissions", string.Join(",", claimPermissionsIds));
            }

            // For each of the operation descriptors supplied in the requests parameters, the response body will
            // contain one result per user role. We now need to aggregate these into a single response for each
            // request.
            return requests.ToDictionary(request => request, request =>
            {
                // Find the subset of responses that match this particular request.
                IList<ResourceAccessEvaluation> evaluatedPermissions = evaluations.Where(x => x.Submission.ResourceUri == pathToResourceUriMap[request.Path] && x.Submission.ResourceAccessType == request.Method).ToList();

                bool noRequestsFailed = evaluatedPermissions.Count == claimPermissionsIds.Count;

                // Aggregate the responses based on the rule set for this.
                bool allow = this.allowOnlyIfAll
                    ? evaluatedPermissions.AllAndAtLeastOne(p => p.Result.Permission == Permission.Allow) && noRequestsFailed
                    : evaluatedPermissions.Any(p => p.Result.Permission == Permission.Allow);

                // If denying permission, log this out.
                if (!allow)
                {
                    string claimPermissionsNames = string.Join(",", claimPermissionsIds);
                    this.logger.LogWarning(
                        nameof(OpenApiAccessControlPolicy) + " blocking access for [{httpMethod}] [{path}] (OpenApi operation [{operationId}]) for principal in roles [{claimPermissionsNames}]",
                        request.Method,
                        request.Path,
                        request.OperationId,
                        claimPermissionsNames);
                }

                return new AccessControlPolicyResult(allow
                    ? AccessControlPolicyResultType.Allowed
                    : AccessControlPolicyResultType.NotAllowed);
            });
        }
    }
}