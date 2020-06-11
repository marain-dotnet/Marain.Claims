// <copyright file="RepositorySteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Elements should be documented

namespace Marain.Claims.SpecFlow.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.TenantManagement.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public class ClaimPermissionsStoreSteps
    {
        public const string ClaimPermissionsResult = "results/claimpermissions";

        private readonly ScenarioContext scenarioContext;
        private readonly FeatureContext featureContext;
        private readonly IServiceProvider serviceProvider;

        public ClaimPermissionsStoreSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
            this.serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
        }

        [Given(@"I have a list of resource access rules called ""(.*)""")]
        public void GivenIHaveAListOfResourceAccessRulesCalled(string rulesName, Table table)
        {
            IList<ResourceAccessRule> rules = table.Rows.Select(x =>
            {
                var resource = new Resource(new Uri(x[1], UriKind.Relative), x[1]);
                var permission = (Permission)Enum.Parse(typeof(Permission), x[2], true);
                return new ResourceAccessRule(x[0], resource, permission);
            }).ToList();

            this.scenarioContext.Set(rules, rulesName);
        }

        [Given(@"I have resource access rulesets called ""(.*)""")]
        public void GivenIHaveResourceAccessRulesetsCalled(string rulesetsName, Table table)
        {
            var data = table.CreateSet<ResourceAccessRuleSet>().ToList();
            this.scenarioContext.Set(data, rulesetsName);
        }

        [Given(@"I have claim permissions called ""(.*)""")]
        public void GivenIHaveClaimPermissionsCalled(string permissionsName, Table table)
        {
            var data = table.CreateSet<ClaimPermissions>().ToList();

            // When we create the claim permissions, we need the rulesets to be trimmed down to only include their names.
            foreach (ClaimPermissions current in data)
            {
                current.ResourceAccessRuleSets = current.ResourceAccessRuleSets.Select(x => new ResourceAccessRuleSet { Id = x.Id }).ToList();
            }

            this.scenarioContext.Set(data, permissionsName);
        }

        [Given(@"I have saved the resource access rulesets called ""(.*)"" to the resource access ruleset store")]
        public async Task GivenIHaveSavedTheResourceAccessRulesetsCalledToTheResourceAccessRulesetStore(string rulesetsName)
        {
            List<ResourceAccessRuleSet> rulesets = this.scenarioContext.Get<List<ResourceAccessRuleSet>>(rulesetsName);
            IResourceAccessRuleSetStore store = await this.GetResourceAccessRuleSetStoreAsync().ConfigureAwait(false);

            IEnumerable<Task<ResourceAccessRuleSet>> tasks = rulesets.Select(store.PersistAsync);

            await Task.WhenAll(tasks);
        }

        [Given(@"I have saved the claim permissions called ""(.*)"" to the claim permissions store")]
        public async Task GivenIHaveSavedTheClaimPermissionsCalledToTheClaimPermissionsStore(string claimPermissionsName)
        {
            List<ClaimPermissions> claimPermissions = this.scenarioContext.Get<List<ClaimPermissions>>(claimPermissionsName);
            IClaimPermissionsStore claimPermissionsStore = await this.GetClaimPermissionsStoreAsync().ConfigureAwait(false);

            IEnumerable<Task<ClaimPermissions>> tasks = claimPermissions.Select(claimPermissionsStore.PersistAsync);

            await Task.WhenAll(tasks);
        }

        [When(@"I request the claim permission with Id ""(.*)"" from the claim permissions store")]
        public async Task WhenIRequestTheClaimPermissionWithIdFromTheClaimPermissionsStore(string p0)
        {
            IClaimPermissionsStore store = await this.GetClaimPermissionsStoreAsync().ConfigureAwait(false);

            try
            {
                ClaimPermissions result = await store.GetAsync(p0).ConfigureAwait(false);
                this.scenarioContext.Set(result, ClaimPermissionsResult);
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex);
            }
        }

        [Then(@"the claim permission is returned")]
        public void ThenTheClaimPermissionIsReturned()
        {
            if (!this.scenarioContext.TryGetValue(ClaimPermissionsResult, out ClaimPermissions _))
            {
                Assert.Fail("The expected result was not found in the scenario context.");
            }
        }

        [Then(@"the resource access rulesets on the claim permission match the rulesets ""(.*)""")]
        public void ThenTheResourceAccessRulesetsOnTheClaimPermissionMatchTheExpectedRulesets(string expectedRulesetsName)
        {
            ClaimPermissions loadedClaimPermissions = this.scenarioContext.Get<ClaimPermissions>(ClaimPermissionsResult);
            List<ResourceAccessRuleSet> expectedRulesets = this.scenarioContext.Get<List<ResourceAccessRuleSet>>(expectedRulesetsName);

            Assert.AreEqual(expectedRulesets.Count, loadedClaimPermissions.ResourceAccessRuleSets.Count, "The loaded ClaimPermissions did not contain the expected number of ResourceAccessRulesets");

            foreach (ResourceAccessRuleSet current in expectedRulesets)
            {
                ResourceAccessRuleSet loadedRuleset = loadedClaimPermissions.ResourceAccessRuleSets.FirstOrDefault(x => x.Id == current.Id);

                Assert.NotNull(loadedRuleset, $"The loaded ClaimPermissions did not contain the expected ResourceAccessRuleset with Id '{current.Id}'");
                Assert.That(loadedRuleset.Rules, Is.EquivalentTo(current.Rules), $"The loaded ResourceAccessRuleset with Id '{current.Id}' was not equivalent to the expected ResourceAccessRuleset");
            }
        }

        [Then(@"a ""(.*)"" exception is thrown")]
        public void ThenAExceptionIsThrown(string p0)
        {
            if (!this.scenarioContext.TryGetValue<Exception>(out Exception exception))
            {
                Assert.Fail("No exception was thrown");
            }

            Assert.AreEqual(p0, exception.GetType().Name);
        }

        private Task<IResourceAccessRuleSetStore> GetResourceAccessRuleSetStoreAsync()
        {
            ITenant transientTenant = TransientTenantManager.GetInstance(featureContext).PrimaryTransientClient;
            IPermissionsStoreFactory permissionsStoreFactory = this.serviceProvider.GetRequiredService<IPermissionsStoreFactory>();
            return permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(transientTenant);
        }

        private Task<IClaimPermissionsStore> GetClaimPermissionsStoreAsync()
        {
            ITenant transientTenant = TransientTenantManager.GetInstance(featureContext).PrimaryTransientClient;
            IPermissionsStoreFactory permissionsStoreFactory = this.serviceProvider.GetRequiredService<IPermissionsStoreFactory>();
            return permissionsStoreFactory.GetClaimPermissionsStoreAsync(transientTenant);
        }
    }
}
