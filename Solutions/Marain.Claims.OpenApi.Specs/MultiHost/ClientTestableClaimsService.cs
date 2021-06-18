// <copyright file="WorkflowClientTestableClaimsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Marain.Claims.Client;
    using Marain.Claims.OpenApi.Specs.Bindings;

    using Microsoft.Rest;

    using Newtonsoft.Json;

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
            var claimsClient = new UnauthenticatedClaimsService(new Uri(serviceUrl));
            ////claimsClient.HttpClient.DefaultRequestHeaders.Add("X-MARAIN-CLAIMS", $"{{ \"oid\": [ \"{this.clientOid}\" ] }}");

            var jwt = new JwtSecurityToken(claims: new[] { new Claim("oid", this.clientOid) });
            ////string token = $"{jwt.EncodedHeader}.{jwt.EncodedPayload}.{jwt.En}";
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);
            claimsClient.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", token);
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
