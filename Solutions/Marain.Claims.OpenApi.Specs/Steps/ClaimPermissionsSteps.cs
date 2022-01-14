// <copyright file="WorkflowClaimPermissionsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Corvus.Extensions;

    using Marain.Claims.OpenApi.Specs.MultiHost;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    [Binding]
    public class ClaimPermissionsSteps
    {
        private readonly ITestableClaimsService serviceWrapper;
        private readonly Dictionary<string, string> claimIds = new ();

        private ClaimPermissions newClaimPermissions;

        private int statusCodeFromClaimsService;
        private ClaimPermissions claimPermissionsFromClaimsService;
        private IList<ResourceAccessRule> effectiveRulesFromClaimsService;
        private PermissionResult individualPermissionFromClaimsService;
        private IList<ClaimPermissionsBatchResponseItem> permissionsBatchFromClaimsService;

        public ClaimPermissionsSteps(
            ITestableClaimsService serviceWrapper)
        {
            this.serviceWrapper = serviceWrapper;
        }

        [Given(@"a unique ClaimsPermission id named '(.*)'")]
        public void GivenAUniqueClaimsPermissionIdNamed(string claimIdName)
        {
            this.claimIds.Add(claimIdName, $"claimId-{Guid.NewGuid()}");
        }

        [Given(@"a new ClaimsPermission with id named '(.*)'")]
        public void GivenTheNewClaimsPermissionHasIdNamed(string claimIdName)
        {
            string claimId = this.claimIds[claimIdName];
            this.newClaimPermissions = new ClaimPermissions
            {
                Id = claimId,
            };
        }

        [Given(@"the new ClaimsPermission has these rules")]
        public void GivenTheNewClaimsPermissionHasTheseRules(Table table)
        {
            List<ResourceAccessRule> rules = ParseRulesTable(table);
            this.newClaimPermissions.ResourceAccessRules.AddRange(rules);
        }

        [Given(@"the new ClaimsPermission has these ruleset IDs")]
        public void GivenTheNewClaimsPermissionHasTheseRulesetIDs(Table table)
        {
            this.newClaimPermissions.ResourceAccessRuleSets = table
                .Rows
                .Select(r => new ResourceAccessRuleSet { Id = r["ID"] })
                .ToList();
        }

        [Given(@"the ClaimsPermission with id '(.*)' is updated via the updateClaimPermissionsResourceAccessRuleSets endpoint to add rulesets with these ids")]
        [When(@"the ClaimsPermission with id '(.*)' is updated via the updateClaimPermissionsResourceAccessRuleSets endpoint to add rulesets with these ids")]
        public async Task GivenTheClaimsPermissionWithIdIsUpdatedToAddRulesetsWithTheseIdsAsync(
            string claimIdName, Table table)
        {
            string claimId = this.claimIds[claimIdName];
            var ruleSetsToAdd = table
                .Rows
                .Select(r => new ResourceAccessRuleSet { Id = r["ID"] })
                .ToList();
            (this.statusCodeFromClaimsService, _) = await this.serviceWrapper.AddRuleSetsForClaimPermissionsAsync(claimId, ruleSetsToAdd);
        }

        [When(@"the ClaimsPermission with id '(.*)' is updated via the updateClaimPermissionsResourceAccessRuleSets endpoint to remove rulesets with these ids")]
        public async Task WhenTheClaimsPermissionWithIdIsUpdatedViaTheUpdateClaimPermissionsResourceAccessRuleSetsEndpointToRemoveRulesetsWithTheseIdsAsync(
            string claimIdName, Table table)
        {
            string claimId = this.claimIds[claimIdName];
            var ruleSetsToRemove = table
                .Rows
                .Select(r => new ResourceAccessRuleSet { Id = r["ID"] })
                .ToList();
            (this.statusCodeFromClaimsService, _) = await this.serviceWrapper.RemoveRuleSetsForClaimPermissionsAsync(claimId, ruleSetsToRemove);
        }

        [Given(@"the ClaimsPermission with id '(.*)' is updated via the updateClaimPermissionsResourceAccessRuleSets endpoint to remove rulesets with these ids")]
        public async Task GivenTheClaimsPermissionWithIdIsUpdatedViaTheUpdateClaimPermissionsResourceAccessRuleSetsEndpointToRemoveRulesetsWithTheseIdsAsync(
            string claimIdName, Table table)
        {
            string claimId = this.claimIds[claimIdName];
            var ruleSetToAdd = table
                .Rows
                .Select(r => new ResourceAccessRuleSet { Id = r["ID"] })
                .ToList();
            (this.statusCodeFromClaimsService, _) = await this.serviceWrapper.RemoveRuleSetsForClaimPermissionsAsync(claimId, ruleSetToAdd);
        }

        [Given(@"these ruleset IDs are POSTed to the setClaimPermissionsResourceAccessRuleSets endpoint for the ClaimsPermission with id named '(.*)'")]
        [When(@"these ruleset IDs are POSTed to the setClaimPermissionsResourceAccessRuleSets endpoint for the ClaimsPermission with id named '(.*)'")]
        public async Task GivenTheseRulesetIDsArePOSTedToTheSetClaimPermissionsResourceAccessRuleSetsEndpointForTheClaimsPermissionWithIdNamedAsync(
            string claimIdName, Table table)
        {
            string claimId = this.claimIds[claimIdName];
            var ruleSets = table
                .Rows
                .Select(r => new ResourceAccessRuleSet { Id = r["ID"] })
                .ToList();
            (this.statusCodeFromClaimsService, _) = await this.serviceWrapper.SetRuleSetsForClaimPermissionsAsync(claimId, ruleSets);
        }

        [Given(@"an existing ruleset with id '(.*)' named '(.*)' and these rules")]
        public async Task GivenAnExistingRuleSetWithIdAndTheseRulesAsync(string ruleSetId, string ruleSetName, Table table)
        {
            var ruleSet = new ResourceAccessRuleSet
            {
                Id = ruleSetId,
                DisplayName = ruleSetName,
                Rules = ParseRulesTable(table),
            };
            await this.serviceWrapper.CreateResourceAccessRuleSetAsync(ruleSet);
        }

        [Given(@"these rules are added to the existing ruleset with id '(.*)'")]
        public async Task GivenTheseRulesAreAddedToTheExistingRulesetWithIdAsync(string ruleSetId, Table table)
        {
            await this.serviceWrapper.AddRulesToResourceAccessRuleSetAsync(
                ruleSetId,
                ParseRulesTable(table));
        }

        [Given(@"these rules are added via the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named '(.*)'")]
        [When(@"these rules are added via the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named '(.*)'")]
        public async Task GivenTheseRulesArePOSTedToTheUpdateClaimPermissionsResourceAccessRulesEndpointForTheClaimsPermissionWithIdNamedAsync(
            string claimIdName, Table table)
        {
            string claimId = this.claimIds[claimIdName];
            (this.statusCodeFromClaimsService, _) = await this.serviceWrapper.AddRulesForClaimPermissionsAsync(claimId, ParseRulesTable(table));
        }

        [Given(@"these rules are removed via the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named '(.*)'")]
        [When(@"these rules are removed via the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named '(.*)'")]
        public async Task GivenTheseRulesAreRemovedViaTheUpdateClaimPermissionsResourceAccessRulesEndpointForTheClaimsPermissionWithIdNamedAsync(
            string claimIdName, Table table)
        {
            string claimId = this.claimIds[claimIdName];
            (this.statusCodeFromClaimsService, _) = await this.serviceWrapper.RemoveRulesForClaimPermissionsAsync(claimId, ParseRulesTable(table));
        }

        [Given(@"these rules are POSTed to the setClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named '(.*)'")]
        public async Task GivenTheseRulesArePOSTedToTheSetClaimPermissionsResourceAccessRulesEndpointForTheClaimsPermissionWithIdNamedAsync(string claimIdName, Table table)
        {
            string claimId = this.claimIds[claimIdName];
            await this.serviceWrapper.SetRulesForClaimPermissionsAsync(claimId, ParseRulesTable(table));
        }

        [Given(@"the new ClaimsPermission is POSTed to the createClaimPermissions endpoint")]
        [When(@"the new ClaimsPermission is POSTed to the createClaimPermissions endpoint")]
        public async Task WhenTheNewClaimsPermissionIsPOSTedToTheCreateClaimPermissionsEndpointAsync()
        {
            (this.statusCodeFromClaimsService, this.claimPermissionsFromClaimsService) = await this.serviceWrapper.CreateClaimPermissionsAsync(this.newClaimPermissions);
        }

        [Then(@"the ClaimPermissions returned by the Claims service has exactly these defined rules")]
        public void ThenTheClaimPermissionsReturnedByTheClaimsServiceHasExactlyTheseDefinedRules(Table table)
        {
            IList<ResourceAccessRule> rulesToCheck = this.claimPermissionsFromClaimsService.ResourceAccessRules;
            CheckRulesMatch(table, rulesToCheck);
        }

        [Then(@"the ClaimPermissions returned by the Claims service has no rulesets")]
        public void ThenTheClaimPermissionsReturnedByTheClaimsServiceHasNoRulesets()
        {
            Assert.IsEmpty(this.claimPermissionsFromClaimsService.ResourceAccessRuleSets, "ResourceAccessRuleSets");
        }

        [Then(@"the ClaimPermissions returned by the Claims service has (.*) rulesets?")]
        public void ThenTheClaimPermissionsReturnedByTheClaimsServiceHasRuleset(int expectedRuleSetCount)
        {
            Assert.AreEqual(expectedRuleSetCount, this.claimPermissionsFromClaimsService.ResourceAccessRuleSets.Count);
        }

        [Then(@"the ClaimPermissions returned by the Claims service has a ruleset with id '(.*)' named '(.*)' with these rules")]
        public void ThenTheClaimPermissionsReturnedByTheClaimsServiceHasARuleSetWithIdNamedWithTheseRules(
            string ruleSetId, string ruleSetName, Table table)
        {
            ResourceAccessRuleSet matchingRuleSet = this.claimPermissionsFromClaimsService.ResourceAccessRuleSets
                .SingleOrDefault(rs => rs.Id == ruleSetId);
            Assert.IsNotNull(matchingRuleSet, $"No ruleset found with id {ruleSetId}");
            Assert.AreEqual(ruleSetName, matchingRuleSet.DisplayName);

            CheckRulesMatch(table, matchingRuleSet.Rules);
        }

        [Then(@"the ClaimPermissions returned by the Claims service has exactly these effective rules")]
        public void ThenTheClaimPermissionsReturnedByTheClaimsServiceHasExactlyTheseEffectiveRules(Table table)
        {
            IList<ResourceAccessRule> rulesToCheck = this.claimPermissionsFromClaimsService.AllResourceAccessRules;
            CheckRulesMatch(table, rulesToCheck);
        }

        [Given(@"ClaimsPermission with id named '(.*)' is fetched from the getClaimPermissions endpoint")]
        [When(@"ClaimsPermission with id named '(.*)' is fetched from the getClaimPermissions endpoint")]
        public async Task WhenClaimsPermissionWithIdNamedIsFetchedFromTheGetClaimPermissionsEndpoint(string claimIdName)
        {
            string claimId = this.claimIds[claimIdName];
            (this.statusCodeFromClaimsService, this.claimPermissionsFromClaimsService) = await this.serviceWrapper.GetClaimPermissionsAsync(claimId);
        }

        [When(@"the effective rules for ClaimsPermission with id named '(.*)' are fetched via the getClaimPermissionsResourceAccessRules endpoint")]
        public async Task WhenTheEffectiveRulesForClaimsPermissionWithIdNamedAreFetchedViaTheGetClaimPermissionsResourceAccessRulesEndpointAsync(
            string claimIdName)
        {
            string claimId = this.claimIds[claimIdName];
            (this.statusCodeFromClaimsService, this.effectiveRulesFromClaimsService) =
                await this.serviceWrapper.GetEffectiveRulesForClaimPermissionsAsync(claimId);
        }

        [When(@"permissions are evaluated via the getClaimPermissionsPermission endpoint ClaimsPermission id '(.*)', resource '(.*)' and access type '(.*)'")]
        public async Task WhenPermissionsAreEvaluatedViaTheGetClaimPermissionsPermissionEndpointClaimsPermissionIdResourceAndAccessTypeAsync(
            string claimIdName, string resourceUri, string accessType)
        {
            string claimId = this.claimIds[claimIdName];
            (this.statusCodeFromClaimsService, this.individualPermissionFromClaimsService) =
                await this.serviceWrapper.EvaluateSinglePermissionForClaimPermissionsAsync(claimId, resourceUri, accessType);
        }

        [When(@"these permissions are evaluated via the getClaimPermissionsPermissionBatch endpoint")]
        public async Task WhenThesePermissionsAreEvaluatedViaTheGetClaimPermissionsPermissionBatchEndpointAsync(Table table)
        {
            var requestItems = new List<ClaimPermissionsBatchRequestItem>();
            foreach (TableRow row in table.Rows)
            {
                string claimsPermissionsIdName = row["ClaimsPermissionsId"];
                string claimPermissionsId = this.claimIds[claimsPermissionsIdName];
                string resourceUri = row["ResourceUri"];
                string accessType = row["AccessType"];

                requestItems.Add(new ClaimPermissionsBatchRequestItem(
                    claimPermissionsId,
                    resourceUri,
                    accessType));
            }

            (this.statusCodeFromClaimsService, this.permissionsBatchFromClaimsService) =
                await this.serviceWrapper.BatchEvaluatePermissionsForClaimPermissionsAsync(requestItems);
        }

        [Then(@"the effective rules returns by the Claims service are")]
        public void ThenTheEffectiveRulesReturnsByTheClaimsServiceAre(Table table)
        {
            CheckRulesMatch(table, this.effectiveRulesFromClaimsService);
        }

        [Then(@"the permission returned by the Claims service is '(.*)'")]
        public void ThenThePermissionReturnedByTheClaimsServiceIs(Permission permission)
        {
            Assert.AreEqual(permission, this.individualPermissionFromClaimsService?.Permission);
        }

        [Then(@"the permissions batch response items are")]
        public void ThenThePermissionsBatchResponseItemsAre(Table table)
        {
            Assert.AreEqual(table.RowCount, this.permissionsBatchFromClaimsService.Count);

            for (int i = 0; i < table.RowCount; ++i)
            {
                TableRow row = table.Rows[i];
                ClaimPermissionsBatchResponseItem responseItem = this.permissionsBatchFromClaimsService[i];

                string claimsPermissionIdName = row["ClaimsPermissionId"];
                string claimPermissionsId = this.claimIds[claimsPermissionIdName];
                string resourceUri = row["ResourceUri"];
                string accessType = row["AccessType"];
                int responseCode = int.Parse(row["ResponseCode"]);
                string permission = row["Permission"];

                Assert.AreEqual(claimPermissionsId, responseItem.ClaimPermissionsId, $"ClaimPermissionsId[{i}]");
                Assert.AreEqual(resourceUri, responseItem.ResourceUri, $"ResourceUri[{i}]");
                Assert.AreEqual(accessType, responseItem.ResourceAccessType, $"ResourceAccessType[{i}]");
                Assert.AreEqual(responseCode, responseItem.ResponseCode, $"ResponseCode[{i}]");
                Assert.AreEqual(permission, responseItem.Permission, $"Permission[{i}]");
            }
        }

        [Then(@"the HTTP status returned by the Claims service is (.*)")]
        public void ThenTheHTTPStatusReturnedByTheClaimsServiceIs(int expectedStatus)
        {
            Assert.AreEqual(expectedStatus, this.statusCodeFromClaimsService);
        }

        [Then(@"the ClaimPermissions returned by the Claims service's id matches '([^']*)'")]
        public void ThenTheClaimsPermissionsReturnedByTheClaimsServiceId(string claimIdName)
        {
            string expectedClaimId = this.claimIds[claimIdName];
            Assert.AreEqual(expectedClaimId, this.claimPermissionsFromClaimsService.Id);
        }

        private static List<ResourceAccessRule> ParseRulesTable(Table table)
        {
            var rules = new List<ResourceAccessRule>();
            foreach (TableRow row in table.Rows)
            {
                string accessType = row["AccessType"];
                string resourceUri = row["ResourceUri"];
                string resourceName = row["ResourceName"];
                Permission permission = Enum.Parse<Permission>(row["Permission"]);

                rules.Add(new ResourceAccessRule(
                    accessType,
                    new Resource(new Uri(resourceUri, UriKind.RelativeOrAbsolute), resourceName),
                    permission));
            }

            return rules;
        }

        private static void CheckRulesMatch(Table table, IList<ResourceAccessRule> rulesToCheck)
        {
            List<ResourceAccessRule> expectedRules = ParseRulesTable(table);
            Assert.AreEqual(expectedRules.Count, rulesToCheck.Count);
            foreach (ResourceAccessRule expectedRule in expectedRules)
            {
                Assert.IsTrue(
                    rulesToCheck.Any(r =>
                        r.AccessType == expectedRule.AccessType &&
                        r.Permission == expectedRule.Permission &&
                        r.Resource.Uri == expectedRule.Resource.Uri &&
                        r.Resource.DisplayName == expectedRule.Resource.DisplayName),
                    $"Did not find rule with AccessType {expectedRule.AccessType}, resource '{expectedRule.Resource.Uri}' ('{expectedRule.Resource.DisplayName}'), Permission '{expectedRule.Permission}'");
            }
        }
    }
}
