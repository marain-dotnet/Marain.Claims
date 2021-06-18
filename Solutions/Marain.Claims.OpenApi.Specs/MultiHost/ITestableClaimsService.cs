// <copyright file="ITestableClaimsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using System.Threading.Tasks;

    /// <summary>
    /// Enables a single set of tests to work against a service hosted directly without Menes,
    /// in-process via Menes, or out of process.
    /// </summary>
    public interface ITestableClaimsService
    {
        Task BootstrapTenantClaimsPermissions();

        Task<(int HttpStatusCode, ClaimPermissions Result)> GetClaimIdAsync(string claimPermissionsId);

        Task<(int HttpStatusCode, ClaimPermissions Result)> CreateClaimAsync(ClaimPermissions newClaimPermissions);
    }
}
