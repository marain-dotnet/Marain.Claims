// <copyright file="ITestableClaimsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Enables a single set of tests to work against a service hosted directly without Menes,
    /// in-process via Menes, or out of process.
    /// </summary>
    public interface ITestableClaimsService
    {
        Task BootstrapTenantClaimsPermissions();

        Task<(int HttpStatusCode, ClaimPermissions Result)> GetClaimPermissionsAsync(string claimPermissionsId);

        Task<(int HttpStatusCode, ClaimPermissions Result)> CreateClaimPermissionsAsync(ClaimPermissions newClaimPermissions);

        Task<(int HttpStatusCode, ResourceAccessRuleSet Result)> CreateResourceAccessRuleSetAsync(ResourceAccessRuleSet newClaimPermissions);

        Task<(int HttpStatusCode, JObject Result)> AddRulesForClaimPermissionsAsync(string claimId, List<ResourceAccessRule> resourceAccessRules);
    }
}
