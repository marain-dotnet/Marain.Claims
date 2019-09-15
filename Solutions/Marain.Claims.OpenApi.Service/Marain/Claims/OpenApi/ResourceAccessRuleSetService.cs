// <copyright file="ResourceAccessRuleSetService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
namespace Marain.Claims.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.Tenancy;
    using Marain.Claims;
    using Marain.Claims.Storage;
    using Menes;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    ///     Handles resource access rule set requests.
    /// </summary>
    public class ResourceAccessRuleSetService : IOpenApiService
    {
        private const string CreateResourceAccessRuleSetOperationId = "createResourceAccessRuleSet";
        private const string GetResourceAccessRuleSetOperationId = "getResourceAccessRuleSet";
        private const string UpdateResourceAccessRuleSetResourceAccessRulesOperationId = "updateResourceAccessRuleSetResourceAccessRules";
        private const string SetResourceAccessRuleSetResourceAccessRulesOperationId = "setResourceAccessRuleSetResourceAccessRules";
        private readonly ITenantProvider tenantProvider;
        private readonly IPermissionsStoreFactory permissionsStoreFactory;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAccessRuleSetService"/> class.
        /// </summary>
        /// <param name="tenantProvider">The tenant provider.</param>
        /// <param name="permissionsStoreFactory">Provides access to the permissions store.</param>
        /// <param name="telemetryClient">A <see cref="TelemetryClient"/> to log telemetry.</param>
        public ResourceAccessRuleSetService(
            ITenantProvider tenantProvider,
            IPermissionsStoreFactory permissionsStoreFactory,
            TelemetryClient telemetryClient)
        {
            this.tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
            this.permissionsStoreFactory = permissionsStoreFactory ?? throw new ArgumentNullException(nameof(permissionsStoreFactory));
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        }

        /// <summary>
        /// Handles a request to create a resource access rule set.
        /// </summary>
        /// <param name="tenantId">The id of the tenant.</param>
        /// <param name="body">The resource access rule set to create.</param>
        /// <returns>The <see cref="ResourceAccessRuleSet"/>.</returns>
        [OperationId(CreateResourceAccessRuleSetOperationId)]
        public async Task<OpenApiResult> CreateResourceAccessRuleSetAsync(
            string tenantId,
            ResourceAccessRuleSet body)
        {
            using (this.telemetryClient.StartOperation<RequestTelemetry>(CreateResourceAccessRuleSetOperationId))
            {
                ITenant tenant = await this.TenantFromIdAsync(tenantId).ConfigureAwait(false);
                IResourceAccessRuleSetStore store = await this.permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(tenant).ConfigureAwait(false);
                ResourceAccessRuleSet result = await store.PersistAsync(body).ConfigureAwait(false);
                return this.OkResult(result);
            }
        }

        /// <summary>
        /// Handles a request to get a resource access rule set.
        /// </summary>
        /// <param name="tenantId">The id of the tenant.</param>
        /// <param name="resourceAccessRuleSetId">The resource access rule set ID.</param>
        /// <returns>The <see cref="ResourceAccessRuleSet"/>.</returns>
        [OperationId(GetResourceAccessRuleSetOperationId)]
        public async Task<OpenApiResult> GetResourceAccessRuleSetAsync(
            string tenantId,
            string resourceAccessRuleSetId)
        {
            using (this.telemetryClient.StartOperation<RequestTelemetry>(GetResourceAccessRuleSetOperationId))
            {
                ITenant tenant = await this.TenantFromIdAsync(tenantId).ConfigureAwait(false);
                IResourceAccessRuleSetStore store = await this.permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(tenant).ConfigureAwait(false);
                try
                {
                    ResourceAccessRuleSet result = await store.GetAsync(resourceAccessRuleSetId).ConfigureAwait(false);
                    return this.OkResult(result);
                }
                catch (ResourceAccessRuleSetNotFoundException)
                {
                    return this.NotFoundResult();
                }
            }
        }

        /// <summary>
        /// Handles a request to update a resource access rule set's resource access rules.
        /// </summary>
        /// <param name="tenantId">The id of the tenant.</param>
        /// <param name="resourceAccessRuleSetId">The resource access rule set ID.</param>
        /// <param name="operation">Add or remove operation.</param>
        /// <param name="body">The set of resource access rules to add/remove.</param>
        /// <returns>The <see cref="OpenApiResult" />.</returns>
        [OperationId(UpdateResourceAccessRuleSetResourceAccessRulesOperationId)]
        public async Task<OpenApiResult> UpdateResourceAccessRuleSetResourceAccessRulesAsync(
            string tenantId,
            string resourceAccessRuleSetId,
            UpdateOperation operation,
            IEnumerable<ResourceAccessRule> body)
        {
            using (this.telemetryClient.StartOperation<RequestTelemetry>(UpdateResourceAccessRuleSetResourceAccessRulesOperationId))
            {
                ITenant tenant = await this.TenantFromIdAsync(tenantId).ConfigureAwait(false);
                IResourceAccessRuleSetStore store = await this.permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(tenant).ConfigureAwait(false);

                ResourceAccessRuleSet ruleSet;

                try
                {
                    ruleSet = await store.GetAsync(resourceAccessRuleSetId).ConfigureAwait(false);
                }
                catch (ResourceAccessRuleSetNotFoundException)
                {
                    return this.NotFoundResult();
                }

                switch (operation)
                {
                    case UpdateOperation.Add:
                        ruleSet.Rules.AddRange(body);
                        break;
                    case UpdateOperation.Remove:
                        body.ForEach(rc => ruleSet.Rules.Remove(rc));
                        break;
                }

                await store.PersistAsync(ruleSet).ConfigureAwait(false);

                return this.CreatedResult();
            }
        }

        /// <summary>
        /// Handles a request to set a resource access rule set's resource access rules.
        /// </summary>
        /// <param name="tenantId">The id of the tenant.</param>
        /// <param name="resourceAccessRuleSetId">The resource access rule set ID.</param>
        /// <param name="body">The set of resource access rules to set.</param>
        /// <returns>The <see cref="OpenApiResult" />.</returns>
        [OperationId(SetResourceAccessRuleSetResourceAccessRulesOperationId)]
        public async Task<OpenApiResult> SetResourceAccessRuleSetResourceAccessRulesAsync(
            string tenantId,
            string resourceAccessRuleSetId,
            IEnumerable<ResourceAccessRule> body)
        {
            using (this.telemetryClient.StartOperation<RequestTelemetry>(SetResourceAccessRuleSetResourceAccessRulesOperationId))
            {
                ITenant tenant = await this.TenantFromIdAsync(tenantId).ConfigureAwait(false);
                IResourceAccessRuleSetStore store = await this.permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(tenant).ConfigureAwait(false);

                ResourceAccessRuleSet ruleSet;

                try
                {
                    ruleSet = await store.GetAsync(resourceAccessRuleSetId).ConfigureAwait(false);
                }
                catch (ResourceAccessRuleSetNotFoundException)
                {
                    return this.NotFoundResult();
                }

                ruleSet.Rules = body.ToList();

                await store.PersistAsync(ruleSet).ConfigureAwait(false);

                return this.OkResult();
            }
        }

        /// <summary>
        /// Gets the <see cref="Tenant"/> with the given id.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <returns>A task that produces the <see cref="Tenant"/> with the specified id.</returns>
        private ValueTask<ITenant> TenantFromIdAsync(string tenantId)
            => new ValueTask<ITenant>(this.tenantProvider.GetTenantAsync(tenantId));
    }
}
