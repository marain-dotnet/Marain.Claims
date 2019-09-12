// <copyright file="IClaimPermissionsEvaluator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System.Threading.Tasks;
    using Marain.Claims.Client.Models;
    using Menes;

    /// <summary>
    /// Interface enabling code to evaluate claims without necessarily needing to know exactly what
    /// class is doing it.
    /// </summary>
    /// <remarks>
    /// This is for the benefit of the intra-process claims evaluation that we need in order for
    /// the claims service to protect itself using the role-based security policy which in turn
    /// relies on the claims service. (And the interface is mainly to enable testing.)
    /// </remarks>
    public interface IClaimPermissionsEvaluator
    {
        /// <summary>
        /// Handles a request to get permission results for a batch of claim permissions.
        /// </summary>
        /// <param name="tenantId">The id of the tenant.</param>
        /// <param name="requests">The list of request items to check.</param>
        /// <returns>
        /// A task that produces the <c>PermissionResult</c> wrapped as an <see cref="OpenApiResult"/>.
        /// </returns>
        Task<OpenApiResult> GetClaimPermissionsPermissionAsync(
            string tenantId,
            params ClaimPermissionsBatchRequestItem[] requests);
    }
}
