// <copyright file="DirectTestableClaimsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> RemoveRulesForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRule> resourceAccessRules)
        {
            OpenApiResult result = await this.claimsService.UpdateClaimPermissionsResourceAccessRulesAsync(
                this.MakeOpenApiContext(),
                claimId,
                UpdateOperation.Remove,
                resourceAccessRules);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as JObject);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> SetRulesForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRule> resourceAccessRules)
        {
            OpenApiResult result = await this.claimsService.SetClaimPermissionsResourceAccessRulesAsync(
                this.MakeOpenApiContext(),
                claimId,
                resourceAccessRules);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as JObject);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> AddRuleSetsForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRuleSet> resourceAccessRuleSets)
        {
            OpenApiResult result = await this.claimsService.UpdateClaimPermissionsResourceAccessRuleSetsAsync(
                this.MakeOpenApiContext(),
                claimId,
                UpdateOperation.Add,
                resourceAccessRuleSets);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as JObject);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> RemoveRuleSetsForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRuleSet> resourceAccessRuleSets)
        {
            OpenApiResult result = await this.claimsService.UpdateClaimPermissionsResourceAccessRuleSetsAsync(
                this.MakeOpenApiContext(),
                claimId,
                UpdateOperation.Remove,
                resourceAccessRuleSets);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as JObject);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> SetRuleSetsForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRuleSet> resourceAccessRuleSets)
        {
            OpenApiResult result = await this.claimsService.SetClaimPermissionsResourceAccessRuleSetsAsync(
                this.MakeOpenApiContext(),
                claimId,
                resourceAccessRuleSets);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as JObject);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> AddRulesToResourceAccessRuleSetAsync(
            string ruleSetId,
            IList<ResourceAccessRule> newRules)
        {
            OpenApiResult result = await this.ruleSetService.UpdateResourceAccessRuleSetResourceAccessRulesAsync(
                this.testTenants.TransientClientTenantId,
                ruleSetId,
                UpdateOperation.Add,
                newRules);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as JObject);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, IList<ResourceAccessRule> Result)> GetEffectiveRulesForClaimPermissionsAsync(
            string claimPermissionsId)
        {
            OpenApiResult result = await this.claimsService.GetClaimPermissionResourceAccessRulesAsync(
                this.MakeOpenApiContext(),
                claimPermissionsId);
            result.Results.TryGetValue("application/json", out object rules);
            return (result.StatusCode, rules as IList<ResourceAccessRule>);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, PermissionResult Result)> EvaluateSinglePermissionForClaimPermissionsAsync(
            string claimPermissionsId, string resourceUri, string accessType)
        {
            OpenApiResult result = await this.claimsService.GetClaimPermissionsPermissionAsync(
                this.MakeOpenApiContext(),
                claimPermissionsId,
                resourceUri,
                accessType);
            result.Results.TryGetValue("application/json", out object rules);
            return (result.StatusCode, rules as PermissionResult);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, IList<ClaimPermissionsBatchResponseItem> Result)> BatchEvaluatePermissionsForClaimPermissionsAsync(
            IEnumerable<ClaimPermissionsBatchRequestItem> items)
        {
            OpenApiResult result = await this.claimsService.GetClaimPermissionsPermissionBatchAsync(
                this.MakeOpenApiContext(),
                items.ToArray());
            result.Results.TryGetValue("application/json", out object rules);
            return (result.StatusCode, rules as IList<ClaimPermissionsBatchResponseItem>);
        }

        private SimpleOpenApiContext MakeOpenApiContext()
        {
            return new SimpleOpenApiContext { CurrentTenantId = this.testTenants.TransientClientTenantId };
        }
    }
}