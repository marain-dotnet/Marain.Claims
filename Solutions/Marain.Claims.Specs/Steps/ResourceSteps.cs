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
    public class ResourceSteps
    {
        private const string Resource1Key = "Resource1";
        private const string Resource2Key = "Resource2";
        private const string ResultKey = "Result";

        private readonly ScenarioContext scenarioContext;

        public ResourceSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("two resources have identical properties")]
        public void GivenTwoResourcesHaveIdenticalProperties()
        {
            var resource1 = new Resource(new Uri("foo/bar/*", UriKind.Relative), "Foo bar");
            var resource2 = new Resource(new Uri("foo/bar/*", UriKind.Relative), "Foo bar");

           this.scenarioContext.Set(resource1, Resource1Key);
           this.scenarioContext.Set(resource2, Resource2Key);
        }

        [Given("two resources have differing names")]
        public void GivenTwoResourcesHaveDifferingNames()
        {
            var resource1 = new Resource(new Uri("foo/bar", UriKind.Relative), "Foo bar");
            var resource2 = new Resource(new Uri("foo/bar/*", UriKind.Relative), "Foo bar");

           this.scenarioContext.Set(resource1, Resource1Key);
           this.scenarioContext.Set(resource2, Resource2Key);
        }

        [Given("two resources have differing display names")]
        public void GivenTwoResourcesHaveDifferingDisplayNames()
        {
            var resource1 = new Resource(new Uri("foo/bar/*", UriKind.Relative), "Foo bar");
            var resource2 = new Resource(new Uri("foo/bar/*", UriKind.Relative), "Bar foo");

           this.scenarioContext.Set(resource1, Resource1Key);
           this.scenarioContext.Set(resource2, Resource2Key);
        }

        [When("the resources are compared")]
        public void WhenTheResourcesAreCompared()
        {
            Resource resource1 = this.scenarioContext.Get<Resource>(Resource1Key);
            Resource resource2 = this.scenarioContext.Get<Resource>(Resource2Key);

            bool result = resource1 == resource2;

            this.scenarioContext.Set(result, ResultKey);
        }

        [Then("the resource comparison result should be (.*)")]
        public void ThenTheResourceComparisonResultShouldBe(bool expected)
        {
            bool result = this.scenarioContext.Get<bool>(ResultKey);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
