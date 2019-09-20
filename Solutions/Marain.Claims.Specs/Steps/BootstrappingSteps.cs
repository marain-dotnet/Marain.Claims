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
    using Marain.Claims.SpecFlow.Bindings;
    using Marain.Claims.Storage;
    using Menes;
    using Corvus.Tenancy;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;
    using Microsoft.Azure.Storage.Blob;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Extensions.Json;
    using Microsoft.Extensions.DependencyInjection;
    using Marain.Claims.OpenApi;

    [Binding]
    public class BootstrappingSteps
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;
        private readonly bool useRealDb;
        private readonly ITenant tenant;

        private readonly Mock<IClaimPermissionsStore> permissionStoreMock = new Mock<IClaimPermissionsStore>();
        private readonly Mock<IResourceAccessRuleSetStore> resourceAccessRuleSetStoreMock = new Mock<IResourceAccessRuleSetStore>();
        private readonly List<ClaimPermissions> claimPermissionsPersistedToStore = new List<ClaimPermissions>();
        private readonly List<ResourceAccessRuleSet> resourceAccessRulesPersistedToStore = new List<ResourceAccessRuleSet>();
        private readonly IClaimPermissionsStore claimPermissionsStore;
        private readonly IResourceAccessRuleSetStore ruleSetStore;
        private readonly IOpenApiContext openApiContext;
        private ClaimPermissionsService service;
        private OpenApiResult bootstrapTenantResult;

        public BootstrappingSteps(
            FeatureContext featureContext,
            ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;

            this.tenant = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<ITenantProvider>().Root;

            string[] tags = this.scenarioContext.ScenarioInfo.Tags;
#pragma warning disable RCS1104 // I disagree with Roslynators contention that what it wants do to would "Simplify" this.
            this.useRealDb = tags.Contains("realStore")
                ? true
                : (tags.Contains("inMemoryStore") ? false : throw new InvalidOperationException("You must tag this test with either @realStore or @inMemoryStore"));
#pragma warning restore RCS1104 // Simplify conditional expression.

            if (this.useRealDb)
            {
                IJsonSerializerSettingsProvider serializerSettingsProvider = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
                CloudBlobContainer claimsPermissionsContainer = this.featureContext.Get<CloudBlobContainer>(ClaimsTenantedCloudBlobContainerBindings.ClaimsPermissionsContainer);
                CloudBlobContainer ruleSetsContainer = this.featureContext.Get<CloudBlobContainer>(ClaimsTenantedCloudBlobContainerBindings.RuleSetsContainer);
                this.ruleSetStore = new ResourceAccessRuleSetStore(ruleSetsContainer, serializerSettingsProvider);
                this.claimPermissionsStore = new ClaimPermissionsStore(claimsPermissionsContainer, this.ruleSetStore, serializerSettingsProvider);
            }
            else
            {
                this.permissionStoreMock
                    .Setup(m => m.PersistAsync(It.IsAny<ClaimPermissions>()))
                    .Returns((ClaimPermissions cp) =>
                    {
                        this.claimPermissionsPersistedToStore.Add(cp);
                        return Task.FromResult(cp);
                    });
                this.resourceAccessRuleSetStoreMock
                    .Setup(m => m.PersistAsync(It.IsAny<ResourceAccessRuleSet>()))
                    .Returns((ResourceAccessRuleSet rs) =>
                    {
                        this.resourceAccessRulesPersistedToStore.Add(rs);
                        return Task.FromResult(rs);
                    });

                this.claimPermissionsStore = this.permissionStoreMock.Object;
                this.ruleSetStore = this.resourceAccessRuleSetStoreMock.Object;
            }

            var openApiContextMock = new Mock<IOpenApiContext>();
            openApiContextMock
                .SetupGet(m => m.CurrentTenantId)
                .Returns(this.tenant.Id);
            this.openApiContext = openApiContextMock.Object;
        }

        [Given("the tenant is uninitialised")]
        public async Task GivenTheTenantIsUninitialisedAsync()
        {
            if (this.useRealDb)
            {
                await this.DeleteAllPermissions();
                await this.DeleteAllRuleSets().ConfigureAwait(false);

                bool isInitialized = await this.claimPermissionsStore.AnyPermissions().ConfigureAwait(false);
                Assert.IsFalse(isInitialized, "Test tried to clear out all permissions, but store is reporting that it is initialized");
            }
            else
            {
                this.permissionStoreMock
                    .Setup(m => m.AnyPermissions())
                    .ReturnsAsync(false);
            }
        }

        [Given("the tenant is initialised")]
        public void GivenTheTenantIsInitialised()
        {
            Assert.IsFalse(this.useRealDb, "Tests using this step can only use @inMemoryStore");
            this.permissionStoreMock
                .Setup(m => m.AnyPermissions())
                .ReturnsAsync(true);
        }

        [When("I initialise the tenant with the role id '(.*)'")]
        public async Task WhenIInitialiseTheTenantWithTheRoleId(
            string roleId)
        {
            var storeFactory = new Mock<IPermissionsStoreFactory>();
            storeFactory
                .Setup(m => m.GetClaimPermissionsStoreAsync(this.tenant))
                .ReturnsAsync(this.claimPermissionsStore);
            storeFactory
                .Setup(m => m.GetResourceAccessRuleSetStoreAsync(this.tenant))
                .ReturnsAsync(this.ruleSetStore);
            this.service = new ClaimPermissionsService(
                storeFactory.Object,
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<ITenantProvider>(),
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>(),
                new Microsoft.ApplicationInsights.TelemetryClient());

            var openApiContext = new Mock<IOpenApiContext>();
            openApiContext
                .SetupGet(m => m.CurrentTenantId)
                .Returns(this.tenant.Id);
            var body = new JObject
            {
                ["administratorRoleClaimValue"] = roleId
            };
            this.bootstrapTenantResult = await this.service.BootstrapTenantAsync(
                openApiContext.Object,
                body).ConfigureAwait(false);
        }

        [Then("the tenant is initialised")]
        public async Task ThenTheTenantIsInitialised()
        {
            if (this.useRealDb)
            {
                bool result = await this.claimPermissionsStore.AnyPermissions().ConfigureAwait(false);
                Assert.IsTrue(result);
            }
        }

        [Then("the service creates an access rule set with id '(.*)' with displayname '(.*)'")]
        public void ThenTheServiceCreatesAnAccessRuleSetWithIdWithDisplayname(
            string ruleSetId,
            string ruleSetDisplayName)
        {
            ResourceAccessRuleSet ruleSet = this.resourceAccessRulesPersistedToStore.Single();
            Assert.AreEqual(ruleSetId, ruleSet.Id, nameof(ruleSet.Id));
            Assert.AreEqual(ruleSetDisplayName, ruleSet.DisplayName, nameof(ruleSet.DisplayName));
        }

        [Then("the access rule set created has the following rules")]
        public void ThenTheAccessRuleSetHasTheFollowingRules(Table table)
        {
            IList<ResourceAccessRule> rules = this.resourceAccessRulesPersistedToStore.Single().Rules;
            foreach (TableRow row in table.Rows)
            {
                string resourceUri = row["resourceUri"];
                string resourceDisplayName = row["resourceDisplayName"];
                string accessType = row["accessType"];
                var permission = (Permission)Enum.Parse(typeof(Permission), row["permission"]);

                ResourceAccessRule? ruleOrNull = rules
                    .Where(r => r.Resource.Uri.ToString() == resourceUri && r.AccessType == accessType)
                    .Select(r => (ResourceAccessRule?)r)
                    .SingleOrDefault();
                Assert.IsNotNull(ruleOrNull, $"Didn't find rule with resourceUri '{resourceUri}' and accessType '{accessType}'");
                ResourceAccessRule rule = ruleOrNull.Value;

                Assert.AreEqual(resourceDisplayName, rule.Resource.DisplayName, nameof(rule.Resource.DisplayName));
                Assert.AreEqual(permission, rule.Permission, nameof(rule.Permission));
            }
        }

        [Then("the service creates a claims permission with id '(.*)' with empty resourceAccessRules and a single resourceAccessRuleSet '(.*)'")]
        public void ThenTheServiceCreatesAClaimsPermissionWithIdWithEmptyResourceAccessRulesAndASingleResourceAccessRuleSet(
            string claimPermissionsId,
            string ruleSetId)
        {
            ClaimPermissions permissions = this.claimPermissionsPersistedToStore.Single();
            Assert.AreEqual(claimPermissionsId, permissions.Id, nameof(permissions.Id));
            Assert.IsEmpty(permissions.ResourceAccessRules, nameof(permissions.ResourceAccessRules));
            Assert.AreEqual(ruleSetId, permissions.ResourceAccessRuleSets.Single().Id, "Ruleset Id");
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to create a claim permissions")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToCreateAClaimsPermissionAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/claimPermissions", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to read a claim permissions")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToReadAClaimsPermissionAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/claimPermissions/3223", "GET", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to read all effective rules for a claims permission")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToReadAllRulesFromAClaimsPermissionAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/claimPermissions/3223/allResourceAccessRules", "GET", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to add a rule to a claim permissions")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToAddARuleToAClaimsPermissionAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/claimPermissions/123/resourceAccessRules", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to set all rules in a claim permissions")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToSetAllRulesInAClaimsPermissionAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/claimPermissions/123/resourceAccessRules", "PUT", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to add a resource access rule set to the claim permissions")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToAddAResourceAccessRuleSetToTheClaimsPermissionAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/claimPermissions/432/resourceAccessRuleSets", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to set all resource access rule sets in a claim permissions")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToSetAllResourceAccessRuleSetToTheClaimsPermissionAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/claimPermissions/432/resourceAccessRuleSets", "PUT", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to create a resource access rule set")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToCreateAResourceAccessRuleSetAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/resourceAccessRuleSet", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to read a resource access rule set")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToReadAResourceAccessRuleSetAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/resourceAccessRuleSet/3233", "GET", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to add an access rule to the resource access rule set")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToAddAnAccessRuleToTheResourceAccessRuleSetAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/resourceAccessRuleSet/abc/resourceAccessRules", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal in the '(.*)' role gets '(.*)' trying to set all access rules in a resource access rule set")]
        public async Task ThenAPrincipalInTheRoleGetsTryingToSetAllAccessRulesInAResourceAccessRuleSetAsync(
            string roleId,
            string permission)
        {
            await this.CheckPermissions(roleId, "f26450ab1668784bb327951c8b08f347/marain/claims/api/resourceAccessRuleSet/abc/resourceAccessRules", "PUT", permission).ConfigureAwait(false);
        }

        [Then("I am told that the tenant is already is initialised")]
        public void ThenIAmToldThatTheTenantIsAlreadyIsInitialised()
        {
            Assert.AreEqual(400, this.bootstrapTenantResult.StatusCode);
            var response = (JObject)this.bootstrapTenantResult.Results["application/json"];
            Assert.AreEqual("Tenant already initialized", response["detail"].Value<string>());
        }

        [Then("no access rules sets are created")]
        public void ThenNoAccessRulesSetsAreCreated()
        {
            this.resourceAccessRuleSetStoreMock.Verify(
                m => m.PersistAsync(It.IsAny<ResourceAccessRuleSet>()),
                Times.Never);
        }

        [Then("no claim permissions are created")]
        public void ThenNoClaimPermissionsAreCreated()
        {
            this.permissionStoreMock.Verify(
                m => m.PersistAsync(It.IsAny<ClaimPermissions>()),
                Times.Never);
        }

        private Task DeleteAllPermissions()
        {

            CloudBlobContainer container = this.featureContext.Get<CloudBlobContainer>(ClaimsTenantedCloudBlobContainerBindings.ClaimsPermissionsContainer);
            return this.DeleteAllDocuments(container);
        }

        private Task DeleteAllRuleSets()
        {

            CloudBlobContainer container = this.featureContext.Get<CloudBlobContainer>(ClaimsTenantedCloudBlobContainerBindings.RuleSetsContainer);
            return this.DeleteAllDocuments(container);
        }

        private async Task DeleteAllDocuments(CloudBlobContainer container)
        {
            foreach (CloudBlockBlob blob in container.ListBlobs(null, true))
            {
                await blob.DeleteAsync().ConfigureAwait(false);
            }
        }

        private async Task CheckPermissions(string roleId, string uri, string method, string permission)
        {
            OpenApiResult permissionsResult = await this.service.GetClaimPermissionsPermissionAsync(
                this.openApiContext,
                roleId,
                uri,
                method).ConfigureAwait(false);

            var expectedPermission = (Permission)Enum.Parse(typeof(Permission), permission);
            if (expectedPermission == Permission.Allow)
            {
                Assert.AreEqual(200, permissionsResult.StatusCode);
                var body = (PermissionResult)permissionsResult.Results["application/json"];
                Assert.AreEqual(expectedPermission, body.Permission);
            }
            else
            {
                Assert.AreEqual(404, permissionsResult.StatusCode);
            }
        }
    }
}
