namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using System;
    using System.Threading.Tasks;

    using Marain.Claims;
    using Marain.Claims.OpenApi.Specs.Bindings;

    using Menes;

    using Newtonsoft.Json.Linq;

    internal class DirectTestableClaimsService : ITestableClaimsService
    {
        private readonly ClaimsServiceTestTenants testTenants;
        private readonly ClaimPermissionsService service;
        private readonly string clientOid = Guid.NewGuid().ToString();

        public DirectTestableClaimsService(
            ClaimsServiceTestTenants testTenants,
            ClaimPermissionsService service)
        {
            this.testTenants = testTenants;
            this.service = service;
        }

        public async Task BootstrapTenantClaimsPermissions()
        {
            await this.service.BootstrapTenantAsync(
                this.MakeOpenApiContext(),
                new JObject(new JProperty("administratorPrincipalObjectId", this.clientOid)));
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, ClaimPermissions Result)> GetClaimIdAsync(string claimPermissionsId)
        {
            OpenApiResult result = await this.service.GetClaimPermissionAsync(this.MakeOpenApiContext(), claimPermissionsId);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as ClaimPermissions);
        }

        /// <inheritdoc/>
        public async Task<(int HttpStatusCode, ClaimPermissions Result)> CreateClaimAsync(ClaimPermissions newClaimPermissions)
        {
            OpenApiResult result = await this.service.CreateClaimPermissionsAsync(this.MakeOpenApiContext(), newClaimPermissions);
            result.Results.TryGetValue("application/json", out object claimPermissions);
            return (result.StatusCode, claimPermissions as ClaimPermissions);
        }

        private SimpleOpenApiContext MakeOpenApiContext()
        {
            return new SimpleOpenApiContext { CurrentTenantId = this.testTenants.TransientClientTenantId };
        }
    }
}
