namespace Marain.Claims.Benchmark
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Json;
    using Corvus.Tenancy;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using Marain.Tenancy.Client;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines all of the benchmarks and global setup/teardown.
    /// </summary>
    /// <remarks>
    /// You must grant the service principal (or yourself) permission to read secrets from the tenant's key vault.
    /// </remarks>
    [JsonExporterAttribute.Full]
    public class ClaimsBenchmarks
    {
        private readonly IClaimsService claimsService;
        private readonly ITenancyService tenancyService;
        private readonly ITenantCloudBlobContainerFactory tenantCloudBlobContainerFactory;
        private readonly IPropertyBagFactory propertyBagFactory;
        private readonly string clientTenantId;

        public ClaimsBenchmarks()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                       .AddEnvironmentVariables()
                       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            IConfiguration root = configurationBuilder.Build();

            this.clientTenantId = root["ClientTenantId"];

            ServiceProvider serviceProvider = new ServiceCollection()
                .AddClaimsClient(sp => root.GetSection("ClaimsClient").Get<ClaimsClientOptions>())
                .AddSingleton(sp => new TenancyClientOptions { TenancyServiceBaseUri = new Uri(root["TenancyClient:TenancyServiceBaseUri"]), ResourceIdForMsiAuthentication = root["TenancyClient:ResourceIdForMsiAuthentication"] })
                .AddTenancyClient()
                .AddTenantCloudBlobContainerFactory(sp => new TenantCloudBlobContainerFactoryOptions 
                    { 
                        AzureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"] 
                    })
                .AddAzureManagedIdentityBasedTokenSource(
                    sp => new AzureManagedIdentityTokenSourceOptions
                    {
                        AzureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"],
                    })
                .BuildServiceProvider();

            this.claimsService = serviceProvider.GetRequiredService<IClaimsService>();
            this.tenancyService = serviceProvider.GetRequiredService<ITenancyService>();
            this.tenantCloudBlobContainerFactory = serviceProvider.GetRequiredService<ITenantCloudBlobContainerFactory>();
            this.propertyBagFactory = serviceProvider.GetRequiredService<IPropertyBagFactory>();
        }

        /// <summary>
        /// Invoked by BenchmarkDotNet before running all benchmarks.
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup()
        {
            this.DeleteTestDataAsync().Wait();
            this.SetupTestDataAsync().Wait();
        }

        /// <summary>
        /// Invoked by BenchmarkDotNet after running all benchmarks.
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            this.DeleteTestDataAsync().Wait();
        }

        /// <summary>
        /// Benchmark: TODO
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task B1() => Task.CompletedTask;

        private async Task DeleteTestDataAsync()
        {
            var response = (JObject)await this.tenancyService.GetTenantAsync(this.clientTenantId);

            Tenancy.Client.Models.Tenant clientTenant = JsonConvert.DeserializeObject<Tenancy.Client.Models.Tenant>(response.ToString());

            var tenant = new Tenant(clientTenant.Id, clientTenant.Name, this.propertyBagFactory.Create(clientTenant.Properties));

            CloudBlobContainer claimPermissionsContainer = 
                await this.tenantCloudBlobContainerFactory.GetBlobContainerForTenantAsync(tenant, new BlobStorageContainerDefinition("claimpermissions"));
            CloudBlobContainer resourceAccessRuleSetsContainer = 
                await this.tenantCloudBlobContainerFactory.GetBlobContainerForTenantAsync(tenant, new BlobStorageContainerDefinition("resourceaccessrulesets"));

            foreach (CloudBlockBlob blob in claimPermissionsContainer.ListBlobs().OfType<CloudBlockBlob>())
            {
                await blob.DeleteAsync();
            }

            foreach (CloudBlockBlob blob in resourceAccessRuleSetsContainer.ListBlobs().OfType<CloudBlockBlob>())
            {
                await blob.DeleteAsync();
            }
        }

        private async Task SetupTestDataAsync()
        {
            ProblemDetails initializeTenantResponse = await this.claimsService.InitializeTenantAsync(this.clientTenantId, new Body { AdministratorRoleClaimValue = "ClaimsAdministrator" });

            if (initializeTenantResponse != null && 
                (initializeTenantResponse.Status < 200 || initializeTenantResponse.Status >= 300) && 
                initializeTenantResponse.Detail != "Tenant already initialized")
            {
                throw new Exception(initializeTenantResponse.Detail);
            }

            var ruleSets =
                Enumerable
                    .Range(0, 50)
                    .Select(i => i.ToString())
                    .Select(i => new ResourceAccessRuleSetWithPostExample
                    {
                        DisplayName = i,
                        Id = i,
                        Rules = new List<ResourceAccessRule> 
                        {
                            new ResourceAccessRule(i, new Resource(i, i), "allow")
                        }
                    })
                    .ToList();

            foreach (ResourceAccessRuleSetWithPostExample ruleSet in ruleSets)
            {
                object response = await this.claimsService.CreateResourceAccessRuleSetAsync(clientTenantId, ruleSet);
            }

            var claimPermissionSets =
                Enumerable
                    .Range(0, 50)
                    .Select(i => i.ToString())
                    .Select(i => new ClaimPermissionsWithPostExample
                    {
                        Id = i.ToString(),
                        ResourceAccessRules = new List<ResourceAccessRule>
                        {
                            new ResourceAccessRule(i, new Resource(i, i), "allow")
                        },
                        ResourceAccessRuleSets = new List<ResourceAccessRuleSet>
                        {
                            new ResourceAccessRuleSet(i)
                        }
                    })
                    .ToList();

            foreach (ClaimPermissionsWithPostExample claimPermissions in claimPermissionSets)
            {
                object response = await this.claimsService.CreateClaimPermissionsAsync(clientTenantId, claimPermissions);
            }
        }
    }
}
