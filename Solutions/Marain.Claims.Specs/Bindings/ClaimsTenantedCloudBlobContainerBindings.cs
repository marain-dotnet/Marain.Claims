// <copyright file="ClaimsCosmosDbBindings.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.SpecFlow.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Specflow bindings to support a tenanted cloud blob container.
    /// </summary>
    [Binding]
    public static class ClaimsTenantedCloudBlobContainerBindings
    {
        /// <summary>
        /// The key for the claims permissions container in the feature context
        /// </summary>
        public const string ClaimsPermissionsContainer = "ClaimsPermissionsContainer";

        /// <summary>
        /// The key for the rule sets container in the feature context
        /// </summary>
        public const string RuleSetsContainer = "RuleSetsContainer";

        /// <summary>
        /// Set up a tenanted Cloud Blob Container for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <remarks>Note that this sets up a resource in Azure and will incur cost. Ensure the corresponding tear down operation is always run, or verify manually after a test run.</remarks>
        [BeforeFeature("@setupTenantedCloudBlobContainer", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task SetupCloudBlobContainerForRootTenant(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantCloudBlobContainerFactory factory = serviceProvider.GetRequiredService<ITenantCloudBlobContainerFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            string containerBase = Guid.NewGuid().ToString();

            CloudBlobContainer claimsPermissionsContainer = await factory.GetBlobContainerForTenantAsync(
                tenantProvider.Root,
                new BlobStorageContainerDefinition($"{containerBase}claims"));

            featureContext.Set(claimsPermissionsContainer, ClaimsPermissionsContainer);

            CloudBlobContainer ruleSetContainer = await factory.GetBlobContainerForTenantAsync(
                tenantProvider.Root,
                new BlobStorageContainerDefinition($"{containerBase}rulesets"));

            featureContext.Set(ruleSetContainer, RuleSetsContainer);
        }

        /// <summary>
        /// Tear down the tenanted Cloud Blob Container for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <returns>A <see cref="Task"/> which completes once the operation has completed.</returns>
        [AfterFeature("@setupTenantedCloudBlobContainer", Order = 100000)]
        public static Task TearDownBlobContainers(FeatureContext featureContext)
        {
            return featureContext.RunAndStoreExceptionsAsync(
                async () =>
                {
                    await featureContext.Get<CloudBlobContainer>(ClaimsPermissionsContainer).DeleteAsync().ConfigureAwait(false);
                    await featureContext.Get<CloudBlobContainer>(RuleSetsContainer).DeleteAsync().ConfigureAwait(false);
                });
        }
    }
}
