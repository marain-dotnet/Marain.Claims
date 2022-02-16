// <copyright file="RepositorySteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.SpecFlow.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;

    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Marain.Claims;
    using Marain.Claims.OpenApi;
    using Marain.Claims.Storage;
    using Marain.Services.Tenancy;
    using Marain.TenantManagement.Testing;
    using Menes;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class BootstrappingSteps
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;
        private readonly TransientTenantManager transientTenantManager;
        private readonly bool useRealDb;

        private readonly Mock<IClaimPermissionsStore> permissionStoreMock = new();
        private readonly Mock<IResourceAccessRuleSetStore> resourceAccessRuleSetStoreMock = new();
        private readonly List<ClaimPermissions> claimPermissionsPersistedToStore = new();
        private readonly List<ResourceAccessRuleSet> resourceAccessRulesPersistedToStore = new();
        private readonly IOpenApiContext openApiContext;

        private ClaimPermissionsService service;
        private OpenApiResult bootstrapTenantResult;

        private IClaimPermissionsStore claimPermissionsStore;
        private IResourceAccessRuleSetStore ruleSetStore;
        private Mock<IPermissionsStoreFactory> permissionsStoreFactoryMock;

        public BootstrappingSteps(
            FeatureContext featureContext,
            ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;

            this.transientTenantManager = TransientTenantManager.GetInstance(featureContext);

            string[] tags = this.scenarioContext.ScenarioInfo.Tags;
#pragma warning disable IDE0075 // I disagree with VS's contention that what it wants do to would "Simplify" this.
#pragma warning disable IDE0079 // Avoid redundant suppression message for users who don't have Roslynator
#pragma warning disable RCS1104 // I disagree with Roslynators contention that what it wants do to would "Simplify" this.
            this.useRealDb = tags.Contains("realStore")
                ? true
                : (tags.Contains("inMemoryStore") ? false : throw new InvalidOperationException("You must tag this test with either @realStore or @inMemoryStore"));
#pragma warning restore RCS1104 // Simplify conditional expression.
#pragma warning restore IDE0079
#pragma warning restore IDE0075

            var openApiContextMock = new Mock<IOpenApiContext>();
            openApiContextMock
                .SetupGet(m => m.CurrentTenantId)
                .Returns(this.transientTenantManager.PrimaryTransientClient.Id);
            this.openApiContext = openApiContextMock.Object;
        }

        [Given("the tenant is uninitialised")]
        public async Task GivenTheTenantIsUninitialisedAsync()
        {
            this.permissionsStoreFactoryMock = await this.SetupMockPermissionsStoreFactoryAsync().ConfigureAwait(false);

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
        public async Task GivenTheTenantIsInitialisedAsync()
        {
            this.permissionsStoreFactoryMock = await this.SetupMockPermissionsStoreFactoryAsync().ConfigureAwait(false);

            Assert.IsFalse(this.useRealDb, "Tests using this step can only use @inMemoryStore");
            this.permissionStoreMock
                .Setup(m => m.AnyPermissions())
                .ReturnsAsync(true);
        }

        [When("I initialise the tenant with the object id '(.*)'")]
        public async Task WhenIInitialiseTheTenantWithTheObjectId(
            string objectId)
        {
            this.service = new ClaimPermissionsService(
                this.permissionsStoreFactoryMock.Object,
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IMarainServicesTenancy>(),
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>());

            var openApiContext = new Mock<IOpenApiContext>();
            openApiContext
                .SetupGet(m => m.CurrentTenantId)
                .Returns(this.transientTenantManager.PrimaryTransientClient.Id);
            var body = new JObject
            {
                ["administratorPrincipalObjectId"] = objectId,
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
                string resourceUri = row["resourceUri"].Replace("<tenantid>", this.transientTenantManager.PrimaryTransientClient.Id);
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

        [Then("a principal with oid '(.*)' gets '(.*)' trying to create a claim permissions")]
        public async Task ThenAPrincipalWithOidGetsTryingToCreateAClaimsPermissionAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/claimPermissions", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to read a claim permissions")]
        public async Task ThenAPrincipalWithOidGetsTryingToReadAClaimsPermissionAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/claimPermissions/3223", "GET", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to read all effective rules for a claims permission")]
        public async Task ThenAPrincipalWithOidGetsTryingToReadAllRulesFromAClaimsPermissionAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/claimPermissions/3223/allResourceAccessRules", "GET", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to add a rule to a claim permissions")]
        public async Task ThenAPrincipalWithOidGetsTryingToAddARuleToAClaimsPermissionAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/claimPermissions/123/resourceAccessRules", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to set all rules in a claim permissions")]
        public async Task ThenAPrincipalWithOidGetsTryingToSetAllRulesInAClaimsPermissionAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/claimPermissions/123/resourceAccessRules", "PUT", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to add a resource access rule set to the claim permissions")]
        public async Task ThenAPrincipalWithOidGetsTryingToAddAResourceAccessRuleSetToTheClaimsPermissionAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/claimPermissions/432/resourceAccessRuleSets", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to set all resource access rule sets in a claim permissions")]
        public async Task ThenAPrincipalWithOidGetsTryingToSetAllResourceAccessRuleSetToTheClaimsPermissionAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/claimPermissions/432/resourceAccessRuleSets", "PUT", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to create a resource access rule set")]
        public async Task ThenAPrincipalWithOidGetsTryingToCreateAResourceAccessRuleSetAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/resourceAccessRuleSet", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to read a resource access rule set")]
        public async Task ThenAPrincipalWithOidGetsTryingToReadAResourceAccessRuleSetAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/resourceAccessRuleSet/3233", "GET", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to add an access rule to the resource access rule set")]
        public async Task ThenAPrincipalWithOidGetsTryingToAddAnAccessRuleToTheResourceAccessRuleSetAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/resourceAccessRuleSet/abc/resourceAccessRules", "POST", permission).ConfigureAwait(false);
        }

        [Then("a principal with oid '(.*)' gets '(.*)' trying to set all access rules in a resource access rule set")]
        public async Task ThenAPrincipalWithOidGetsTryingToSetAllAccessRulesInAResourceAccessRuleSetAsync(
            string objectId,
            string permission)
        {
            await this.CheckPermissions(objectId, $"{this.transientTenantManager.PrimaryTransientClient.Id}/marain/claims/resourceAccessRuleSet/abc/resourceAccessRules", "PUT", permission).ConfigureAwait(false);
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
                m => m.CreateAsync(It.IsAny<ClaimPermissions>()),
                Times.Never);
        }

        private static async Task DeleteAllDocuments(BlobContainerClient container)
        {
            foreach (BlobItem blob in container.GetBlobs())
            {
                await container.DeleteBlobAsync(blob.Name).ConfigureAwait(false);
            }
        }

        private Task DeleteAllPermissions()
        {
            BlobContainerClient container = ((ClaimPermissionsStore)this.claimPermissionsStore).Container;
            return DeleteAllDocuments(container);
        }

        private Task DeleteAllRuleSets()
        {
            BlobContainerClient container = ((ResourceAccessRuleSetStore)this.ruleSetStore).Container;
            return DeleteAllDocuments(container);
        }

        private async Task CheckPermissions(string objectId, string uri, string method, string permission)
        {
            OpenApiResult permissionsResult = await this.service.GetClaimPermissionsPermissionAsync(
                this.openApiContext,
                objectId,
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

        private async Task<Mock<IPermissionsStoreFactory>> SetupMockPermissionsStoreFactoryAsync()
        {
            if (this.useRealDb)
            {
                IPermissionsStoreFactory permissionsStoreFactory = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IPermissionsStoreFactory>();
                this.ruleSetStore = await permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(this.transientTenantManager.PrimaryTransientClient).ConfigureAwait(false);
                this.claimPermissionsStore = await permissionsStoreFactory.GetClaimPermissionsStoreAsync(this.transientTenantManager.PrimaryTransientClient).ConfigureAwait(false);
            }
            else
            {
                this.permissionStoreMock
                    .Setup(m => m.CreateAsync(It.IsAny<ClaimPermissions>()))
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

            var storeFactory = new Mock<IPermissionsStoreFactory>();
            storeFactory
                .Setup(m => m.GetClaimPermissionsStoreAsync(It.Is<ITenant>(t => t.Id == this.transientTenantManager.PrimaryTransientClient.Id)))
                .ReturnsAsync(this.claimPermissionsStore);
            storeFactory
                .Setup(m => m.GetResourceAccessRuleSetStoreAsync(It.Is<ITenant>(t => t.Id == this.transientTenantManager.PrimaryTransientClient.Id)))
                .ReturnsAsync(this.ruleSetStore);
            return storeFactory;
        }
    }
}