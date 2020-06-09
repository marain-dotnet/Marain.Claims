// <copyright file="TransientTenantBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Marain.Claims.Storage;
    using Marain.Services;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;
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
        /// Creates a new <see cref="ITenant"/> for the current feature, adding a test <see cref="BlobStorageConfiguration"/>
        /// to the tenant data.
        /// </summary>
        /// <param name="featureContext">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useTransientTenant", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task SetupTransientTenant(FeatureContext featureContext)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<ITenantProvider>();
            var transientTenantManager = TransientTenantManager.GetInstance(featureContext);
            await transientTenantManager.EnsureInitialised().ConfigureAwait(false);

            // Create a transient service tenant for testing purposes.
            ITenant transientServiceTenant = await transientTenantManager.CreateTransientServiceTenantFromEmbeddedResourceAsync(
                typeof(TransientTenantBindings).Assembly,
                "Marain.Claims.Specs.ServiceManifests.ClaimsServiceManifest.jsonc").ConfigureAwait(false);

            // Now update the service Id in our configuration
            UpdateServiceConfigurationWithTransientTenantId(featureContext, transientServiceTenant);

            // Now we need to construct a transient client tenant for the test, and enroll it in the new
            // transient service.
            ITenant transientClientTenant = await transientTenantManager.CreateTransientClientTenantAsync().ConfigureAwait(false);

            await transientTenantManager.AddEnrollmentAsync(
                transientClientTenant.Id,
                transientServiceTenant.Id,
                GetClaimsConfig(featureContext)).ConfigureAwait(false);

            // TODO: Temporary hack to work around the fact that the transient tenant manager no longer holds the latest
            // version of the tenants it's tracking; see https://github.com/marain-dotnet/Marain.TenantManagement/issues/28
            transientTenantManager.PrimaryTransientClient = await tenantProvider.GetTenantAsync(transientClientTenant.Id).ConfigureAwait(false);

            featureContext.Set(transientClientTenant);
        }

        /// <summary>
        /// Tear down the tenanted Cloud Blob Container for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <returns>A <see cref="Task"/> which completes once the operation has completed.</returns>
        [AfterFeature("@useTransientTenant", Order = 100000)]
        public static async Task TearDownBlobContainers(FeatureContext context)
        {
            ITenant transientTenant = TransientTenantManager.GetInstance(context).PrimaryTransientClient;
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(context);

            if (transientTenant != null && serviceProvider != null)
            {
                IPermissionsStoreFactory permissionsStoreFactory = serviceProvider.GetRequiredService<IPermissionsStoreFactory>();

                await context.RunAndStoreExceptionsAsync(async () =>
                {
                    var claimsStore = (ClaimPermissionsStore)await permissionsStoreFactory.GetClaimPermissionsStoreAsync(transientTenant);
                    await claimsStore.Container.DeleteAsync();
                }).ConfigureAwait(false);

                await context.RunAndStoreExceptionsAsync(async () =>
                {
                    var ruleSetsStore = (ResourceAccessRuleSetStore)await permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(transientTenant);
                    await ruleSetsStore.Container.DeleteAsync();
                }).ConfigureAwait(false);
            }
        }

        private static void UpdateServiceConfigurationWithTransientTenantId(
            FeatureContext featureContext,
            ITenant transientServiceTenant)
        {
            IConfiguration configuration = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<IConfiguration>();

            configuration["MarainServiceConfiguration:ServiceTenantId"] = transientServiceTenant.Id;
            configuration["MarainServiceConfiguration:ServiceDisplayName"] = transientServiceTenant.Name;
        }

        private static EnrollmentConfigurationItem[] GetClaimsConfig(FeatureContext featureContext)
        {
            IConfiguration configuration = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<IConfiguration>();

            // Load the config items we need:    
            BlobStorageConfiguration claimPermissionsStoreStorageConfiguration =
                configuration.GetSection("TestBlobStorageConfiguration").Get<BlobStorageConfiguration>()
                ?? new BlobStorageConfiguration();

            claimPermissionsStoreStorageConfiguration.Container = "claimpermissions";

            BlobStorageConfiguration resourceAccessRuleSetsStoreStorageConfiguration =
                configuration.GetSection("TestBlobStorageConfiguration").Get<BlobStorageConfiguration>()
                ?? new BlobStorageConfiguration();

            resourceAccessRuleSetsStoreStorageConfiguration.Container = "resourceaccessrulesets";

            return new EnrollmentConfigurationItem[]
            {
                new EnrollmentBlobStorageConfigurationItem
                {
                    Key = "claimPermissionsStore",
                    Configuration = claimPermissionsStoreStorageConfiguration,
                },
                new EnrollmentBlobStorageConfigurationItem
                {
                    Key = "resourceAccessRuleSetsStore",
                    Configuration = resourceAccessRuleSetsStoreStorageConfiguration,
                },
            };
        }
    }
}
