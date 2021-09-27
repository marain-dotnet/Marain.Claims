using System;
using System.Linq;
using System.Threading.Tasks;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

using Corvus.Azure.Storage.Tenancy;
using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
using Corvus.Json;
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
        protected readonly ITenantBlobContainerClientFactory TenantBlobContainerClientFactory;
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
                .AddTenantBlobContainerClientFactory(sp => new TenantBlobContainerClientFactoryOptions
                {
                    AzureServicesAuthConnectionString = configuration["AzureServicesAuthConnectionString"]
                })
                .AddAzureManagedIdentityBasedTokenSource(
                    sp => new AzureManagedIdentityTokenSourceOptions
                    {
                        AzureServicesAuthConnectionString = configuration["AzureServicesAuthConnectionString"],
                    })
                .BuildServiceProvider();

            this.ClaimsService = serviceProvider.GetRequiredService<IClaimsService>();
            this.TenancyService = serviceProvider.GetRequiredService<ITenancyService>();
            this.TenantBlobContainerClientFactory = serviceProvider.GetRequiredService<ITenantBlobContainerClientFactory>();
            this.PropertyBagFactory = serviceProvider.GetRequiredService<IPropertyBagFactory>();
        }

        protected abstract Task SetupTestDataAsync();

        protected async Task DeleteTestDataAsync()
        {
            var response = (JObject)await this.TenancyService.GetTenantAsync(this.ClientTenantId);

            Tenancy.Client.Models.Tenant clientTenant = JsonConvert.DeserializeObject<Tenancy.Client.Models.Tenant>(response.ToString());

            var tenant = new Tenant(clientTenant.Id, clientTenant.Name, this.PropertyBagFactory.Create(clientTenant.Properties));

            BlobContainerClient claimPermissionsContainer =
                await this.TenantBlobContainerClientFactory.GetBlobContainerForTenantAsync(tenant, new BlobStorageContainerDefinition("claimpermissions"));
            BlobContainerClient resourceAccessRuleSetsContainer =
                await this.TenantBlobContainerClientFactory.GetBlobContainerForTenantAsync(tenant, new BlobStorageContainerDefinition("resourceaccessrulesets"));

            foreach (BlobItem blob in claimPermissionsContainer.GetBlobs())
            {
                await claimPermissionsContainer.DeleteBlobAsync(blob.Name);
            }
        }
    }
}
