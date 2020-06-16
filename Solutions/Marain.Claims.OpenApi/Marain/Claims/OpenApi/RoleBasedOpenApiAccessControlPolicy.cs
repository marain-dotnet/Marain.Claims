// <copyright file="RoleBasedOpenApiAccessControlPolicy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Menes;
    using Menes.Exceptions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// An OpenApi access control policy that implements application-role based security on top of
    /// the <c>Endjin.Claims</c> access rule system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This maps application role claims in the <see cref="ClaimsPrincipal"/> to claim permissions
    /// in <c>Endjin.Claims</c>. It maps the incoming request path to the 'resource' identifier,
    /// and the HTTP verb to the 'access type'. The behaviour when the claim contains multiple roles
    /// that produce conflicting answers can be configured - this policy can operate either in a mode
    /// where all the roles must grant permission, or where it is enough for one to do so.
    /// </para>
    /// <para>
    /// For example, given an POST to http://example.com/foo/bar/1234 this will ask the Claims
    /// service to evaluate permissions using the application role as the <c>claimPermissionsId</c>,
    /// <c>foo/bar/1234</c> as the <c>resourceUri</c>, and <c>POST</c> as the <c>accessType</c>.
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
    public class RoleBasedOpenApiAccessControlPolicy : IOpenApiAccessControlPolicy
    {
        private readonly string resourcePrefix;
        private readonly bool allowOnlyIfAll;
        private readonly IResourceAccessEvaluator resourceAccessEvaluator;
        private readonly ILogger<RoleBasedOpenApiAccessControlPolicy> logger;

        /// <summary>
        /// Create a <see cref="RoleBasedOpenApiAccessControlPolicy"/>.
        /// </summary>
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
        /// Configures the behaviour when multiple <c>roles</c> claims are present, and the Claims
        /// service reports different permissions for the different roles. If false, permission
        /// will be granted as long as at least one role grants access. If true, all roles must
        /// grant access (and at least one <c>roles</c> claim must be present in either case).
        /// </param>
        /// <remarks>
        /// <para>
        /// The mode where the policy grants access if any one roles' claims permissions grants
        /// access is the one that produces the least surprising 'group membership' behaviour.
        /// E.g., if the claims include "reader" and "admin" roles, and the principal attempts an
        /// operation which is denied to "reader" members but granted to any "admin", the
        /// membership of the role granting access - "admin" in this case - trumps membership of
        /// any roles that do not.) This is consistent with how OS group membership generally
        /// works - administrators get to use their administrator privileges, and if they are also
        /// members of a more lowly 'users' group, they would not expect that to strip them of
        /// their privileges.
        /// </para>
        /// </remarks>
        internal RoleBasedOpenApiAccessControlPolicy(
            IResourceAccessEvaluator resourceAccessEvaluator,
            ILogger<RoleBasedOpenApiAccessControlPolicy> logger,
            string resourcePrefix,
            bool allowOnlyIfAll)
        {
            this.resourcePrefix = resourcePrefix;
            this.allowOnlyIfAll = allowOnlyIfAll;
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
            if (context.CurrentPrincipal.Identity?.IsAuthenticated != true)
            {
                return requests.ToDictionary(x => x, _ => new AccessControlPolicyResult(AccessControlPolicyResultType.NotAuthenticated));
            }

            // Get the list of roles for the user.
            IList<string> roles = context.CurrentPrincipal
                .Claims
                .Where(c => c.Type == "roles")
                .Select(c => c.Value)
                .ToList();

            if (roles.Count == 0)
            {
                // User isn't in any roles, no need to continue. Return NotAllowed for each request.
                return requests.ToDictionary(x => x, _ => new AccessControlPolicyResult(AccessControlPolicyResultType.NotAllowed));
            }

            // We don't evaluate claims for the paths that are supplied in the parameters; we do it for the combination of
            // resource prefix and path, which we call the Resource Uri. In order to simplify this, we create a mapping of
            // requested path to the resource Uri
            var pathToResourceUriMap = requests.Select(x => x.Path).Distinct().ToDictionary(x => x, x => this.resourcePrefix.Replace("{tenantId}", context.CurrentTenantId) + x.TrimStart('/'));

            // Now translate the set of requested evaluations into a set of requests for the claims service. This is built
            // from the cartesian product of the roles and requests lists.
            var submissions = roles.SelectMany(role => requests.Select(request => new ResourceAccessSubmission
            {
                ClaimPermissionsId = role,
                ResourceAccessType = request.Method,
                ResourceUri = pathToResourceUriMap[request.Path],
            })).ToList();

            List<ResourceAccessEvaluation> evaluations;

            try
            {
                evaluations = await this.resourceAccessEvaluator.EvaluateAsync(context.CurrentTenantId, submissions);
            }
            catch (Exception ex)
            {
                throw new OpenApiAccessControlPolicyEvaluationFailedException(
                   nameof(RoleBasedOpenApiAccessControlPolicy),
                   requests,
                   $"Permission evaluation failed.",
                   ex).AddProblemDetailsExtension("Roles", string.Join(",", roles));
            }

            // For each of the operation descriptors supplied in the requests parameters, the response body will
            // contain one result per user role. We now need to aggregate these into a single response for each
            // request.
            return requests.ToDictionary(request => request, request =>
            {
                // Find the subset of responses that match this particular request.
                IList<ResourceAccessEvaluation> evaluatedPermissions = evaluations.Where(x => x.Submission.ResourceUri == pathToResourceUriMap[request.Path] && x.Submission.ResourceAccessType == request.Method).ToList();

                bool noRequestsFailed = evaluatedPermissions.Count == roles.Count;

                // Aggregate the responses based on the rule set for this.
                bool allow = this.allowOnlyIfAll
                    ? evaluatedPermissions.AllAndAtLeastOne(p => p.Result.Permission == Permission.Allow) && noRequestsFailed
                    : evaluatedPermissions.Any(p => p.Result.Permission == Permission.Allow);

                // If denying permission, log this out.
                if (!allow)
                {
                    string roleNames = string.Join(",", roles);
                    this.logger.LogWarning(
                        nameof(RoleBasedOpenApiAccessControlPolicy) + " blocking access for [{httpMethod}] [{path}] (OpenApi operation [{operationId}]) for principal in roles [{roleNames}]",
                        request.Method,
                        request.Path,
                        request.OperationId,
                        roleNames);
                }

                return new AccessControlPolicyResult(allow
                    ? AccessControlPolicyResultType.Allowed
                    : AccessControlPolicyResultType.NotAllowed);
            });
        }
    }
}
