// <copyright file="TransientTenantBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;

    using Corvus.Storage.Azure.BlobStorage;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;

    using Marain.Claims.Specs;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using TechTalk.SpecFlow;

    /// <summary>
    /// Bindings to manage creation and deletion of tenants for test features.
    /// </summary>
    [Binding]
    public static class TransientTenantBindings
    {
        /// <summary>
        /// Creates a new <see cref="ITenant"/> for the current feature, adding a test <see cref="BlobContainerConfiguration"/>
        /// to the tenant data.
        /// </summary>
        /// <param name="featureContext">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useTransientTenant", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task SetupTransientTenant(FeatureContext featureContext)
        {
            ClaimsServiceTestTenants testContext = await ClaimsTestTenantSetup.CreateTestTenantAndEnrollInClaimsAsync(featureContext).ConfigureAwait(false);

            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
            ITenant transientClientTenant = await tenantProvider.GetTenantAsync(testContext.TransientClientTenantId).ConfigureAwait(false);
            featureContext.Set(transientClientTenant);
        }

        /// <summary>
        /// Tear down the tenanted Cloud Blob Container for the feature.
        /// </summary>
        /// <param name="context">The feature context.</param>
        /// <returns>A <see cref="Task"/> which completes once the operation has completed.</returns>
        [AfterFeature("@useTransientTenant", Order = 100000)]
        public static async Task TearDownBlobContainers(FeatureContext context)
        {
            await ClaimsTestStorageSetup.TearDownBlobContainersAsync(context);
        }
    }
}