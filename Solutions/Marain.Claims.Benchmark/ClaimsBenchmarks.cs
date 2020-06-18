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
                .AddSingleton(root.GetSection("TenancyClient").Get<TenancyClientOptions>())
                .AddTenancyClient()
                .AddAzureManagedIdentityBasedTokenSource(
                    sp => new AzureManagedIdentityTokenSourceOptions
                    {
                        AzureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"],
                    })
                .BuildServiceProvider();

            this.claimsService = serviceProvider.GetRequiredService<IClaimsService>();
        }

        /// <summary>
        /// Invoked by BenchmarkDotNet before running all benchmarks.
        /// </summary>
        [GlobalSetup]
        public void GlobalSetup()
        {
            this.SetupTestDataAsync(this.clientTenantId).Wait();
        }

        /// <summary>
        /// Invoked by BenchmarkDotNet after running all benchmarks.
        /// </summary>
        [GlobalCleanup]
        public void GlobalCleanup()
        {
            //this.DeleteTestDataAsync(this.clientTenant.Id).Wait();
        }

        /// <summary>
        /// Benchmark: TODO
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task B1() => Task.CompletedTask;

        private async Task SetupTestDataAsync(string clientTenantId)
        {
            ProblemDetails initializeTenantResponse = await this.claimsService.InitializeTenantAsync(clientTenantId, new Body { AdministratorRoleClaimValue = "ClaimsAdministrator" });

            if ((initializeTenantResponse.Status < 200 || initializeTenantResponse.Status >= 300) && initializeTenantResponse.Detail != "Tenant already initialized")
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
                await this.claimsService.CreateClaimPermissionsAsync(clientTenantId, claimPermissions);
            }
        }
    }
}
