// <copyright file="BlobContainerPermissionsStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Internal
{
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;

    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Marain.Claims.Storage;

    /// <summary>
    ///     Factory class for obtaining instances of <see cref="IClaimPermissionsStore" /> and <see cref="IResourceAccessRuleSetStore"/>
    ///     and <see cref="IResourceAccessRuleSetStore" />.
    /// </summary>
    public class BlobContainerPermissionsStoreFactory : IPermissionsStoreFactory
    {
        private readonly BlobStorageContainerDefinition claimPermissionsRepositoryDefinition;
        private readonly BlobStorageContainerDefinition resourceAccessRuleSetRepositoryDefinition;
        private readonly ITenantBlobContainerClientFactory tenantBlobContainerClientFactory;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlobContainerPermissionsStoreFactory"/> class.
        /// </summary>
        /// <param name="tenantBlobContainerClientFactory">
        ///     The repository factory.
        /// </param>
        /// <param name="serializerSettingsProvider">
        ///     The <see cref="IJsonSerializerSettingsProvider"/> to use for the stores.
        /// </param>
        public BlobContainerPermissionsStoreFactory(
            ITenantBlobContainerClientFactory tenantBlobContainerClientFactory,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.tenantBlobContainerClientFactory = tenantBlobContainerClientFactory ?? throw new System.ArgumentNullException(nameof(tenantBlobContainerClientFactory));
            this.serializerSettingsProvider = serializerSettingsProvider ?? throw new System.ArgumentNullException(nameof(serializerSettingsProvider));
            this.claimPermissionsRepositoryDefinition = new BlobStorageContainerDefinition("claimpermissions");
            this.resourceAccessRuleSetRepositoryDefinition = new BlobStorageContainerDefinition("resourceaccessrulesets");
        }

        /// <inheritdoc/>
        public async Task<IClaimPermissionsStore> GetClaimPermissionsStoreAsync(ITenant tenant)
        {
            BlobContainerClient container = await this.tenantBlobContainerClientFactory.GetBlobContainerForTenantAsync(tenant, this.claimPermissionsRepositoryDefinition);
            return new ClaimPermissionsStore(container, await this.GetResourceAccessRuleSetStoreAsync(tenant).ConfigureAwait(false), this.serializerSettingsProvider);
        }

        /// <inheritdoc/>
        public async Task<IResourceAccessRuleSetStore> GetResourceAccessRuleSetStoreAsync(ITenant tenant)
        {
            BlobContainerClient container = await this.tenantBlobContainerClientFactory.GetBlobContainerForTenantAsync(tenant, this.resourceAccessRuleSetRepositoryDefinition);
            return new ResourceAccessRuleSetStore(container, this.serializerSettingsProvider);
        }
    }
}
