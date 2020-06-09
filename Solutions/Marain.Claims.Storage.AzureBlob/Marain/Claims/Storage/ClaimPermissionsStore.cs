// <copyright file="ClaimPermissionsStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Extensions;
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
        ///     The <see cref="CloudBlobContainer" /> in which permissions will be stored.
        /// </param>
        /// <param name="resourceAccessRuleSetStore">The resource access rule set.</param>
        /// <param name="serializerSettingsProvider">The <see cref="IJsonSerializerSettingsProvider"/> for serializion.</param>
        public ClaimPermissionsStore(
            CloudBlobContainer container,
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
        ///     Gets the <see cref="CloudBlobContainer" /> in which claim permissions will
        ///     be stored.
        /// </summary>
        public CloudBlobContainer Container { get; }

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

            foreach (IList<string> batch in ids.Buffer(maxParallelism))
            {
                IList<Task<ClaimPermissions>> taskBatch = batch.Select(id => Task.Run(async () =>
                {
                    try
                    {
                        return await this.DownloadPermissionsAsync(id).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new ClaimPermissionsNotFoundException(id, ex);
                    }
                })).ToList();

                ClaimPermissions[] claimPermissions = await Task.WhenAll(taskBatch).ConfigureAwait(false);
                result.Permissions.AddRange(claimPermissions);
            }

            IEnumerable<ClaimPermissions> updatedPermissions = await this.UpdateRuleSetsAsync(result.Permissions, maxParallelism).ConfigureAwait(false);
            result.Permissions = updatedPermissions.ToList();
            return result;
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
            BlobResultSegment result = await this.Container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.None, 1, null, null, null).ConfigureAwait(false);
            return result.Results.Any();
        }

        private static Dictionary<string, int> BuildDictionaryOfIdsToIndices(ClaimPermissions permissions)
        {
            var permissionsDictionary = new Dictionary<string, int>();
            permissions.ResourceAccessRuleSets.ForEachAtIndex((r, i) => permissionsDictionary.Add(r.Id, i));
            return permissionsDictionary;
        }

        private async Task<ClaimPermissions> DownloadPermissionsAsync(string id)
        {
            CloudBlockBlob blob = this.Container.GetBlockBlobReference(id);
            string permissionsJson = await blob.DownloadTextAsync().ConfigureAwait(false);
            ClaimPermissions permissions = JsonConvert.DeserializeObject<ClaimPermissions>(permissionsJson, this.serializerSettings);
            permissions.ETag = blob.Properties.ETag;
            return permissions;
        }

        private async Task<ClaimPermissions> UpdateRuleSetsAsync(ClaimPermissions permissions)
        {
            IEnumerable<ClaimPermissions> result = await this.UpdateRuleSetsAsync(new[] { permissions }, 1).ConfigureAwait(false);
            return result.First();
        }

        private async Task<IEnumerable<ClaimPermissions>> UpdateRuleSetsAsync(IEnumerable<ClaimPermissions> permissionsSets, int maxParallelism)
        {
            ResourceAccessRuleSetCollection updatedRuleSets = await this.resourceAccessRuleSetStore.GetBatchAsync(permissionsSets.SelectMany(p => p.ResourceAccessRuleSets.Select(r => new IdWithETag(r.Id, r.ETag))).Distinct()).ConfigureAwait(false);
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
                    tasks.Add(this.PersistAsync(permissions));
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
