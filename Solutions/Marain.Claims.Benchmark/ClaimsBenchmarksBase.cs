using System.Threading.Tasks;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Corvus.Json;
using Corvus.Storage.Azure.BlobStorage.Tenancy;
using Corvus.Tenancy;

using Marain.Claims.Client;
using Marain.Tenancy.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Marain.Claims.Benchmark
{
    public abstract class ClaimsBenchmarksBase
    {
        protected readonly IClaimsService ClaimsService;
        protected readonly ITenancyService TenancyService;
        protected readonly IBlobContainerSourceWithTenantLegacyTransition TenantBlobContainerClientFactory;
        protected readonly IPropertyBagFactory PropertyBagFactory;
        protected readonly string ClientTenantId;
        protected readonly string AdministratorPrincipalObjectId;

        public ClaimsBenchmarksBase()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                      .AddEnvironmentVariables()
                      .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = configurationBuilder.Build();

            this.ClientTenantId = configuration["ClientTenantId"];
            this.AdministratorPrincipalObjectId = configuration["AdministratorPrincipalObjectId"];

            ServiceProvider serviceProvider = new ServiceCollection()
                .AddClaimsClient(sp => configuration.GetSection("ClaimsClient").Get<ClaimsClientOptions>())
                .AddSingleton(sp => configuration.GetSection("TenancyClient").Get<TenancyClientOptions>())
                .AddTenancyClient(enableResponseCaching: true)
                .AddJsonPropertyBagFactory()
                .AddBlobContainerV2ToV3Transition()
                .AddAzureBlobStorageClientSourceFromDynamicConfiguration()
                .AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(configuration["AzureServicesAuthConnectionString"])
                .AddMicrosoftRestAdapterForServiceIdentityAccessTokenSource()
                .BuildServiceProvider();

            this.ClaimsService = serviceProvider.GetRequiredService<IClaimsService>();
            this.TenancyService = serviceProvider.GetRequiredService<ITenancyService>();
            this.TenantBlobContainerClientFactory = serviceProvider.GetRequiredService<IBlobContainerSourceWithTenantLegacyTransition>();
            this.PropertyBagFactory = serviceProvider.GetRequiredService<IPropertyBagFactory>();
        }

        protected abstract Task SetupTestDataAsync();

        protected async Task DeleteTestDataAsync()
        {
            var response = (JObject)await this.TenancyService.GetTenantAsync(this.ClientTenantId);

            Tenancy.Client.Models.Tenant clientTenant = JsonConvert.DeserializeObject<Tenancy.Client.Models.Tenant>(response.ToString());

            var tenant = new Tenant(clientTenant.Id, clientTenant.Name, this.PropertyBagFactory.Create(clientTenant.Properties));

            BlobContainerClient claimPermissionsContainer =
                await this.TenantBlobContainerClientFactory.GetBlobContainerClientFromTenantAsync(
                    tenant,
                    "StorageConfiguration__claimpermissions",
                    "StorageConfigurationV3__claimpermissions");
            BlobContainerClient resourceAccessRuleSetsContainer =
                await this.TenantBlobContainerClientFactory.GetBlobContainerClientFromTenantAsync(
                    tenant,
                    "StorageConfiguration__resourceaccessrulesets",
                    "StorageConfigurationV3__resourceaccessrulesets");

            foreach (BlobItem blob in claimPermissionsContainer.GetBlobs())
            {
                await claimPermissionsContainer.DeleteBlobAsync(blob.Name);
            }

            foreach (BlobItem blob in resourceAccessRuleSetsContainer.GetBlobs())
            {
                await claimPermissionsContainer.DeleteBlobAsync(blob.Name);
            }
        }
    }
}
