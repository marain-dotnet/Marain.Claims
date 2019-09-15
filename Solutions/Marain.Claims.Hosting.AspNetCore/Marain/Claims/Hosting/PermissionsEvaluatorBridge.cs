// <copyright file="PermissionsEvaluatorBridge.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.Claims.Hosting
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Endjin.OpenApi.Claims;
    using Marain.Claims;
    using Marain.Claims.OpenApi;
    using Menes;
    using ClaimPermissionsBatchRequestItem = Marain.Claims.Client.Models.ClaimPermissionsBatchRequestItem;

    /// <summary>
    /// Enables the <see cref="RoleBasedOpenApiAccessControlPolicy"/> to communicate with the
    /// claims service intra-process, so that the claims service can itself use that policy to
    /// secure its own endpoints.
    /// </summary>
    internal class PermissionsEvaluatorBridge : IClaimPermissionsEvaluator
    {
        private readonly ClaimPermissionsService service;

        /// <summary>
        /// Creates <see cref="PermissionsEvaluatorBridge"/>.
        /// </summary>
        /// <param name="service">
        /// The Claims Open API service implementation that provides the ablity to evalute
        /// permissions for a claim.
        /// </param>
        public PermissionsEvaluatorBridge(
            ClaimPermissionsService service)
        {
            this.service = service;
        }

        /// <inheritdoc/>
        public async Task<OpenApiResult> GetClaimPermissionsPermissionAsync(
            string tenantId,
            params ClaimPermissionsBatchRequestItem[] requests)
        {
            var context = new Context
            {
                CurrentTenantId = tenantId,
            };

            return await this.service.GetClaimPermissionsPermissionBatchAsync(context, ToInternalModel(requests)).ConfigureAwait(false);
        }

        private static Marain.Claims.OpenApi.ClaimPermissionsBatchRequestItem[] ToInternalModel(ClaimPermissionsBatchRequestItem[] requests)
        {
            return requests.Select(r => new Marain.Claims.OpenApi.ClaimPermissionsBatchRequestItem(r.ClaimPermissionsId, r.ResourceUri, r.ResourceAccessType)).ToArray();
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
