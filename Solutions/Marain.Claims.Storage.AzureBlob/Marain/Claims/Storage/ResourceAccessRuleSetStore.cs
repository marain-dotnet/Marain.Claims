// <copyright file="ResourceAccessRuleSetStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1021 // Convert lambda expression body to expression body. - would decrease readability in the cases Roslynator is suggesting

namespace Marain.Claims.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;

    using Corvus.Extensions.Json;

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
        ///     The <see cref="BlobContainerClient" /> in which permissions will be stored.
        /// </param>
        /// <param name="serializerSettingsProvider">The <see cref="IJsonSerializerSettingsProvider"/> for serializion.</param>
        public ResourceAccessRuleSetStore(
            BlobContainerClient container,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.Container = container;
            this.serializerSettings = serializerSettingsProvider.Instance;
        }

        /// <summary>
        ///     Gets the <see cref="BlobContainerClient" /> in which resource access rule sets will
        ///     be stored.
        /// </summary>
        public BlobContainerClient Container { get; }

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

            BlockBlobClient blob = this.Container.GetBlockBlobClient(ruleSet.Id);
            string serializedPermissions = JsonConvert.SerializeObject(ruleSet, this.serializerSettings);
            using var content = BinaryData.FromString(serializedPermissions).ToStream();
            Response<BlobContentInfo> response = await blob.UploadAsync(
                content,
                new BlobUploadOptions { Conditions = new BlobRequestConditions { IfMatch = new ETag(ruleSet.ETag) } })
                .ConfigureAwait(false);
            ruleSet.ETag = response.Value.ETag.ToString("G");
            return ruleSet;
        }

        private async Task<ResourceAccessRuleSet> DownloadBlobAsync(string id, string eTag)
        {
            BlockBlobClient blob = this.Container.GetBlockBlobClient(id);

            // Can't use DownloadContentAsync because of https://github.com/Azure/azure-sdk-for-net/issues/22598
            try
            {
                Response<BlobDownloadStreamingResult> response = await blob.DownloadStreamingAsync(
                    conditions: string.IsNullOrEmpty(eTag) ? null : new BlobRequestConditions { IfNoneMatch = new ETag(eTag!) })
                    .ConfigureAwait(false);

                int status = response.GetRawResponse().Status;
                if (status == 304)
                {
                    // NOP - we are quite happy to ignore that, as the blob hasn't changed.
                    return null;
                }

                // Note: it is technically possible to use System.Text.Json to work directly from
                // the UTF-8 data, which is more efficient than decoding to a .NET UTF-16 string
                // first. However, we have to do this for the time being because we are in the world of
                // IJsonSerializerSettingsProvider, where all serialization options are managed in
                // terms of JSON.NET.
                using BlobDownloadStreamingResult blobDownloadStreamingResult = response.Value;
                BinaryData data = await BinaryData.FromStreamAsync(blobDownloadStreamingResult.Content).ConfigureAwait(false);
                string ruleSetJson = data.ToString();

                ResourceAccessRuleSet ruleSet = JsonConvert.DeserializeObject<ResourceAccessRuleSet>(ruleSetJson, this.serializerSettings);
                ruleSet.ETag = response.Value.Details.ETag.ToString("G");
                return ruleSet;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                throw new ResourceAccessRuleSetNotFoundException(id, ex);
            }
        }
    }
}