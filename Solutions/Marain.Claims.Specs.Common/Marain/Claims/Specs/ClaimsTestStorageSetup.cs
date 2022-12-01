// <copyright file="ClaimsTestStorageSetup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Specs;

using System;

using Azure.Storage.Blobs;

using Corvus.Storage.Azure.BlobStorage;
using Corvus.Tenancy;
using Corvus.Testing.SpecFlow;

using Marain.Claims.Storage;
using Marain.TenantManagement.Configuration;
using Marain.TenantManagement.EnrollmentConfiguration;
using Marain.TenantManagement.Testing;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TechTalk.SpecFlow;

/// <summary>
/// Code shared across <c>Marain.Claims.Specs</c> and <c>Marain.Claims.OpenApi.Specs</c> for
/// setting up blob storage.
/// </summary>
public static class ClaimsTestStorageSetup
{
    /// <summary>
    /// Builds an <see cref="EnrollmentConfigurationEntry"/> for Claims with unique container
    /// names and creates those containers.
    /// </summary>
    /// <param name="featureContext">The SpecFlow context.</param>
    /// <param name="serviceProvider">DI service provider.</param>
    /// <returns>
    /// A task producing the new <see cref="EnrollmentConfigurationEntry"/>.
    /// </returns>
    public static async Task<EnrollmentConfigurationEntry> CreateEnrollmentConfigurationAndEnsureContainersExistForEnrollmentAsync(
        FeatureContext featureContext,
        IServiceProvider serviceProvider)
    {
        EnrollmentConfigurationEntry enrollmentConfiguration = CreateClaimsConfig(featureContext);
        IBlobContainerSourceFromDynamicConfiguration blobContainerSource = serviceProvider.GetRequiredService<IBlobContainerSourceFromDynamicConfiguration>();
        var claimsPermissionsConfig = (BlobContainerConfigurationItem)enrollmentConfiguration.ConfigurationItems[ClaimsAzureBlobTenancyPropertyKeys.ClaimPermissions];
        var ruleSetsConfig = (BlobContainerConfigurationItem)enrollmentConfiguration.ConfigurationItems[ClaimsAzureBlobTenancyPropertyKeys.ResourceAccessRuleSet];
        BlobContainerClient claimsPermissionsContainer = await blobContainerSource.GetStorageContextAsync(claimsPermissionsConfig.Configuration);
        BlobContainerClient ruleSetsContainer = await blobContainerSource.GetStorageContextAsync(ruleSetsConfig.Configuration);
        await claimsPermissionsContainer.CreateIfNotExistsAsync();
        await ruleSetsContainer.CreateIfNotExistsAsync();

        return enrollmentConfiguration;
    }

    /// <summary>
    /// Tears down blob containers created for the transient tenant's Claims enrollment.
    /// </summary>
    /// <param name="featureContext">The SpecFlow context.</param>
    /// <returns>
    /// A task that completes once the containers have been torn down.
    /// </returns>
    public static async Task TearDownBlobContainersAsync(FeatureContext featureContext)
    {
        ITenant transientTenant = TransientTenantManager.GetInstance(featureContext).PrimaryTransientClient;
        IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);

        if (transientTenant != null && serviceProvider != null)
        {
            IPermissionsStoreFactory permissionsStoreFactory = serviceProvider.GetRequiredService<IPermissionsStoreFactory>();

            await featureContext.RunAndStoreExceptionsAsync(async () =>
            {
                var claimsStore = (ClaimPermissionsStore)await permissionsStoreFactory.GetClaimPermissionsStoreAsync(transientTenant);
                await claimsStore.Container.DeleteAsync();
            }).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(async () =>
            {
                var ruleSetsStore = (ResourceAccessRuleSetStore)await permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(transientTenant);
                await ruleSetsStore.Container.DeleteAsync();
            }).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Builds an <see cref="EnrollmentConfigurationEntry"/> for Claims, with unique container
    /// names.
    /// </summary>
    /// <param name="featureContext">The SpecFlow context.</param>
    /// <returns>
    /// An <see cref="EnrollmentConfigurationEntry"/> suitable for enrolling a tenant in Claims.
    /// </returns>
    private static EnrollmentConfigurationEntry CreateClaimsConfig(FeatureContext featureContext)
    {
        // We need each test run to have a distinct container. We want these test-generated
        // containers to be easily recognized in storage accounts, so we don't just want to use
        // GUIDs.
        string testRunId = DateTime.Now.ToString("yyyy-MM-dd-hhmmssfff");

        IConfiguration configuration = ContainerBindings
            .GetServiceProvider(featureContext)
            .GetRequiredService<IConfiguration>();

        // Can't create a logger using the generic type of this class because it's static, so we'll do it using
        // the feature context instead.
        ILogger<FeatureContext> logger = ContainerBindings
            .GetServiceProvider(featureContext)
            .GetRequiredService<ILogger<FeatureContext>>();

        BlobContainerConfiguration claimPermissionsStoreStorageConfiguration =
            configuration.GetSection("TestBlobStorageConfiguration").Get<BlobContainerConfiguration>()
            ?? new BlobContainerConfiguration();

        claimPermissionsStoreStorageConfiguration.Container = $"specs-claims-claimpermissions-{testRunId}";

        if (string.IsNullOrEmpty(claimPermissionsStoreStorageConfiguration.AccountName))
        {
            logger.LogDebug("No configuration value 'TestBlobStorageConfiguration:AccountName' provided; using local storage emulator.");
        }

        BlobContainerConfiguration resourceAccessRuleSetsStoreStorageConfiguration =
            configuration.GetSection("TestBlobStorageConfiguration").Get<BlobContainerConfiguration>()
            ?? new BlobContainerConfiguration();

        resourceAccessRuleSetsStoreStorageConfiguration.Container = $"specs-claims-resourceaccessrulesets-{testRunId}";

        if (string.IsNullOrEmpty(resourceAccessRuleSetsStoreStorageConfiguration.AccountName))
        {
            logger.LogDebug("No configuration value 'TestBlobStorageConfiguration:AccountName' provided; using local storage emulator.");
        }

        return new EnrollmentConfigurationEntry(
            new Dictionary<string, ConfigurationItem>
            {
                {
                    ClaimsAzureBlobTenancyPropertyKeys.ClaimPermissions,
                    new BlobContainerConfigurationItem
                    {
                        Configuration = claimPermissionsStoreStorageConfiguration,
                    }
                },
                {
                    ClaimsAzureBlobTenancyPropertyKeys.ResourceAccessRuleSet,
                    new BlobContainerConfigurationItem
                    {
                        Configuration = resourceAccessRuleSetsStoreStorageConfiguration,
                    }
                },
            },
            null);
    }
}