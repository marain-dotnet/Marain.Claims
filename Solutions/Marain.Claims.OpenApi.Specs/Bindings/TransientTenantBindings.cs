// <copyright file="TransientTenantBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Bindings
{
    using System.Threading.Tasks;

    using BoDi;

    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;

    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using TechTalk.SpecFlow;

    [Binding]
    public static class TransientTenantBindings
    {
        [BeforeFeature("@useClaimsApi", Order = BindingSequence.TransientTenantSetup)]
        public static async Task SetupTransientTenant(FeatureContext featureContext, IObjectContainer objectContainer)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<ITenantProvider>();
            var transientTenantManager = TransientTenantManager.GetInstance(featureContext);
            await transientTenantManager.EnsureInitialised().ConfigureAwait(false);

            // Create a transient service tenant for testing purposes.
            ITenant transientServiceTenant = await transientTenantManager.CreateTransientServiceTenantFromEmbeddedResourceAsync(
                typeof(TransientTenantBindings).Assembly,
                "Marain.Claims.OpenApi.Specs.ServiceManifests.ClaimsServiceManifest.jsonc").ConfigureAwait(false);

            // TODO: This only works for the direct invocation mode. We have to do different things for the other
            // modes. Consider creating a separate DI container for the direct mode instance.
            IConfiguration configuration = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<IConfiguration>();
            configuration["MarainServiceConfiguration:ServiceTenantId"] = transientServiceTenant.Id;

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

            var testContext = new ClaimsServiceTestTenants(
                transientServiceTenant.Id,
                transientClientTenant.Id);
            objectContainer.RegisterInstanceAs(testContext);
        }

        private static EnrollmentConfigurationItem[] GetClaimsConfig(FeatureContext featureContext)
        {
            IConfiguration configuration = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<IConfiguration>();

            // Can't create a logger using the generic type of this class because it's static, so we'll do it using
            // the feature context instead.
            ILogger<FeatureContext> logger = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<ILogger<FeatureContext>>();

            BlobStorageConfiguration claimPermissionsStoreStorageConfiguration =
                configuration.GetSection("TestBlobStorageConfiguration").Get<BlobStorageConfiguration>()
                ?? new BlobStorageConfiguration();

            claimPermissionsStoreStorageConfiguration.Container = "claimpermissions";

            if (string.IsNullOrEmpty(claimPermissionsStoreStorageConfiguration.AccountName))
            {
                logger.LogDebug("No configuration value 'TestBlobStorageConfiguration:AccountName' provided; using local storage emulator.");
            }

            BlobStorageConfiguration resourceAccessRuleSetsStoreStorageConfiguration =
                configuration.GetSection("TestBlobStorageConfiguration").Get<BlobStorageConfiguration>()
                ?? new BlobStorageConfiguration();

            resourceAccessRuleSetsStoreStorageConfiguration.Container = "resourceaccessrulesets";

            if (string.IsNullOrEmpty(resourceAccessRuleSetsStoreStorageConfiguration.AccountName))
            {
                logger.LogDebug("No configuration value 'TestBlobStorageConfiguration:AccountName' provided; using local storage emulator.");
            }

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