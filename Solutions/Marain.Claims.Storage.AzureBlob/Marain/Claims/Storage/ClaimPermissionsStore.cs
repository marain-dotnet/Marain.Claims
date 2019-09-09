// <copyright file="ClaimPermissionsStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Storage
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Newtonsoft.Json;

    /// <summary>
    ///     An implementation of <see cref="IClaimPermissionsStore" /> that is backed by
    ///     Azure blob storage.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Basing the store on the blob container allows the claims
    ///         to support multitenancy in a relatively straightforward fashion.
    ///         Instances of the blobb container should be created via a
    ///         factory class that instantiates the new repository with tenant specific
    ///         information.
    ///     </para>
    ///     <para>
    ///         You should ensure that the underlying blob container used for
    ///         storing <see cref="ClaimPermissions" /> documents is used only for that
    ///         purpose.
    ///     </para>
    /// </remarks>
    public class ClaimPermissionsStore : IClaimPermissionsStore
    {
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClaimPermissionsStore" /> class
        ///     backed by the given repository.
        /// </summary>
        /// <param name="container">
        ///     The <see cref="CloudBlobContainer" /> in which permissions will be stored.
        /// </param>
        /// <param name="serializerSettingsProvider">The <see cref="IJsonSerializerSettingsProvider"/> for serializion.</param>
        public ClaimPermissionsStore(
            CloudBlobContainer container,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.Container = container;
            this.serializerSettings = serializerSettingsProvider.Instance;
        }

        /// <summary>
        ///     Gets the <see cref="CloudBlobContainer" /> in which claim permissions will
        ///     be stored.
        /// </summary>
        protected CloudBlobContainer Container { get; }

        /// <inheritdoc/>
        public async Task<ClaimPermissions> GetAsync(string id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                CloudBlockBlob blob = this.Container.GetBlockBlobReference(id);
                string permissions = await blob.DownloadTextAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<ClaimPermissions>(permissions, this.serializerSettings);
            }
            catch (Exception ex)
            {
                throw new ClaimPermissionsNotFoundException(id, ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClaimPermissions> PersistAsync(ClaimPermissions claimPermissions)
        {
            if (claimPermissions is null)
            {
                throw new ArgumentNullException(nameof(claimPermissions));
            }

            CloudBlockBlob blob = this.Container.GetBlockBlobReference(claimPermissions.Id);
            string serializedPermissions = JsonConvert.SerializeObject(claimPermissions, this.serializerSettings);
            await blob.UploadTextAsync(serializedPermissions, Encoding.UTF8, new AccessCondition { IfMatchETag = claimPermissions.ETag }, null, null).ConfigureAwait(false);
            claimPermissions.ETag = blob.Properties.ETag;
            return claimPermissions;
        }

        /// <inheritdoc/>
        public async Task<bool> AnyPermissions()
        {
            BlobResultSegment result = await this.Container.ListBlobsSegmentedAsync(null).ConfigureAwait(false);
            return result.Results.Any();
        }
    }
}
