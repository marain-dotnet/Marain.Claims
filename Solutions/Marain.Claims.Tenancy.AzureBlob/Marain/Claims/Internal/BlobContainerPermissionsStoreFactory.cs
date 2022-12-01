// <copyright file="BlobContainerPermissionsStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Internal
{
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;

    using Corvus.Extensions.Json;
    using Corvus.Storage.Azure.BlobStorage.Tenancy;
    using Corvus.Tenancy;
    using Marain.Claims.Storage;

    /// <summary>
    ///     Factory class for obtaining instances of <see cref="IClaimPermissionsStore" /> and <see cref="IResourceAccessRuleSetStore"/>
    ///     and <see cref="IResourceAccessRuleSetStore" />.
    /// </summary>
    public class BlobContainerPermissionsStoreFactory : IPermissionsStoreFactory
    {
        private const string ClaimPermissionsRepositoryName = "claimpermissions";
        private const string ClaimPermissionsV2ConfigKey = "StorageConfiguration__" + ClaimPermissionsRepositoryName;
        private const string ClaimPermissionsV3ConfigKey = ClaimsAzureBlobTenancyPropertyKeys.ClaimPermissions;
        private const string ResourceAccessRuleSetRepositoryName = "resourceaccessrulesets";
        private const string ResourceAccessRuleSetV2ConfigKey = "StorageConfiguration__" + ResourceAccessRuleSetRepositoryName;
        private const string ResourceAccessRuleSetV3ConfigKey = ClaimsAzureBlobTenancyPropertyKeys.ResourceAccessRuleSet;
        private readonly IBlobContainerSourceWithTenantLegacyTransition tenantBlobContainerSource;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlobContainerPermissionsStoreFactory"/> class.
        /// </summary>
        /// <param name="tenantBlobContainerSource">
        ///     The repository factory.
        /// </param>
        /// <param name="serializerSettingsProvider">
        ///     The <see cref="IJsonSerializerSettingsProvider"/> to use for the stores.
        /// </param>
        public BlobContainerPermissionsStoreFactory(
            IBlobContainerSourceWithTenantLegacyTransition tenantBlobContainerSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.tenantBlobContainerSource = tenantBlobContainerSource ?? throw new System.ArgumentNullException(nameof(tenantBlobContainerSource));
            this.serializerSettingsProvider = serializerSettingsProvider ?? throw new System.ArgumentNullException(nameof(serializerSettingsProvider));
        }

        /// <inheritdoc/>
        public async Task<IClaimPermissionsStore> GetClaimPermissionsStoreAsync(ITenant tenant)
        {
            BlobContainerClient container = await this.tenantBlobContainerSource.GetBlobContainerClientFromTenantAsync(
                tenant,
                ClaimPermissionsV2ConfigKey,
                ClaimPermissionsV3ConfigKey,
                ClaimPermissionsRepositoryName);
            return new ClaimPermissionsStore(container, await this.GetResourceAccessRuleSetStoreAsync(tenant).ConfigureAwait(false), this.serializerSettingsProvider);
        }

        /// <inheritdoc/>
        public async Task<IResourceAccessRuleSetStore> GetResourceAccessRuleSetStoreAsync(ITenant tenant)
        {
            BlobContainerClient container = await this.tenantBlobContainerSource.GetBlobContainerClientFromTenantAsync(
                tenant,
                ResourceAccessRuleSetV2ConfigKey,
                ResourceAccessRuleSetV3ConfigKey,
                ResourceAccessRuleSetRepositoryName);
            return new ResourceAccessRuleSetStore(container, this.serializerSettingsProvider);
        }
    }
}