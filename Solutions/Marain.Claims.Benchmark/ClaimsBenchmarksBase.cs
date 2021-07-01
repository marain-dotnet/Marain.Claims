using System;
using System.Linq;
using System.Threading.Tasks;
using Corvus.Azure.Storage.Tenancy;
using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
using Corvus.Json;
using Corvus.Tenancy;
using Marain.Claims.Client;
using Marain.Tenancy.Client;
using Microsoft.Azure.Storage.Blob;
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
        protected readonly ITenantCloudBlobContainerFactory TenantCloudBlobContainerFactory;
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
                .AddTenancyClient(enableResponseCaching: false)
                .AddTenantCloudBlobContainerFactory(sp => new TenantCloudBlobContainerFactoryOptions
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
            this.TenantCloudBlobContainerFactory = serviceProvider.GetRequiredService<ITenantCloudBlobContainerFactory>();
            this.PropertyBagFactory = serviceProvider.GetRequiredService<IPropertyBagFactory>();
        }

        protected abstract Task SetupTestDataAsync();

        protected async Task DeleteTestDataAsync()
        {
            var response = (JObject)await this.TenancyService.GetTenantAsync(this.ClientTenantId);

            Tenancy.Client.Models.Tenant clientTenant = JsonConvert.DeserializeObject<Tenancy.Client.Models.Tenant>(response.ToString());

            var tenant = new Tenant(clientTenant.Id, clientTenant.Name, this.PropertyBagFactory.Create(clientTenant.Properties));

            CloudBlobContainer claimPermissionsContainer =
                await this.TenantCloudBlobContainerFactory.GetBlobContainerForTenantAsync(tenant, new BlobStorageContainerDefinition("claimpermissions"));
            CloudBlobContainer resourceAccessRuleSetsContainer =
                await this.TenantCloudBlobContainerFactory.GetBlobContainerForTenantAsync(tenant, new BlobStorageContainerDefinition("resourceaccessrulesets"));

            foreach (CloudBlockBlob blob in claimPermissionsContainer.ListBlobs().OfType<CloudBlockBlob>())
            {
                await blob.DeleteAsync();
            }

            foreach (CloudBlockBlob blob in resourceAccessRuleSetsContainer.ListBlobs().OfType<CloudBlockBlob>())
            {
                await blob.DeleteAsync();
            }
        }
    }
}
