// <copyright file="RepositorySteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Elements should be documented

namespace Marain.Claims.SpecFlow.Steps
{
    using NUnit.Framework;
    using System;
    using TechTalk.SpecFlow;

    [Binding]
    public class ResourceAccessRuleSteps
    {
        private const string ResourceAccessRuleKey = "ResourceAccessRule";
        private const string ResourceAccessRule1Key = "ResourceAccessRule1";
        private const string ResourceAccessRule2Key = "ResourceAccessRule2";
        private const string ResultKey = "Result";

        private readonly ScenarioContext scenarioContext;

        public ResourceAccessRuleSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("two resource access rules have identical properties")]
        public void GivenTwoResourceAccessRulesHaveIdenticalProperties()
        {
            var resourceAccessRule1 = new ResourceAccessRule("GET", new Resource(new Uri("foo", UriKind.Relative), "Foo"), Permission.Allow);
            var resourceAccessRule2 = new ResourceAccessRule("GET", new Resource(new Uri("foo", UriKind.Relative), "Foo"), Permission.Allow);

            this.scenarioContext.Set(resourceAccessRule1, ResourceAccessRule1Key);
            this.scenarioContext.Set(resourceAccessRule2, ResourceAccessRule2Key);
        }

        [Given("two resource access rules have differing access types")]
        public void GivenTwoResourceAccessRulesHaveDifferingAccessTypes()
        {
            var resourceAccessRule1 = new ResourceAccessRule("GET", new Resource(new Uri("foo", UriKind.Relative), "Foo"), Permission.Allow);
            var resourceAccessRule2 = new ResourceAccessRule("PUT", new Resource(new Uri("foo", UriKind.Relative), "Foo"), Permission.Allow);

            this.scenarioContext.Set(resourceAccessRule1, ResourceAccessRule1Key);
            this.scenarioContext.Set(resourceAccessRule2, ResourceAccessRule2Key);
        }

        [Given("two resource access rules have differing resources")]
        public void GivenTwoResourceAccessRulesHaveDifferingResources()
        {
            var resourceAccessRule1 = new ResourceAccessRule("GET", new Resource(new Uri("foo", UriKind.Relative), "Foo"), Permission.Allow);
            var resourceAccessRule2 = new ResourceAccessRule("GET", new Resource(new Uri("bar", UriKind.Relative), "Bar"), Permission.Allow);

            this.scenarioContext.Set(resourceAccessRule1, ResourceAccessRule1Key);
            this.scenarioContext.Set(resourceAccessRule2, ResourceAccessRule2Key);
        }

        [Given("two resource access rules have differing permissions")]
        public void GivenTwoResourceAccessRulesHaveDifferingPermissions()
        {
            var resourceAccessRule1 = new ResourceAccessRule("GET", new Resource(new Uri("foo", UriKind.Relative), "Foo"), Permission.Allow);
            var resourceAccessRule2 = new ResourceAccessRule("GET", new Resource(new Uri("foo", UriKind.Relative), "Foo"), Permission.Deny);

            this.scenarioContext.Set(resourceAccessRule1, ResourceAccessRule1Key);
            this.scenarioContext.Set(resourceAccessRule2, ResourceAccessRule2Key);
        }

        [When("the resource access rules are compared")]
        public void WhenTheResourceAccessRulesAreCompared()
        {
            ResourceAccessRule resourceAccessRule1 = this.scenarioContext.Get<ResourceAccessRule>(ResourceAccessRule1Key);
            ResourceAccessRule resourceAccessRule2 = this.scenarioContext.Get<ResourceAccessRule>(ResourceAccessRule2Key);

            bool result = resourceAccessRule1 == resourceAccessRule2;

            this.scenarioContext.Set(result, ResultKey);
        }

        [Then("the resource access rule comparison result should be (.*)")]
        public void ThenTheResourceAccessRuleComparisonResultShouldBe(bool expected)
        {
            bool result = this.scenarioContext.Get<bool>(ResultKey);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Given("I have a resource access rule for a resource with name '(.*)' and display name '(.*)', with an access type '(.*)', and permission '(.*)'")]
        public void GivenIHaveAResourceAccessRuleForAResourceWithNameAndDisplayNameWithAnAccessTypeAndPermission
            (string name, string displayName, string accessType, Permission permission)
        {
            var resourceAccessRule = new ResourceAccessRule(accessType, new Resource(new Uri(name, UriKind.Relative), displayName), permission);
            this.scenarioContext.Set(resourceAccessRule, ResourceAccessRuleKey);
        }

        [When("I check if the resource access rule is a match for a target with resource name '(.*)' and target '(.*)'")]
        public void WhenICheckIfTheResourceAccessRuleIsAMatchForATargetWithResourceNameAndTarget(string resourceName, string accessType)
        {
            ResourceAccessRule resourceAccessRule = this.scenarioContext.Get<ResourceAccessRule>(ResourceAccessRuleKey);
            bool result = resourceAccessRule.IsMatch(new Uri(resourceName, UriKind.Relative), accessType);
            this.scenarioContext.Set(result, ResultKey);
        }

        [Then("the match result should be '(.*)'")]
        public void ThenTheMatchResultShouldBe(bool expected)
        {
            bool result = this.scenarioContext.Get<bool>(ResultKey);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
