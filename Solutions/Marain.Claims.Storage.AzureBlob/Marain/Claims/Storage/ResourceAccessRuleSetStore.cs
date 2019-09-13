// <copyright file="ResourceAccessRuleSetStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Newtonsoft.Json;

    /// <summary>
    ///     An implementation of <see cref="IResourceAccessRuleSetStore" /> that is backed by
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
    ///         storing <see cref="ResourceAccessRuleSet" /> documents is used only for that
    ///         purpose.
    ///     </para>
    /// </remarks>
    public class ResourceAccessRuleSetStore : IResourceAccessRuleSetStore
    {
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResourceAccessRuleSetStore" /> class
        ///     backed by the given repository.
        /// </summary>
        /// <param name="container">
        ///     The <see cref="CloudBlobContainer" /> in which permissions will be stored.
        /// </param>
        /// <param name="serializerSettingsProvider">The <see cref="IJsonSerializerSettingsProvider"/> for serializion.</param>
        public ResourceAccessRuleSetStore(
            CloudBlobContainer container,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.Container = container;
            this.serializerSettings = serializerSettingsProvider.Instance;
        }

        /// <summary>
        ///     Gets the <see cref="CloudBlobContainer" /> in which resource access rule sets will
        ///     be stored.
        /// </summary>
        protected CloudBlobContainer Container { get; }

        /// <inheritdoc/>
        public Task<ResourceAccessRuleSet> GetAsync(string id, string eTag)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return this.DownloadBlobAsync(id, eTag);
        }

        /// <inheritdoc/>
        public Task<ResourceAccessRuleSet> GetAsync(string id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return this.DownloadBlobAsync(id, null);
        }

        /// <inheritdoc/>
        public async Task<ResourceAccessRuleSetCollection> GetBatchAsync(IEnumerable<IdWithETag> ids, int maxParallelism = 5)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = new ResourceAccessRuleSetCollection();

            foreach (IList<IdWithETag> batch in ids.Buffer(maxParallelism))
            {
                IList<Task<ResourceAccessRuleSet>> taskBatch = batch.Select(id => Task.Run(async () =>
                {
                    return await this.DownloadBlobAsync(id.Id, id.ETag).ConfigureAwait(false);
                })).ToList();

                ResourceAccessRuleSet[] ruleSets = await Task.WhenAll(taskBatch).ConfigureAwait(false);
                result.RuleSets.AddRange(ruleSets.Where(r => r != null));
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResourceAccessRuleSetCollection> GetBatchAsync(IEnumerable<string> ids, int maxParallelism = 5)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = new ResourceAccessRuleSetCollection();

            foreach (IList<string> batch in ids.Buffer(maxParallelism))
            {
                IList<Task<ResourceAccessRuleSet>> taskBatch = batch.Select(id => Task.Run(async () =>
                {
                    return await this.DownloadBlobAsync(id, null).ConfigureAwait(false);
                })).ToList();

                ResourceAccessRuleSet[] ruleSets = await Task.WhenAll(taskBatch).ConfigureAwait(false);
                result.RuleSets.AddRange(ruleSets);
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ResourceAccessRuleSet> PersistAsync(ResourceAccessRuleSet ruleSet)
        {
            if (ruleSet is null)
            {
                throw new ArgumentNullException(nameof(ruleSet));
            }

            CloudBlockBlob blob = this.Container.GetBlockBlobReference(ruleSet.Id);
            string serializedPermissions = JsonConvert.SerializeObject(ruleSet, this.serializerSettings);
            await blob.UploadTextAsync(serializedPermissions, Encoding.UTF8, new AccessCondition { IfMatchETag = ruleSet.ETag }, null, null).ConfigureAwait(false);
            ruleSet.ETag = blob.Properties.ETag;
            return ruleSet;
        }

        private async Task<ResourceAccessRuleSet> DownloadBlobAsync(string id, string eTag)
        {
            try
            {
                CloudBlockBlob blob = this.Container.GetBlockBlobReference(id);
                string ruleSetJson = await blob.DownloadTextAsync(Encoding.UTF8, eTag != null ? AccessCondition.GenerateIfNoneMatchCondition(eTag) : null, null, null).ConfigureAwait(false);
                ResourceAccessRuleSet ruleSet = JsonConvert.DeserializeObject<ResourceAccessRuleSet>(ruleSetJson, this.serializerSettings);
                ruleSet.ETag = blob.Properties.ETag;
                return ruleSet;
            }
            catch (StorageException storeEx) when (storeEx.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotModified)
            {
                // NOP - we are quite happy to ignore that, as the blob hasn't changed.
                return null;
            }
            catch (Exception ex)
            {
                throw new ResourceAccessRuleSetNotFoundException(id, ex);
            }
        }
    }
}
