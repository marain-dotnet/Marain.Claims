// <copyright file="TransientTenantBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Bindings
{
    using System.Threading.Tasks;

    using BoDi;

    using Marain.Claims.Specs;

    using TechTalk.SpecFlow;

    [Binding]
    public static class TransientTenantBindings
    {
        [BeforeFeature("@useClaimsApi", Order = BindingSequence.TransientTenantSetup)]
        public static async Task SetupTransientTenant(FeatureContext featureContext, IObjectContainer objectContainer)
        {
            ClaimsServiceTestTenants testContext = await ClaimsTestTenantSetup.CreateTestTenantAndEnrollInClaimsAsync(featureContext).ConfigureAwait(false);
            objectContainer.RegisterInstanceAs(testContext);
        }

        /// <summary>
        /// Tear down the tenanted Cloud Blob Container for the feature.
        /// </summary>
        /// <param name="context">The feature context.</param>
        /// <returns>A <see cref="Task"/> which completes once the operation has completed.</returns>
        [AfterFeature("@useClaimsApi")]
        public static async Task TearDownBlobContainers(FeatureContext context)
        {
            await ClaimsTestStorageSetup.TearDownBlobContainersAsync(context);
        }
    }
}