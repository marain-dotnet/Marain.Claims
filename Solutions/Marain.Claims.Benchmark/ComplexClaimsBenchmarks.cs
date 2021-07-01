namespace Marain.Claims.Benchmark
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the benchmarks for a complex claims scenario.
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
    public class ComplexClaimsBenchmarks : ClaimsBenchmarksBase
    {
        /// <summary>
        /// Invoked by BenchmarkDotNet before running all benchmarks.
        /// </summary>
        [GlobalSetup]
        public async Task GlobalSetup()
        {
            await this.DeleteTestDataAsync();
            await this.SetupTestDataAsync();
        }

        /// <summary>
        /// Invoked by BenchmarkDotNet after running all benchmarks.
        /// </summary>
        [GlobalCleanup]
        public async Task GlobalCleanup()
        {
            await this.DeleteTestDataAsync();
        }

        /// <summary>
        /// Benchmark: GetClaimPermissions
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetClaimPermissions() => this.ClaimsService.GetClaimPermissionsAsync("One", this.ClientTenantId);

        /// <summary>
        /// Benchmark: GetClaimPermissionsPermissionBatchMultipleClaimPermissions
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetClaimPermissionsPermissionBatchMultipleClaimPermissions() => this.ClaimsService.GetClaimPermissionsPermissionBatchAsync(
            this.ClientTenantId, 
            new List<ClaimPermissionsBatchRequestItem> 
            { 
                new ClaimPermissionsBatchRequestItem("One", "api/foo/123/freds", "POST"),
                new ClaimPermissionsBatchRequestItem("Two", "api/foo/123/freds", "POST"),
                new ClaimPermissionsBatchRequestItem("Three", "api/foo/123/freds", "POST"),
                new ClaimPermissionsBatchRequestItem("Four", "api/foo/123/freds", "POST"),
                new ClaimPermissionsBatchRequestItem("Five", "api/foo/123/freds", "POST"),
                new ClaimPermissionsBatchRequestItem("Six", "api/foo/123/freds", "POST"),
                new ClaimPermissionsBatchRequestItem("Seven", "api/foo/123/freds", "POST"),
                new ClaimPermissionsBatchRequestItem("Eight", "api/foo/123/freds", "POST"),
            }
        );

        /// <summary>
        /// Benchmark: GetClaimPermissionsPermissionBatchMultipleClaimPermissions
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetClaimPermissionsPermissionBatchMultipleResources() => this.ClaimsService.GetClaimPermissionsPermissionBatchAsync(
            this.ClientTenantId,
            new List<ClaimPermissionsBatchRequestItem>
            {
                new ClaimPermissionsBatchRequestItem("One", "api/foo/123/freds", "GET"),
                new ClaimPermissionsBatchRequestItem("One", "api/foo/123/freds/456/xyzzy", "GET"),
                new ClaimPermissionsBatchRequestItem("One", "api/foo/123/freds/456/xyzzy-results", "GET"),
                new ClaimPermissionsBatchRequestItem("One", "api/foo/123/freds/456/corge", "GET"),
                new ClaimPermissionsBatchRequestItem("One", "api/foo/123/freds/456/grault", "GET"),
                new ClaimPermissionsBatchRequestItem("One", "api/foo/123/freds/456/plugh", "GET"),
                new ClaimPermissionsBatchRequestItem("One", "api/foo/123/freds/456/garply", "GET"),
                new ClaimPermissionsBatchRequestItem("One", "api/foo/123/freds/456/review", "GET")
            }
        );

        /// <summary>
        /// Benchmark: GetClaimPermissionsResourceAccessRules
        /// </summary>
        /// <returns>A task that completes when the benchmark has finished.</returns>
        [Benchmark]
        public Task GetClaimPermissionsResourceAccessRules() => this.ClaimsService.GetClaimPermissionsResourceAccessRulesAsync("One", this.ClientTenantId);

        /// <summary>
        protected override async Task SetupTestDataAsync()
        {
            RulesetsAndClaimPermissions input = JsonConvert.DeserializeObject<RulesetsAndClaimPermissions>(
                File.ReadAllText("BenchmarkClaimPermissions.json"));

            ProblemDetails initializeTenantResponse = await this.ClaimsService.InitializeTenantAsync(this.ClientTenantId, new Body { AdministratorPrincipalObjectId = this.AdministratorPrincipalObjectId });

            if (initializeTenantResponse != null &&
                (initializeTenantResponse.Status < 200 || initializeTenantResponse.Status >= 300) &&
                initializeTenantResponse.Detail != "Tenant already initialized")
            {
                throw new Exception(initializeTenantResponse.Detail);
            }

            foreach (ResourceAccessRuleSet ruleSet in input.RuleSets)
            {
                object response = await this.ClaimsService.CreateResourceAccessRuleSetAsync(ClientTenantId, ruleSet);
            }

            foreach (CreateClaimPermissionsRequest claimPermissions in input.ClaimPermissions)
            {
                object response = await this.ClaimsService.CreateClaimPermissionsAsync(ClientTenantId, claimPermissions);
            }
        }

        private class RulesetsAndClaimPermissions
        {
            public IList<ResourceAccessRuleSet> RuleSets { get; set; }

            public IList<CreateClaimPermissionsRequest> ClaimPermissions { get; set; }
        }
    }
}
