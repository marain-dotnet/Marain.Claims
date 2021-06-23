namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Marain.Claims;
    using Marain.Claims.OpenApi.Specs.Bindings;

    using Menes;

    using Newtonsoft.Json.Linq;

    internal class DirectTestableClaimsService : ITestableClaimsService
    {
        private readonly ClaimsServiceTestTenants testTenants;
        private readonly ClaimPermissionsService claimsService;
        private readonly ResourceAccessRuleSetService ruleSetService;
        private readonly string clientOid = Guid.NewGuid().ToString();

        public DirectTestableClaimsService(
            ClaimsServiceTestTenants testTenants,
            ClaimPermissionsService claimsService,
            ResourceAccessRuleSetService ruleSetService)
        {
            this.testTenants = testTenants;
            this.claimsService = claimsService;
            this.ruleSetService = ruleSetService;
        }

        public async Task BootstrapTenantClaimsPermissions()
        {
            await this.claimsService.BootstrapTenantAsync(
                this.MakeOpenApiContext(),
                new JObject(new JProperty("administratorPrincipalObjectId", this.clientOid)));
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, ClaimPermissions Result)> GetClaimPermissionsAsync(string claimPermissionsId)
        {
            OpenApiResult result = await this.claimsService.GetClaimPermissionAsync(this.MakeOpenApiContext(), claimPermissionsId);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as ClaimPermissions);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, ClaimPermissions Result)> CreateClaimPermissionsAsync(ClaimPermissions newClaimPermissions)
        {
            OpenApiResult result = await this.claimsService.CreateClaimPermissionsAsync(this.MakeOpenApiContext(), newClaimPermissions);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as ClaimPermissions);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, ResourceAccessRuleSet Result)> CreateResourceAccessRuleSetAsync(ResourceAccessRuleSet newClaimPermissions)
        {
            OpenApiResult result = await this.ruleSetService.CreateResourceAccessRuleSetAsync(
                this.testTenants.TransientClientTenantId, newClaimPermissions);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as ResourceAccessRuleSet);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> AddRulesForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRule> resourceAccessRules)
        {
            OpenApiResult result = await this.claimsService.UpdateClaimPermissionsResourceAccessRulesAsync(
                this.MakeOpenApiContext(),
                claimId,
                UpdateOperation.Add,
                resourceAccessRules);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as JObject);
        }

        private SimpleOpenApiContext MakeOpenApiContext()
        {
            return new SimpleOpenApiContext { CurrentTenantId = this.testTenants.TransientClientTenantId };
        }
    }
}
