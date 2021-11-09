// <copyright file="ClaimPermissionsStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;

    using Corvus.Extensions;
    using Corvus.Extensions.Json;
    using Newtonsoft.Json;

    /// <summary>
    ///     An implementation of <see cref="IClaimPermissionsStore" /> that is backed by
    ///     Azure blob storage.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Basing the store on the blob container allows the claims
    ///         to support multitenancy in a relatively straightforward fashion.
    ///         Instances of the blob container should be created via a
    ///         factory class that instantiates the new blob container with tenant specific
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
        private readonly IResourceAccessRuleSetStore resourceAccessRuleSetStore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClaimPermissionsStore" /> class
        ///     backed by the given repository.
        /// </summary>
        /// <param name="container">
        ///     The <see cref="BlobContainerClient" /> in which permissions will be stored.
        /// </param>
        /// <param name="resourceAccessRuleSetStore">The resource access rule set.</param>
        /// <param name="serializerSettingsProvider">The <see cref="IJsonSerializerSettingsProvider"/> for serializion.</param>
        public ClaimPermissionsStore(
            BlobContainerClient container,
            IResourceAccessRuleSetStore resourceAccessRuleSetStore,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            if (serializerSettingsProvider is null)
            {
                throw new ArgumentNullException(nameof(serializerSettingsProvider));
            }

            this.Container = container ?? throw new ArgumentNullException(nameof(container));
            this.resourceAccessRuleSetStore = resourceAccessRuleSetStore ?? throw new ArgumentNullException(nameof(resourceAccessRuleSetStore));
            this.serializerSettings = serializerSettingsProvider.Instance;
        }

        /// <summary>
        ///     Gets the <see cref="BlobContainerClient" /> in which claim permissions will
        ///     be stored.
        /// </summary>
        public BlobContainerClient Container { get; }

        /// <inheritdoc/>
        public async Task<ClaimPermissions> GetAsync(string id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                ClaimPermissions permissions = await this.DownloadPermissionsAsync(id);
                permissions = await this.UpdateRuleSetsAsync(permissions).ConfigureAwait(false);
                return permissions;
            }
            catch (Exception ex) when (!(ex is ResourceAccessRuleSetNotFoundException))
            {
                throw new ClaimPermissionsNotFoundException(id, ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ClaimPermissionsCollection> GetBatchAsync(IEnumerable<string> ids, int maxParallelism = 5)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = new ClaimPermissionsCollection();

            foreach (IList<string> batch in ids.Distinct().Buffer(maxParallelism))
            {
                IList<Task<ClaimPermissions>> taskBatch = batch.Select(id => Task.Run(async () =>
                {
                    try
                    {
                        return await this.DownloadPermissionsAsync(id).ConfigureAwait(false);
                    }
                    catch (RequestFailedException)
                    {
                        return null;
                    }
                })).ToList();

                ClaimPermissions[] claimPermissions = await Task.WhenAll(taskBatch).ConfigureAwait(false);
                result.Permissions.AddRange(claimPermissions.Where(p => p != null));
            }

            IEnumerable<ClaimPermissions> updatedPermissions = await this.UpdateRuleSetsAsync(result.Permissions, maxParallelism).ConfigureAwait(false);
            result.Permissions = updatedPermissions.ToList();
            return result;
        }

        /// <inheritdoc/>
        public async Task<ClaimPermissions> CreateAsync(ClaimPermissions claimPermissions)
        {
            if (claimPermissions is null)
            {
                throw new ArgumentNullException(nameof(claimPermissions));
            }

            BlockBlobClient blob = this.Container.GetBlockBlobClient(claimPermissions.Id);
            string serializedPermissions = JsonConvert.SerializeObject(claimPermissions, this.serializerSettings);
            try
            {
                using var content = BinaryData.FromString(serializedPermissions).ToStream();
                Response<BlobContentInfo> response = await blob.UploadAsync(
                    content,
                    new BlobUploadOptions { Conditions = new BlobRequestConditions { IfNoneMatch = ETag.All } })
                    .ConfigureAwait(false);
                claimPermissions.ETag = response.Value.ETag.ToString("G");
                return claimPermissions;
            }
            catch (RequestFailedException x)
            {
                System.Diagnostics.Debug.WriteLine(x.ToString());
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc/>
        public async Task<ClaimPermissions> UpdateAsync(ClaimPermissions claimPermissions)
        {
            if (claimPermissions is null)
            {
                throw new ArgumentNullException(nameof(claimPermissions));
            }

            if (string.IsNullOrWhiteSpace(claimPermissions.ETag))
            {
                throw new ArgumentException(
                    "There is no ETag on this ClaimPermissions. Updates are not safe without an ETag.");
            }

            BlobClient blob = this.Container.GetBlobClient(claimPermissions.Id);
            string serializedPermissions = JsonConvert.SerializeObject(claimPermissions, this.serializerSettings);
            Response<BlobContentInfo> response = await blob.UploadAsync(
                BinaryData.FromString(serializedPermissions),
                new BlobUploadOptions { Conditions = new BlobRequestConditions { IfMatch = new ETag(claimPermissions.ETag) } })
                .ConfigureAwait(false);
            claimPermissions.ETag = response.Value.ETag.ToString("G");
            return claimPermissions;
        }

        /// <inheritdoc/>
        public async Task<bool> AnyPermissions()
        {
            // LINQ Any operator would be more succinct, but we don't currently have a dependency on
            // System.Linq.Async, which is where LINQ for IAsyncEnumerable is defined, and it seems
            // a rather simple job to lug in an extra dependency for.
            await foreach (BlobItem blob in this.Container.GetBlobsAsync())
            {
                return true;
            }

            return false;
        }

        private static Dictionary<string, int> BuildDictionaryOfIdsToIndices(ClaimPermissions permissions)
        {
            var permissionsDictionary = new Dictionary<string, int>();
            permissions.ResourceAccessRuleSets.ForEachAtIndex((r, i) => permissionsDictionary.Add(r.Id, i));
            return permissionsDictionary;
        }

        private async Task<ClaimPermissions> DownloadPermissionsAsync(string id)
        {
            BlobClient blob = this.Container.GetBlobClient(id);
            Response<BlobDownloadResult> response = await blob.DownloadContentAsync().ConfigureAwait(false);

            // Note: although BlobDownloadResult supports direct deserialization from JSON, using System.Text.Json
            // (meaning it can work directly with UTF-8 content, avoiding the conversion to UTF-16 we're doing
            // here) we currently depend on the JSON.NET serialization settings mechanism, so we have to use
            // this more inefficient route for now.
            string permissionsJson = response.Value.Content.ToString();
            ClaimPermissions permissions = JsonConvert.DeserializeObject<ClaimPermissions>(permissionsJson, this.serializerSettings);
            permissions.ETag = response.Value.Details.ETag.ToString("G");
            return permissions;
        }

        private async Task<ClaimPermissions> UpdateRuleSetsAsync(ClaimPermissions permissions)
        {
            IEnumerable<ClaimPermissions> result = await this.UpdateRuleSetsAsync(new[] { permissions }, 1).ConfigureAwait(false);
            return result.First();
        }

        private async Task<IEnumerable<ClaimPermissions>> UpdateRuleSetsAsync(IEnumerable<ClaimPermissions> permissionsSets, int maxParallelism)
        {
            ResourceAccessRuleSetCollection updatedRuleSets =
                await this.resourceAccessRuleSetStore.GetBatchAsync(
                    permissionsSets
                        .SelectMany(p => p.ResourceAccessRuleSets.Select(r => new IdWithETag(r.Id, r.ETag)))
                        .Distinct()).ConfigureAwait(false);

            if (updatedRuleSets.RuleSets.Any())
            {
                IList<ClaimPermissions> results = new List<ClaimPermissions>();

                foreach (IList<ClaimPermissions> batch in permissionsSets.Buffer(maxParallelism))
                {
                    await this.UpdateBatchAsync(updatedRuleSets, results, batch).ConfigureAwait(false);
                }

                return results;
            }

            return permissionsSets;
        }

        private async Task UpdateBatchAsync(ResourceAccessRuleSetCollection updatedRuleSets, IList<ClaimPermissions> results, IList<ClaimPermissions> batch)
        {
            var tasks = new List<Task<ClaimPermissions>>();
            foreach (ClaimPermissions permissions in batch)
            {
                Dictionary<string, int> permissionsDictionary = BuildDictionaryOfIdsToIndices(permissions);

                bool hasUpdates = false;
                updatedRuleSets.RuleSets.ForEach(newRuleSet =>
                {
                    if (permissionsDictionary.TryGetValue(newRuleSet.Id, out int index))
                    {
                        permissions.ResourceAccessRuleSets.RemoveAt(index);
                        permissions.ResourceAccessRuleSets.Insert(index, newRuleSet);
                        hasUpdates = true;
                    }
                });

                if (hasUpdates)
                {
                    tasks.Add(this.UpdateAsync(permissions));
                }
                else
                {
                    results.Add(permissions);
                }
            }

            if (tasks.Any())
            {
                results.AddRange(await Task.WhenAll(tasks).ConfigureAwait(false));
            }
        }
    }
}
