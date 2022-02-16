// <copyright file="OpenApiSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Steps
{
    using System;
    using Corvus.Testing.SpecFlow;
    using Marain.Claims.Client;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class OpenApiSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly IServiceProvider serviceProvider;

        public OpenApiSteps(
            ScenarioContext scenarioContext,
            FeatureContext featureContext)
        {
            this.scenarioContext = scenarioContext;
            this.serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
        }

        [When("I make a request to get the OpenAPI definition")]
        public async System.Threading.Tasks.Task WhenIMakeARequestToGetClaimPermissionsAsync()
        {
            IClaimsService claimsService = this.serviceProvider.GetRequiredService<IClaimsService>();

            try
            {
                object result = await claimsService.GetSwaggerAsync();
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex, "Exception");
            }
        }

        [Then("the request should succeed")]
        public void ThenTheRequestShouldSucceed()
        {
            bool hasException = this.scenarioContext.TryGetValue("Exception", out Exception exception);

            Assert.IsFalse(hasException, exception?.ToString());
        }
    }
}