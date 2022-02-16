// <copyright file="RepositorySteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.SpecFlow.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class ClaimPermissionsSteps
    {
        private const string ClaimPermissionsId = "test";

        private const string ClaimPermissionsKey = "Claim";
        private const string ResourceAccessRuleSetsKey = "ResourceAccessRuleSets";
        private const string ResultKey = "Result";

        private readonly ScenarioContext scenarioContext;

        private readonly List<ResourceAccessRule> directResourceAccessRules = new()
        {
            new ResourceAccessRule("GET", new Resource(new Uri("foo1", UriKind.Relative), "Foo1"), Permission.Allow),
            new ResourceAccessRule("GET", new Resource(new Uri("foo2", UriKind.Relative), "Foo2"), Permission.Allow),
            new ResourceAccessRule("POST", new Resource(new Uri("foo1", UriKind.Relative), "Foo1"), Permission.Deny),
        };

        private readonly List<ResourceAccessRule> directOverlappingResourceAccessRules = new()
        {
            new ResourceAccessRule("GET", new Resource(new Uri("foo1", UriKind.Relative), "Foo1"), Permission.Allow),
            new ResourceAccessRule("GET", new Resource(new Uri("foo2", UriKind.Relative), "Foo2"), Permission.Allow),
            new ResourceAccessRule("POST", new Resource(new Uri("foo1", UriKind.Relative), "Foo1"), Permission.Deny),
            new ResourceAccessRule("GET", new Resource(new Uri("foo4", UriKind.Relative), "Foo4"), Permission.Allow),
        };

        private readonly List<ResourceAccessRule> resourceAccessRuleSet1ResourceAccessRules = new()
        {
            new ResourceAccessRule("GET", new Resource(new Uri("foo3", UriKind.Relative), "Foo3"), Permission.Allow),
            new ResourceAccessRule("GET", new Resource(new Uri("foo4", UriKind.Relative), "Foo4"), Permission.Allow),
        };

        private readonly List<ResourceAccessRule> resourceAccessRuleSet2ResourceAccessRules = new()
        {
            new ResourceAccessRule("GET", new Resource(new Uri("foo5", UriKind.Relative), "Foo5"), Permission.Allow),
            new ResourceAccessRule("GET", new Resource(new Uri("foo6", UriKind.Relative), "Foo6"), Permission.Allow),
        };

        private readonly List<ResourceAccessRule> resourceAccessRuleSet3ResourceAccessRules = new()
        {
            new ResourceAccessRule("GET", new Resource(new Uri("foo4", UriKind.Relative), "Foo4"), Permission.Allow),
            new ResourceAccessRule("GET", new Resource(new Uri("foo5", UriKind.Relative), "Foo5"), Permission.Allow),
        };

        public ClaimPermissionsSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("a claims permission with only direct resource access rules")]
        public void GivenAClaimPermissionsWithOnlyDirectResourceAccessRules()
        {
            var claimPermissions = new ClaimPermissions
            {
                Id = ClaimPermissionsId,
                ResourceAccessRules = this.directResourceAccessRules,
            };

            this.scenarioContext.Set(claimPermissions, ClaimPermissionsKey);
        }

        [Given("two resource access rule sets with different resource access rules")]
        public void GivenTwoResourceAccessRuleSetsWithDifferentResourceAccessRules()
        {
            var resourceAccessRuleSets = new List<ResourceAccessRuleSet>
            {
                new ResourceAccessRuleSet
                {
                    DisplayName = "1",
                    Id = "1",
                    Rules = this.resourceAccessRuleSet1ResourceAccessRules,
                },
                new ResourceAccessRuleSet
                {
                    DisplayName = "2",
                    Id = "2",
                    Rules = this.resourceAccessRuleSet2ResourceAccessRules,
                },
            };

            this.scenarioContext.Set(resourceAccessRuleSets, ResourceAccessRuleSetsKey);
        }

        [Given("a claims permission with only resource access rule sets")]
        public void GivenAClaimPermissionsWithOnlyResourceAccessRuleSets()
        {
            List<ResourceAccessRuleSet> resourceAccessRuleSets = this.scenarioContext.Get<List<ResourceAccessRuleSet>>(ResourceAccessRuleSetsKey);

            var claimPermissions = new ClaimPermissions
            {
                Id = ClaimPermissionsId,
                ResourceAccessRuleSets = resourceAccessRuleSets,
            };

            this.scenarioContext.Set(claimPermissions, ClaimPermissionsKey);
        }

        [Given("two resource access rule sets with overlapping resource access rules")]
        public void GivenTwoResourceAccessRuleSetsWithOverlappingResourceAccessRules()
        {
            var resourceAccessRuleSets = new List<ResourceAccessRuleSet>
            {
                new ResourceAccessRuleSet
                {
                    DisplayName = "1",
                    Id = "1",
                    Rules = this.resourceAccessRuleSet1ResourceAccessRules,
                },
                new ResourceAccessRuleSet
                {
                    DisplayName = "3",
                    Id = "3",
                    Rules = this.resourceAccessRuleSet3ResourceAccessRules,
                },
            };

            this.scenarioContext.Set(resourceAccessRuleSets, ResourceAccessRuleSetsKey);
        }

        [Given("a claims permission with resource access rule sets and direct resource access rules")]
        public void GivenAClaimPermissionsWithResourceAccessRuleSetsAndDirectResourceAccessRules()
        {
            List<ResourceAccessRuleSet> resourceAccessRuleSets = this.scenarioContext.Get<List<ResourceAccessRuleSet>>(ResourceAccessRuleSetsKey);

            var claimPermissions = new ClaimPermissions
            {
                Id = ClaimPermissionsId,
                ResourceAccessRuleSets = resourceAccessRuleSets,
                ResourceAccessRules = this.directResourceAccessRules,
            };

            this.scenarioContext.Set(claimPermissions, ClaimPermissionsKey);
        }

        [Given("a claims permission with resource access rule sets and overlapping direct resource access rules")]
        public void GivenAClaimPermissionsWithResourceAccessRuleSetsAndOverlappingDirectResourceAccessRules()
        {
            List<ResourceAccessRuleSet> resourceAccessRuleSets = this.scenarioContext.Get<List<ResourceAccessRuleSet>>(ResourceAccessRuleSetsKey);

            var claimPermissions = new ClaimPermissions
            {
                Id = ClaimPermissionsId,
                ResourceAccessRuleSets = resourceAccessRuleSets,
                ResourceAccessRules = this.directOverlappingResourceAccessRules,
            };

            this.scenarioContext.Set(claimPermissions, ClaimPermissionsKey);
        }

        [When("I get all resource access rules for the claim permissions")]
        public void WhenIGetAllResourceAccessRulesForTheClaimPermissions()
        {
            ClaimPermissions claimPermissions = this.scenarioContext.Get<ClaimPermissions>(ClaimPermissionsKey);
            IList<ResourceAccessRule> result = claimPermissions.AllResourceAccessRules;

            this.scenarioContext.Set(result, ResultKey);
        }

        [Then("the result should contain all resource access rules that were directly assigned to the claim permissions")]
        public void ThenTheResultShouldContainAllResourceAccessRulesThatWereDirectlyAssignedToTheClaimPermissions()
        {
            List<ResourceAccessRule> result = this.scenarioContext.Get<List<ResourceAccessRule>>(ResultKey);

            Assert.Multiple(() =>
            {
                foreach (ResourceAccessRule resourceAccessRule in this.directResourceAccessRules)
                {
                    Assert.Contains(resourceAccessRule, result);
                }
            });
        }

        [Then("the result should contain all resource access rules that were in the resource access rule sets")]
        public void ThenTheResultShouldContainAllResourceAccessRulesThatWereInTheResourceAccessRuleSets()
        {
            List<ResourceAccessRule> result = this.scenarioContext.Get<List<ResourceAccessRule>>(ResultKey);
            IList<ResourceAccessRuleSet> resourceAccessRuleSets = this.scenarioContext.Get<IList<ResourceAccessRuleSet>>(ResourceAccessRuleSetsKey);

            Assert.Multiple(() =>
            {
                foreach (ResourceAccessRule resourceAccessRule in resourceAccessRuleSets.SelectMany(x => x.Rules))
                {
                    Assert.Contains(resourceAccessRule, result);
                }
            });
        }

        [Then("the result should not contain any duplicate resource access rules")]
        public void ThenTheResultShouldNotContainAnyDuplicateResourceAccessRules()
        {
            List<ResourceAccessRule> result = this.scenarioContext.Get<List<ResourceAccessRule>>(ResultKey);

            Assert.That(result, Is.Unique);
        }
    }
}