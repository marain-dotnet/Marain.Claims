// <copyright file="WorkflowClientTestableClaimsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Marain.Claims.Client;
    using Marain.Claims.OpenApi.Specs.Bindings;

    using Microsoft.Rest;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class ClientTestableClaimsService : ITestableClaimsService
    {
        private readonly ClaimsServiceTestTenants testTenants;
        private readonly IClaimsService claimsServiceClient;
        private readonly string clientOid = Guid.NewGuid().ToString();
        private readonly JsonSerializerSettings serializerSettings;

        public ClientTestableClaimsService(
            ClaimsServiceTestTenants testTenants,
            string serviceUrl,
            JsonSerializerSettings serializerSettings)
        {
            this.testTenants = testTenants;
            this.serializerSettings = serializerSettings;

            // When testing the Functions Host locally (and also when doing in-process testing that
            // emulates functions hosting) we can simulate authentication by passing an
            // X-MS-CLIENT-PRINCIPAL header in the same form that Azure Functions would supply.
            // (To test against a deployed instance, we would need to provide a real token, because
            // Azure will block any requests that don't. Also, it won't pass through any
            // X-MS-CLIENT-PRINCIPAL header from the outside.)
            var claimsClient = new UnauthenticatedClaimsService(new Uri(serviceUrl));
            var appServiceClientPrincipal = new JObject(
                new JProperty(
                    "claims",
                    new JArray(
                        new JObject(
                            new JProperty("typ", "http://schemas.microsoft.com/identity/claims/objectidentifier"),
                            new JProperty("val", this.clientOid)))));
            claimsClient.HttpClient.DefaultRequestHeaders.Add(
                "X-MS-CLIENT-PRINCIPAL",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(appServiceClientPrincipal.ToString(Formatting.None))));
            this.claimsServiceClient = claimsClient;
        }

        /// <inheritdoc/>
        public async Task BootstrapTenantClaimsPermissions()
        {
            await this.claimsServiceClient.InitializeTenantAsync(
                this.testTenants.TransientClientTenantId,
                new Client.Models.Body(this.clientOid));
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, ClaimPermissions Result)> GetClaimPermissionsAsync(string claimId)
        {
            HttpOperationResponse<Client.Models.ClaimPermissions> result = await this.claimsServiceClient.GetClaimPermissionsWithHttpMessagesAsync(
                claimId,
                this.testTenants.TransientClientTenantId);

            return await GetStatusAndConvertedBody(result);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, ClaimPermissions Result)> CreateClaimPermissionsAsync(ClaimPermissions newClaimPermissions)
        {
            this.ToClientLibraryType(newClaimPermissions, out Client.Models.CreateClaimPermissionsRequest input);
            HttpOperationResponse<object> result = await this.claimsServiceClient.CreateClaimPermissionsWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                input);

            return await GetStatusAndConvertedBody(result);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, Claims.ResourceAccessRuleSet Result)> CreateResourceAccessRuleSetAsync(
            Claims.ResourceAccessRuleSet ruleSet)
        {
            this.ToClientLibraryType(ruleSet, out Client.Models.ResourceAccessRuleSet input);
            HttpOperationResponse<object> result = await this.claimsServiceClient.CreateResourceAccessRuleSetWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                input);

            return await GetStatusAndConvertedResourceAccessRuleSetBody(result);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> AddRulesForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRule> resourceAccessRules)
        {
            this.ToClientLibraryType(resourceAccessRules, out Client.Models.ResourceAccessRule[] input);
            HttpOperationResponse<Client.Models.ProblemDetails> result = await this.claimsServiceClient.UpdateClaimPermissionsResourceAccessRulesWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                claimId,
                "add",
                input);

            return await GetStatusAndConvertedBody<Client.Models.ProblemDetails, JObject>(result, result.Body);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> RemoveRulesForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRule> resourceAccessRules)
        {
            this.ToClientLibraryType(resourceAccessRules, out Client.Models.ResourceAccessRule[] input);
            HttpOperationResponse<Client.Models.ProblemDetails> result = await this.claimsServiceClient.UpdateClaimPermissionsResourceAccessRulesWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                claimId,
                "remove",
                input);

            return await GetStatusAndConvertedBody<Client.Models.ProblemDetails, JObject>(result, result.Body);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> SetRulesForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRule> resourceAccessRules)
        {
            this.ToClientLibraryType(resourceAccessRules, out Client.Models.ResourceAccessRule[] input);
            HttpOperationResponse<Client.Models.ProblemDetails> result = await this.claimsServiceClient.SetClaimPermissionsResourceAccessRulesWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                claimId,
                input);

            return await GetStatusAndConvertedBody<Client.Models.ProblemDetails, JObject>(result, result.Body);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> AddRuleSetsForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRuleSet> resourceAccessRuleSets)
        {
            this.ToClientLibraryType(resourceAccessRuleSets, out Client.Models.ResourceAccessRuleSetIdOnly[] input);
            HttpOperationResponse<Client.Models.ProblemDetails> result = await this.claimsServiceClient.UpdateClaimPermissionsResourceAccessRuleSetsWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                claimId,
                "add",
                input);

            return await GetStatusAndConvertedBody<Client.Models.ProblemDetails, JObject>(result, result.Body);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> RemoveRuleSetsForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRuleSet> resourceAccessRuleSets)
        {
            this.ToClientLibraryType(resourceAccessRuleSets, out Client.Models.ResourceAccessRuleSetIdOnly[] input);
            HttpOperationResponse<Client.Models.ProblemDetails> result = await this.claimsServiceClient.UpdateClaimPermissionsResourceAccessRuleSetsWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                claimId,
                "remove",
                input);

            return await GetStatusAndConvertedBody<Client.Models.ProblemDetails, JObject>(result, result.Body);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> SetRuleSetsForClaimPermissionsAsync(
            string claimId,
            List<ResourceAccessRuleSet> resourceAccessRuleSets)
        {
            this.ToClientLibraryType(resourceAccessRuleSets, out Client.Models.ResourceAccessRuleSetIdOnly[] input);
            HttpOperationResponse<Client.Models.ProblemDetails> result = await this.claimsServiceClient.SetClaimPermissionsResourceAccessRuleSetsWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                claimId,
                input);

            return await GetStatusAndConvertedBody<Client.Models.ProblemDetails, JObject>(result, result.Body);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, JObject Result)> AddRulesToResourceAccessRuleSetAsync(
            string ruleSetId,
            IList<ResourceAccessRule> newRules)
        {
            this.ToClientLibraryType(newRules, out Client.Models.ResourceAccessRule[] input);
            HttpOperationResponse<Client.Models.ProblemDetails> result = await this.claimsServiceClient.UpdateResourceAccessRuleSetResourceAccessRulesWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                ruleSetId,
                "add",
                input);

            return await GetStatusAndConvertedBody<Client.Models.ProblemDetails, JObject>(result, result.Body);
        }

        private static Task<(int HttpStatusCode, ClaimPermissions Result)> GetStatusAndConvertedBody(
            HttpOperationResponse<object> result)
            => GetStatusAndConvertedBody<Client.Models.ClaimPermissions, ClaimPermissions>(result, result.Body as Client.Models.ClaimPermissions);

        private static Task<(int HttpStatusCode, ClaimPermissions Result)> GetStatusAndConvertedBody(
            HttpOperationResponse<Client.Models.ClaimPermissions> result)
            => GetStatusAndConvertedBody<Client.Models.ClaimPermissions, ClaimPermissions>(result, result.Body);

        private static Task<(int HttpStatusCode, ResourceAccessRuleSet Result)> GetStatusAndConvertedResourceAccessRuleSetBody(
            HttpOperationResponse<object> result)
            => GetStatusAndConvertedBody<Client.Models.ResourceAccessRuleSet, ResourceAccessRuleSet>(
                    result, (Client.Models.ResourceAccessRuleSet)result.Body);

        private static async Task<(int HttpStatusCode, TResult Result)> GetStatusAndConvertedBody<TInput, TResult>(
            HttpOperationResponse response, TInput clientTypeResult)
        {
            // We get the result back in some  type defined in the client library, but we want
            // to convert that to the internal type for test purposes (because for in-process direct testing,
            // we don't involve the client library, so ITestableClaimsService is all in terms of internal types).
            string resultBody = await response.Response.Content.ReadAsStringAsync();
            TResult internalTypeResult = JsonConvert.DeserializeObject<TResult>(resultBody);
            return ((int)response.Response.StatusCode, internalTypeResult);
        }

        private void ToClientLibraryType<TInternal, TClient>(TInternal input, out TClient output)
        {
            output = JsonConvert.DeserializeObject<TClient>(JsonConvert.SerializeObject(input, this.serializerSettings));
        }
    }
}
