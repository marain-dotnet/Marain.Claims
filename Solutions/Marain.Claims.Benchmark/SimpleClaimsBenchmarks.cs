using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Marain.Claims.Client;
using Marain.Claims.Client.Models;

namespace Marain.Claims.Benchmark
{
    /// <summary>
    /// Defines the benchmarks for a simple claims scenario.
    /// </summary>
    /// <remarks>
    /// In order to run the benchmarks, you must have a configured Marain client tenant that has been enrolled
    /// with Marain Claims. The principal you use for running the benchmarks (either yourself or a service principal, 
    /// set via the AzureServicesAuthConnectionString setting in local.settings.json), must have access to both the 
    /// Claims API and the Key Vault containing the claims tenant configuration.
    /// Also, be aware that running these benchmarks will delete all data in the specified claims tenant storage,
    /// so do not use this against production instances.
    /// </remarks>
    [JsonExporterAttribute.Full]
    [MarkdownExporter]
    public class SimpleClaimsBenchmarks : ClaimsBenchmarksBase
    {
        private string iterationStr = "1";
        private int iteration = 1;

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
        public Task CreateClaimPermissions() => this.ClaimsService.CreateClaimPermissionsAsync(
            this.ClientTenantId, 
            new ClaimPermissions
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
        public Task CreateResourceAccessRuleSet() => this.ClaimsService.CreateResourceAccessRuleSetAsync(
            this.ClientTenantId, 
            new ResourceAccessRuleSet
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
        public Task GetClaimPermissions() => this.ClaimsService.GetClaimPermissionsAsync(this.iterationStr, this.ClientTenantId);

        /// <summary>
        /// Benchmark: GetClaimPermissionsPermissionBatch
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetClaimPermissionsPermissionBatch() => this.ClaimsService.GetClaimPermissionsPermissionBatchAsync(
            this.ClientTenantId, 
            new List<ClaimPermissionsBatchRequestItem> 
            { 
                new ClaimPermissionsBatchRequestItem("0", "0", "GET"),
                new ClaimPermissionsBatchRequestItem("1", "1", "GET"),
                new ClaimPermissionsBatchRequestItem("2", "2", "GET"),
                new ClaimPermissionsBatchRequestItem("3", "3", "GET"),
                new ClaimPermissionsBatchRequestItem("4", "4", "GET"),
                new ClaimPermissionsBatchRequestItem("5", "5", "GET"),
                new ClaimPermissionsBatchRequestItem("6", "6", "GET"),
                new ClaimPermissionsBatchRequestItem("7", "7", "GET"),
                new ClaimPermissionsBatchRequestItem("8", "8", "GET"),
                new ClaimPermissionsBatchRequestItem("9", "9", "GET"),
            }
        );

        /// <summary>
        /// Benchmark: GetClaimPermissionsResourceAccessRules
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetClaimPermissionsResourceAccessRules() => this.ClaimsService.GetClaimPermissionsResourceAccessRulesAsync("0", this.ClientTenantId);

        /// <summary>
        /// Benchmark: GetResourceAccessRuleSet
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetResourceAccessRuleSet() => this.ClaimsService.GetResourceAccessRuleSetAsync("0", this.ClientTenantId);

        /// <summary>
        /// Benchmark: SetClaimPermissionsResourceAccessRules
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task SetClaimPermissionsResourceAccessRules() => this.ClaimsService.SetClaimPermissionsResourceAccessRulesAsync(
            this.ClientTenantId,
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
        public Task SetClaimPermissionsResourceAccessRuleSets() => this.ClaimsService.SetClaimPermissionsResourceAccessRuleSetsAsync(
            this.ClientTenantId,
            this.iterationStr,
            new List<ResourceAccessRuleSet>
            {
                new ResourceAccessRuleSet(this.iterationStr)
            }
        );

        /// <summary>
        /// Benchmark: SetResourceAccessRuleSetResourceAccessRules
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task SetResourceAccessRuleSetResourceAccessRules() => this.ClaimsService.SetResourceAccessRuleSetResourceAccessRulesAsync(
            this.ClientTenantId,
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
        public Task UpdateClaimPermissionsResourceAccessRules() => this.ClaimsService.UpdateClaimPermissionsResourceAccessRulesAsync(
            this.ClientTenantId,
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
        public Task UpdateResourceAccessRuleSetResourceAccessRules() => this.ClaimsService.UpdateResourceAccessRuleSetResourceAccessRulesAsync(
            this.ClientTenantId,
            this.iterationStr,
            "add",
            new List<ResourceAccessRule>
            {
                new ResourceAccessRule("POST", new Resource($"benchmark{this.iterationStr}", $"benchmark{this.iterationStr}"), "allow")
            }
        );

        protected override async Task SetupTestDataAsync()
        {
            ProblemDetails initializeTenantResponse = await this.ClaimsService.InitializeTenantAsync(this.ClientTenantId, new Body { AdministratorRoleClaimValue = "ClaimsAdministrator" });

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
                    .Select(i => new ResourceAccessRuleSet
                    {
                        DisplayName = i,
                        Id = i,
                        Rules = new List<ResourceAccessRule> 
                        {
                            new ResourceAccessRule(i, new Resource(i, i), "allow")
                        }
                    })
                    .ToList();

            foreach (ResourceAccessRuleSet ruleSet in ruleSets)
            {
                object response = await this.ClaimsService.CreateResourceAccessRuleSetAsync(ClientTenantId, ruleSet);
            }

            var claimPermissionsList =
                Enumerable
                    .Range(0, 50)
                    .Select(i => i.ToString())
                    .Select(i => new ClaimPermissions
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

            foreach (ClaimPermissions claimPermissions in claimPermissionsList)
            {
                object response = await this.ClaimsService.CreateClaimPermissionsAsync(ClientTenantId, claimPermissions);
            }
        }
    }
}
