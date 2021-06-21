// <copyright file="WorkflowClientTestableClaimsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Marain.Claims.Client;
    using Marain.Claims.OpenApi.Specs.Bindings;

    using Microsoft.OpenApi;
    using Microsoft.Rest;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class ClientTestableClaimsService : ITestableClaimsService
    {
        private readonly ClaimsServiceTestTenants testTenants;
        private readonly IClaimsService claimsServiceClient;
        private readonly string clientOid = Guid.NewGuid().ToString();

        public ClientTestableClaimsService(
            ClaimsServiceTestTenants testTenants,
            string serviceUrl)
        {
            this.testTenants = testTenants;

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
        public async Task<(int HttpStatusCode, ClaimPermissions Result)> GetClaimIdAsync(string claimId)
        {
            HttpOperationResponse<Client.Models.ClaimPermissions> result = await this.claimsServiceClient.GetClaimPermissionsWithHttpMessagesAsync(
                claimId,
                this.testTenants.TransientClientTenantId);

            return await GetStatusAndConvertedBody(result);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, ClaimPermissions Result)> CreateClaimAsync(ClaimPermissions newClaimPermissions)
        {
            ToClientLibraryType(newClaimPermissions, out Client.Models.ClaimPermissions input);
            HttpOperationResponse<Client.Models.ClaimPermissions> result = await this.claimsServiceClient.CreateClaimPermissionsWithHttpMessagesAsync(
                this.testTenants.TransientClientTenantId,
                input);

            return await GetStatusAndConvertedBody(result);
        }

        private static async Task<(int HttpStatusCode, ClaimPermissions Result)> GetStatusAndConvertedBody(HttpOperationResponse<Client.Models.ClaimPermissions> result)
        {
            // We get the result back in some  type defined in the client library, but we want
            // to convert that to the internal type for test purposes (because for in-process direct testing,
            // we don't involve the client library, so ITestableClaimsService is all in terms of internal types).
            string resultBody = await result.Response.Content.ReadAsStringAsync();
            ClaimPermissions claimPermissions = JsonConvert.DeserializeObject<ClaimPermissions>(resultBody);
            return ((int)result.Response.StatusCode, claimPermissions);
        }

        private static void ToClientLibraryType<TInternal, TClient>(TInternal input, out TClient output)
        {
            output = JsonConvert.DeserializeObject<TClient>(JsonConvert.SerializeObject(input));
        }
    }
}
