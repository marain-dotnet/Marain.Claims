// <copyright file="LocalResourceAccessEvaluator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Hosting.AspNetCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Marain.Claims.OpenApi;
    using Menes;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Evaluates resource access submissions using the <see cref="ClaimPermissionsService"/>.
    /// </summary>
    public class LocalResourceAccessEvaluator : IResourceAccessEvaluator
    {
        private readonly ILogger<LocalResourceAccessEvaluator> logger;
        private readonly ClaimPermissionsService service;

        /// <summary>
        /// Creates <see cref="LocalResourceAccessEvaluator"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="service">
        /// The Claims Open API service implementation that provides the ablity to evaluate
        /// resource access submissions.
        /// </param>
        public LocalResourceAccessEvaluator(
            ILogger<LocalResourceAccessEvaluator> logger,
            ClaimPermissionsService service)
        {
            this.logger = logger;
            this.service = service;
        }

        /// <inheritdoc/>
        public async Task<List<ResourceAccessEvaluation>> EvaluateAsync(string tenantId, IEnumerable<ResourceAccessSubmission> submissions)
        {
            var context = new Context
            {
                CurrentTenantId = tenantId,
            };

            OpenApiResult result = await this.service.GetClaimPermissionsPermissionBatchAsync(context, ToInternalModel(submissions)).ConfigureAwait(false);

            var results = (IList<ClaimPermissionsBatchResponseItem>)result.Results["application/json"];

            // If any of the requests didn't return an OK result, we need to log a warning, as this is most likely due to
            // misconfiguration of the claims service (e.g. a missing ClaimPermissionsId). The caller may still be able to carry
            // on and evaluate the remaining results.
            foreach (ClaimPermissionsBatchResponseItem r in results.Where(x => x.ResponseCode != (int)HttpStatusCode.OK))
            {
                this.logger.LogWarning(
                    "Claims service returned [{statusCode}] permission evaluation for claim permission ID [{claimPermissionID}] accessing resource [{resourceUri}], [{httpMethod}]",
                    r.ResponseCode,
                    r.ClaimPermissionsId,
                    r.ResourceUri,
                    r.ResourceAccessType);
            }

            return results
                .Where(x => x.ResponseCode == (int)HttpStatusCode.OK)
                .Select(x => new ResourceAccessEvaluation
                {
                    Result = new PermissionResult
                    {
                        Permission = Enum.TryParse(x.Permission, true, out Permission permission) ? permission : throw new FormatException(),
                    },
                    Submission = new ResourceAccessSubmission
                    {
                        ClaimPermissionsId = x.ClaimPermissionsId,
                        ResourceAccessType = x.ResourceAccessType,
                        ResourceUri = x.ResourceUri,
                    },
                }).ToList();
        }

        private static ClaimPermissionsBatchRequestItem[] ToInternalModel(IEnumerable<ResourceAccessSubmission> submissions)
        {
            return submissions.Select(submission => new ClaimPermissionsBatchRequestItem(submission.ClaimPermissionsId, submission.ResourceUri, submission.ResourceAccessType)).ToArray();
        }

        /// <summary>
        /// Fake context for passing tenant id to service.
        /// </summary>
        /// <remarks>
        /// Access control policies don't work directly with the context - they are passed the
        /// claims and tenant id directly. But since we're calling directly into the Open API
        /// service to evalute claims, it will expect to be able to extract the tenant id
        /// from the context, so we need to re-wrap the tenant in a context.
        /// </remarks>
        private class Context : IOpenApiContext
        {
            /// <inheritdoc/>
            /// <remarks>
            /// This context is passed only to the claims evaluation endpoint which should not be
            /// looking at the claims principal. (For one thing, the Claims service relies on
            /// role-based access control policy to implement its security, so the service
            /// implementation itself has no need to inspect the principal. Second, the permission
            /// evaluation endpoint is open to all authenticated clients anyway.) So we throw an
            /// exception to ensure that should that ever change for any reason, we'll notice that
            /// there's a problem.
            /// </remarks>
            public ClaimsPrincipal CurrentPrincipal { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            /// <inheritdoc/>
            public string CurrentTenantId { get; set; }

            public dynamic AdditionalContext { get; set; }
        }
    }
}