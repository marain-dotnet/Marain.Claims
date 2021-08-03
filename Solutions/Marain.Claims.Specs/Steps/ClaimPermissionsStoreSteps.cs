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
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
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
        private readonly Dictionary<string, string> claimPermissionIds = new Dictionary<string, string>();

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

            // We need the ClaimPermission ids to be different every time because we create test
            // tenants per-feature, but we create claims per-scenario. Now that everything has to
            // be clear about whether it is creating or updating, reuse of ClaimPermission ids
            // across scenarios within a feature now becomes a problem.
            // We we have to use the technique in which ids specified in tests are actually
            // references to dynamically generated ids. Note that it's not the name passed into
            // this method, because that actually identifies a set of claims to be created. It's
            // the ids of the individual claims themselves (which are set as the Id in the table).
            foreach (ClaimPermissions cp in data)
            {
                string actualId = $"{cp.Id}-{Guid.NewGuid()}";
                this.claimPermissionIds[cp.Id] = actualId;
                cp.Id = actualId;
            }

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

        [Given(@"I have created the claim permissions called ""(.*)"" in the claim permissions store")]
        public async Task GivenIHaveCreatedTheClaimPermissionsCalledInTheClaimPermissionsStore(string claimPermissionsName)
        {
            // Note: lookup via this.claimPermissionIds not required here because the name here is
            // of the set of claims in the context, rather than the id itself. The mapping from id name
            // to generated id is done at the point where the table is converted into a List<ClaimPermissions>.
            List<ClaimPermissions> claimPermissions = this.scenarioContext.Get<List<ClaimPermissions>>(claimPermissionsName);
            IClaimPermissionsStore claimPermissionsStore = await this.GetClaimPermissionsStoreAsync().ConfigureAwait(false);

            IEnumerable<Task<ClaimPermissions>> tasks = claimPermissions.Select(claimPermissionsStore.CreateAsync);

            ClaimPermissions[] updatedClaimPermissions = await Task.WhenAll(tasks);

            // The returned items all have the ETag set, which we need in later tests if they go on
            // to modify the claim permissions further, because UpdateAsync refuses to run if you
            // give it a ClaimPermissions with a null ETag.
            this.scenarioContext.Set(updatedClaimPermissions.ToList(), claimPermissionsName);
        }

        [Given(@"an id exists named ""(.*)"" but there is no claims permission associated with it")]
        public void GivenAnIdExistsNamedButThereIsNoClaimsPermissionAssociatedWithIt(string idName)
        {
            string actualId = $"{idName}-{Guid.NewGuid()}";
            this.claimPermissionIds[idName] = actualId;
        }

        [When(@"I request the claim permission with Id ""(.*)"" from the claim permissions store")]
        public async Task WhenIRequestTheClaimPermissionWithIdFromTheClaimPermissionsStore(string claimIdName)
        {
            string claimId = this.claimPermissionIds[claimIdName];

            IClaimPermissionsStore store = await this.GetClaimPermissionsStoreAsync().ConfigureAwait(false);

            try
            {
                ClaimPermissions result = await store.GetAsync(claimId).ConfigureAwait(false);
                this.scenarioContext.Set(result, ClaimPermissionsResult);
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex);
            }
        }

        [When("I request a batch of claim permissions by Id from the claim permissions store")]
        public async Task WhenIRequestABatchOfClaimPermissionsByIdFromTheClaimPermissionsStore(Table table)
        {
            IEnumerable<string> claimIdNames = table.Rows.Select(x => x[0]);
            IEnumerable<string> claimIds = claimIdNames.Select(x => this.claimPermissionIds[x]);

            IClaimPermissionsStore store = await this.GetClaimPermissionsStoreAsync().ConfigureAwait(false);

            try
            {
                ClaimPermissionsCollection result = await store.GetBatchAsync(claimIds).ConfigureAwait(false);
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

        [Then("the claim permissions are returned")]
        public void ThenClaimPermissionsAreReturned()
        {
            if (!this.scenarioContext.TryGetValue(ClaimPermissionsResult, out ClaimPermissionsCollection results))
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

        [Then("the resource access rulesets on the claim permissions match the expected rulesets")]
        public void ThenTheResourceAccessRulesetsOnTheClaimPermissionsMatchTheExpectedRulesets(Table table)
        {
            ClaimPermissionsCollection loadedClaimPermissionsBatch = this.scenarioContext.Get<ClaimPermissionsCollection>(ClaimPermissionsResult);

            Assert.AreEqual(table.Rows.Count, loadedClaimPermissionsBatch.Permissions.Count, "The expected number of claim permissions were not loaded");

            foreach (TableRow currentRow in table.Rows)
            {
                // For each row in the "expected results", we need to:
                // - ensure we actually got the target ClaimPermissions back in the batch
                // - load the expected rulesets and then make sure the loaded claim permission matches.
                ClaimPermissions loadedClaimPermissions = loadedClaimPermissionsBatch.Permissions.Find(x => x.Id == this.claimPermissionIds[currentRow[0]]);
                Assert.NotNull(loadedClaimPermissions, $"The ClaimPermissions with Id '{currentRow[0]}' was not loaded");

                List<ResourceAccessRuleSet> expectedRulesets = this.scenarioContext.Get<List<ResourceAccessRuleSet>>(currentRow[1]);
                Assert.AreEqual(expectedRulesets.Count, loadedClaimPermissions.ResourceAccessRuleSets.Count, $"The loaded ClaimPermissions with Id '{loadedClaimPermissions.Id}' did not contain the expected number of ResourceAccessRulesets");

                foreach (ResourceAccessRuleSet currentRuleset in expectedRulesets)
                {
                    ResourceAccessRuleSet loadedRuleset = loadedClaimPermissions.ResourceAccessRuleSets.FirstOrDefault(x => x.Id == currentRuleset.Id);

                    Assert.NotNull(loadedRuleset, $"The loaded ClaimPermissions did not contain the expected ResourceAccessRuleset with Id '{currentRuleset.Id}'");
                    Assert.That(loadedRuleset.Rules, Is.EquivalentTo(currentRuleset.Rules), $"The loaded ResourceAccessRuleset with Id '{currentRuleset.Id}' was not equivalent to the expected ResourceAccessRuleset");
                }
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
            ITenant transientTenant = TransientTenantManager.GetInstance(this.featureContext).PrimaryTransientClient;
            IPermissionsStoreFactory permissionsStoreFactory = this.serviceProvider.GetRequiredService<IPermissionsStoreFactory>();
            return permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(transientTenant);
        }

        private Task<IClaimPermissionsStore> GetClaimPermissionsStoreAsync()
        {
            ITenant transientTenant = TransientTenantManager.GetInstance(this.featureContext).PrimaryTransientClient;
            IPermissionsStoreFactory permissionsStoreFactory = this.serviceProvider.GetRequiredService<IPermissionsStoreFactory>();
            return permissionsStoreFactory.GetClaimPermissionsStoreAsync(transientTenant);
        }
    }
}
