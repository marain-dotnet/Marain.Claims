// <copyright file="PermissionsStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Internal
{
    using System.Threading.Tasks;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Marain.Claims.Storage;
    using Microsoft.Azure.Storage.Blob;

    /// <summary>
    ///     Factory class for obtaining instances of <see cref="IClaimPermissionsStore" /> and <see cref="IResourceAccessRuleSetStore"/>
    ///     and <see cref="IResourceAccessRuleSetStore" />.
    /// </summary>
    public class PermissionsStoreFactory : IPermissionsStoreFactory
    {
        private readonly BlobStorageContainerDefinition claimPermissionsRepositoryDefinition;
        private readonly BlobStorageContainerDefinition resourceAccessRuleSetRepositoryDefinition;
        private readonly ITenantCloudBlobContainerFactory tenantCloudBlobContainerFactory;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PermissionsStoreFactory"/> class.
        /// </summary>
        /// <param name="tenantCloudBlobContainerFactory">
        ///     The repository factory.
        /// </param>
        /// <param name="serializerSettingsProvider">
        ///     The <see cref="IJsonSerializerSettingsProvider"/> to use for the stores.
        /// </param>
        public PermissionsStoreFactory(
            ITenantCloudBlobContainerFactory tenantCloudBlobContainerFactory,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.tenantCloudBlobContainerFactory = tenantCloudBlobContainerFactory ?? throw new System.ArgumentNullException(nameof(tenantCloudBlobContainerFactory));
            this.serializerSettingsProvider = serializerSettingsProvider ?? throw new System.ArgumentNullException(nameof(serializerSettingsProvider));
            this.claimPermissionsRepositoryDefinition = new BlobStorageContainerDefinition("claimpermissions");
            this.resourceAccessRuleSetRepositoryDefinition = new BlobStorageContainerDefinition("resourceaccessrulesets");
        }

        /// <inheritdoc/>
        public async Task<IClaimPermissionsStore> GetClaimPermissionsStoreAsync(ITenant tenant)
        {
            CloudBlobContainer container = await this.tenantCloudBlobContainerFactory.GetBlobContainerForTenantAsync(tenant, this.claimPermissionsRepositoryDefinition);
            return new ClaimPermissionsStore(container, await this.GetResourceAccessRuleSetStoreAsync(tenant).ConfigureAwait(false), this.serializerSettingsProvider);
        }

        /// <inheritdoc/>
        public async Task<IResourceAccessRuleSetStore> GetResourceAccessRuleSetStoreAsync(ITenant tenant)
        {
            CloudBlobContainer container = await this.tenantCloudBlobContainerFactory.GetBlobContainerForTenantAsync(tenant, this.resourceAccessRuleSetRepositoryDefinition);

            return new ResourceAccessRuleSetStore(container, this.serializerSettingsProvider);
        }
    }
}
