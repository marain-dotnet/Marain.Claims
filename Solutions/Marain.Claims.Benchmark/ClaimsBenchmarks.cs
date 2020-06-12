namespace Marain.Claims.Benchmark
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using Marain.Tenancy.Client;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Defines all of the benchmarks and global setup/teardown.
    /// </summary>
    [JsonExporterAttribute.Full]
    public class ClaimsBenchmarks
    {
        private readonly IClaimsService claimsService;
        private readonly ITenancyService tenancyService;
        //private readonly ITenantManagementService tenantManagementService;

        private readonly string serviceTenantId;
        private readonly string clientTenantId;
        //private readonly BlobStorageConfiguration blobStorageConfiguration;
        //private ITenant clientTenant;

        public ClaimsBenchmarks()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                       .AddEnvironmentVariables()
                       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            IConfiguration root = configurationBuilder.Build();

            var claimsBaseUri = new Uri(root["ClaimsClient:BaseUrl"]);
            string claimsResourceIdForMsiAuthentication = root["ClaimsClient:ResourceIdForMsiAuthentication"];

            //this.serviceTenantId = root["MarainServiceConfiguration:ServiceTenantId"];
            this.clientTenantId = root["ClientTenantId"];

            //this.blobStorageConfiguration = 
            //    root.GetSection("TestBlobStorageConfiguration").Get<BlobStorageConfiguration>();

            ServiceProvider serviceProvider = new ServiceCollection()
                .AddClaimsClient(claimsBaseUri, claimsResourceIdForMsiAuthentication)
                .AddSingleton(root.GetSection("TenancyClient").Get<TenancyClientOptions>())
                .AddTenancyClient()
                .AddAzureManagedIdentityBasedTokenSource(
                    sp => new AzureManagedIdentityTokenSourceOptions
                    {
                        AzureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"],
                    })
                //.AddMarainTenantManagement()
                .BuildServiceProvider();

            this.claimsService = serviceProvider.GetRequiredService<IClaimsService>();
            this.tenancyService = serviceProvider.GetRequiredService<ITenancyService>();
        }

        /// <summary>
        /// Invoked by BenchmarkDotNet before running all benchmarks.
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup()
        {
            //this.clientTenant = this.SetupClientTenantAsync(this.serviceTenantId, this.blobStorageConfiguration).Result;

            this.SetupTestDataAsync(this.clientTenantId).Wait();

        }

        /// <summary>
        /// Invoked by BenchmarkDotNet after running all benchmarks.
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            //this.DeleteTestDataAsync(this.clientTenant.Id).Wait();

            //this.DeleteClientTenantAsync(this.clientTenant, this.serviceTenantId, this.blobStorageConfiguration).Wait();
        }

        /// <summary>
        /// Benchmark: 
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task B1() => Task.CompletedTask;

        //private async Task<ITenant> SetupClientTenantAsync(string serviceTenantId, BlobStorageConfiguration blobStorageConfiguration)
        //{
        //    string clientName = $"benchmark_{Guid.NewGuid()}";

        //    ITenant clientTenant = await this.tenantManagementService.CreateClientTenantAsync(clientName);
        //    ITenant serviceTenant = await this.tenantManagementService.GetServiceTenantAsync(serviceTenantId);

        //    var enrollmentConfig = new EnrollmentBlobStorageConfigurationItem
        //    {
        //        Configuration = blobStorageConfiguration
        //    };

        //    await this.tenantManagementService.EnrollInServiceAsync(clientTenant, serviceTenant, new EnrollmentConfigurationItem[] { enrollmentConfig });

        //    return clientTenant;
        //}

        //private async Task DeleteClientTenantAsync(ITenant clientTenant, string serviceTenantId, BlobStorageConfiguration blobStorageConfiguration)
        //{
        //    ITenant serviceTenant = await this.tenantManagementService.GetServiceTenantAsync(serviceTenantId);

        //    await this.tenantManagementService.UnenrollFromServiceAsync(clientTenant, serviceTenant);

        //    //TODO: delete the tenant - method does not currently exist in tenant management service
        //}

        private async Task SetupTestDataAsync(string clientTenantId)
        {
            ProblemDetails initializeTenantResponse = await this.claimsService.InitializeTenantAsync(clientTenantId, new Body { AdministratorRoleClaimValue = "ClaimsAdministrator" });

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
                await this.claimsService.CreateClaimPermissionsAsync(clientTenantId, claimPermissions);
            }
        }
    }
}
