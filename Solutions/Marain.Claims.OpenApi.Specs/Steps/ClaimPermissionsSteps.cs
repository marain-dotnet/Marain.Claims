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
        private readonly Dictionary<string, string> claimIds = new Dictionary<string, string>();

        private ClaimPermissions newClaimPermissions;

        private int statusCodeFromClaimsService;
        private ClaimPermissions claimPermissionsFromClaimsService;

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

        [Given(@"these rules are POSTed to the updateClaimPermissionsResourceAccessRules endpoint for the ClaimsPermission with id named '(.*)'")]
        public async Task GivenTheseRulesArePOSTedToTheUpdateClaimPermissionsResourceAccessRulesEndpointForTheClaimsPermissionWithIdNamedAsync(
            string claimIdName, Table table)
        {
            string claimId = this.claimIds[claimIdName];
            await this.serviceWrapper.AddRulesForClaimPermissionsAsync(claimId, ParseRulesTable(table));
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
                        r.AccessType == expectedRule.AccessType),
                    $"Did not find rule with AccessType {expectedRule.AccessType}, resource '{expectedRule.Resource.Uri}' ('{expectedRule.Resource.DisplayName}'), Permission '{expectedRule.Permission}'");
            }
        }
    }
}
