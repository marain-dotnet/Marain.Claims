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
    /// In order to run the benchmarks, you must have a configured Marain client tenant that has been enrolled
    /// with Marain Claims. The principal you use for running the benchmarks (either yourself or a service principal, 
    /// set via the AzureServicesAuthConnectionString setting in local.settings.json), must have access to both the 
    /// Claims API and the Key Vault containing the claims tenant configuration.
    /// </remarks>
    [JsonExporterAttribute.Full]
    [MarkdownExporter]
    public class ClaimsBenchmarks
    {
        private readonly IClaimsService claimsService;
        private readonly ITenancyService tenancyService;
        private readonly ITenantCloudBlobContainerFactory tenantCloudBlobContainerFactory;
        private readonly IPropertyBagFactory propertyBagFactory;
        private readonly string clientTenantId;

        private string iterationStr = "1";
        private int iteration = 1;

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

        [IterationCleanup]
        public void IterationCleanup()
        {
            this.iteration++;
            this.iterationStr = this.iteration.ToString();
        }

        /// <summary>
        /// Benchmark: CreateClaimPermissions
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task CreateClaimPermissions() => this.claimsService.CreateClaimPermissionsAsync(
            this.clientTenantId, 
            new ClaimPermissionsWithPostExample
            {
                Id = $"benchmark{this.iterationStr}",
                ResourceAccessRules = new List<ResourceAccessRule>
                {
                    new ResourceAccessRule("GET", new Resource($"benchmark{this.iterationStr}", $"benchmark{this.iterationStr}"), "allow")
                }
            }
        );

        /// <summary>
        /// Benchmark: CreateResourceAccessRuleSet
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task CreateResourceAccessRuleSet() => this.claimsService.CreateResourceAccessRuleSetAsync(
            this.clientTenantId, 
            new ResourceAccessRuleSetWithPostExample
            {
                DisplayName = $"benchmark{this.iterationStr}",
                Id = $"benchmark{this.iterationStr}",
                Rules = new List<ResourceAccessRule>
                {
                    new ResourceAccessRule("GET", new Resource($"benchmark{this.iterationStr}", $"benchmark{this.iterationStr}"), "allow")
                }
            }
        );

        /// <summary>
        /// Benchmark: GetClaimPermissions
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetClaimPermissions() => this.claimsService.GetClaimPermissionsAsync(this.iterationStr, this.clientTenantId);

        /// <summary>
        /// Benchmark: GetClaimPermissionsPermissionBatch
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetClaimPermissionsPermissionBatch() => this.claimsService.GetClaimPermissionsPermissionBatchAsync(
            this.clientTenantId, 
            new List<ClaimPermissionsBatchRequestItemWithPostExample> 
            { 
                new ClaimPermissionsBatchRequestItemWithPostExample("0", "0", "GET"),
                new ClaimPermissionsBatchRequestItemWithPostExample("1", "1", "GET"),
                new ClaimPermissionsBatchRequestItemWithPostExample("2", "2", "GET"),
                new ClaimPermissionsBatchRequestItemWithPostExample("3", "3", "GET"),
                new ClaimPermissionsBatchRequestItemWithPostExample("4", "4", "GET"),
                new ClaimPermissionsBatchRequestItemWithPostExample("5", "5", "GET"),
                new ClaimPermissionsBatchRequestItemWithPostExample("6", "6", "GET"),
                new ClaimPermissionsBatchRequestItemWithPostExample("7", "7", "GET"),
                new ClaimPermissionsBatchRequestItemWithPostExample("8", "8", "GET"),
                new ClaimPermissionsBatchRequestItemWithPostExample("9", "9", "GET"),
            }
        );

        /// <summary>
        /// Benchmark: GetClaimPermissionsResourceAccessRules
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetClaimPermissionsResourceAccessRules() => this.claimsService.GetClaimPermissionsResourceAccessRulesAsync("0", this.clientTenantId);

        /// <summary>
        /// Benchmark: GetResourceAccessRuleSet
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetResourceAccessRuleSet() => this.claimsService.GetResourceAccessRuleSetAsync("0", this.clientTenantId);

        /// <summary>
        /// Benchmark: SetClaimPermissionsResourceAccessRules
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task SetClaimPermissionsResourceAccessRules() => this.claimsService.SetClaimPermissionsResourceAccessRulesAsync(
            this.clientTenantId,
            this.iterationStr, 
            new List<ResourceAccessRule>
            {
                new ResourceAccessRule("GET", new Resource($"benchmark{this.iterationStr}", $"benchmark{this.iterationStr}"), "deny")
            }
        );

        /// <summary>
        /// Benchmark: SetClaimPermissionsResourceAccessRuleSets
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task SetClaimPermissionsResourceAccessRuleSets() => this.claimsService.SetClaimPermissionsResourceAccessRuleSetsAsync(
            this.clientTenantId,
            this.iterationStr,
            new List<ResourceAccessRuleSetId>
            {
                new ResourceAccessRuleSetId(this.iterationStr)
            }
        );

        /// <summary>
        /// Benchmark: SetResourceAccessRuleSetResourceAccessRules
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task SetResourceAccessRuleSetResourceAccessRules() => this.claimsService.SetResourceAccessRuleSetResourceAccessRulesAsync(
            this.clientTenantId,
            this.iterationStr,
            new List<ResourceAccessRule>
            {
                new ResourceAccessRule("GET", new Resource($"benchmark{this.iterationStr}", $"benchmark{this.iterationStr}"), "deny")
            }
        );

        /// <summary>
        /// Benchmark: UpdateClaimPermissionsResourceAccessRules
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task UpdateClaimPermissionsResourceAccessRules() => this.claimsService.UpdateClaimPermissionsResourceAccessRulesAsync(
            this.clientTenantId,
            this.iterationStr,
            "add",
            new List<ResourceAccessRule>
            {
                new ResourceAccessRule("POST", new Resource($"benchmark{this.iterationStr}", $"benchmark{this.iterationStr}"), "allow")
            }
        );

        /// <summary>
        /// Benchmark: UpdateResourceAccessRuleSetResourceAccessRules
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task UpdateResourceAccessRuleSetResourceAccessRules() => this.claimsService.UpdateResourceAccessRuleSetResourceAccessRulesAsync(
            this.clientTenantId,
            this.iterationStr,
            "add",
            new List<ResourceAccessRule>
            {
                new ResourceAccessRule("POST", new Resource($"benchmark{this.iterationStr}", $"benchmark{this.iterationStr}"), "allow")
            }
        );

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
