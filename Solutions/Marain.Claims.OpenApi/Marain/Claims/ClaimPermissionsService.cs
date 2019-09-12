// <copyright file="ClaimPermissionsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Marain.Claims.Client.Models;
    using Marain.Claims.Storage;
    using Menes;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     Handles claim permissions requests.
    /// </summary>
    public class ClaimPermissionsService : IOpenApiService
    {
        /// <summary>
        /// Prefix passed to <c>OpenApiClaimsServiceCollectionExtensions.AddRoleBasedOpenApiAccessControlWithPreemptiveExemptions</c>
        /// to distinguish between rules defining access control policy for the Claims service vs those for other services.
        /// </summary>
        public const string ClaimsResourcePrefix = "marain/claims/";

        /// <summary>
        /// Open API operation ID for endpoint that evaluates permissions for a particular claims permission.
        /// </summary>
        public const string GetClaimPermissionsPermissionOperationId = "getClaimPermissionsPermission";

        /// <summary>
        /// Open API operation ID for endpoint that evaluates permissions for a batch of claims permissions.
        /// </summary>
        public const string GetClaimPermissionsPermissionBatchOperationId = "getClaimPermissionsPermissionBatch";

        /// <summary>
        /// Open API operation ID for tenant bootstrapping endpoint.
        /// </summary>
        public const string InitializeTenantOperationId = "initializeTenant";
        private const string CreateClaimPermissionsOperationId = "createClaimPermissions";
        private const string GetClaimPermissionsOperationId = "getClaimPermissions";
        private const string GetClaimPermissionsResourceAccessRulesOperationId = "getClaimPermissionsResourceAccessRules";
        private const string UpdateClaimPermissionsResourceAccessRulesOperationId = "updateClaimPermissionsResourceAccessRules";
        private const string SetClaimPermissionsResourceAccessRulesOperationId = "setClaimPermissionsResourceAccessRules";
        private const string UpdateClaimPermissionsResourceAccessRuleSetsOperationId = "updateClaimPermissionsResourceAccessRuleSets";
        private const string SetClaimPermissionsResourceAccessRuleSetsOperationId = "setClaimPermissionsResourceAccessRuleSets";

        private readonly IPermissionsStoreFactory permissionsStoreFactory;
        private readonly ITenantProvider tenantProvider;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimPermissionsService"/> class.
        /// </summary>
        /// <param name="permissionsStoreFactory">Provides access to the permissions store.</param>
        /// <param name="tenantProvider">The tenant provider.</param>
        /// <param name="serializerSettingsProvider">The serializer settings provider.</param>
        /// <param name="telemetryClient">A <see cref="TelemetryClient"/> to log telemetry.</param>
        public ClaimPermissionsService(
            IPermissionsStoreFactory permissionsStoreFactory,
            ITenantProvider tenantProvider,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            TelemetryClient telemetryClient)
        {
            this.permissionsStoreFactory = permissionsStoreFactory ?? throw new ArgumentNullException(nameof(permissionsStoreFactory));
            this.tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
            this.serializerSettingsProvider = serializerSettingsProvider ?? throw new ArgumentNullException(nameof(serializerSettingsProvider));
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        }

        /// <summary>
        /// Handles a request to create a new claims permissions definition.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="body">The Claims Permission to create.</param>
        /// <returns>The <see cref="ClaimPermissions"/>.</returns>
        [OperationId(CreateClaimPermissionsOperationId)]
        public async Task<OpenApiResult> CreateClaimPermissionsAsync(
            IOpenApiContext context,
            ClaimPermissions body)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(CreateClaimPermissionsOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
                IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);
                ClaimPermissions result = await store.PersistAsync(body).ConfigureAwait(false);
                return this.OkResult(result);
            }
        }

        /// <summary>
        /// Handles a request to get a claims permissions.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="claimPermissionsId">The claim permissions ID.</param>
        /// <returns>The <see cref="ClaimPermissions"/>.</returns>
        [OperationId(GetClaimPermissionsOperationId)]
        public async Task<OpenApiResult> GetClaimPermissionAsync(
            IOpenApiContext context,
            string claimPermissionsId)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (claimPermissionsId is null)
            {
                throw new ArgumentNullException(nameof(claimPermissionsId));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(GetClaimPermissionsOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
                IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);
                try
                {
                    ClaimPermissions claimPermissions = await store.GetAsync(claimPermissionsId).ConfigureAwait(false);
                    return this.OkResult(claimPermissions);
                }
                catch (ClaimPermissionsNotFoundException)
                {
                    return this.NotFoundResult();
                }
            }
        }

        /// <summary>
        /// Handles a request to get a claim permission's resource access rules.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="claimPermissionsId">The claim permissions ID.</param>
        /// <returns>The <see cref="IEnumerable{ResourceAccessRule}"/>.</returns>
        [OperationId(GetClaimPermissionsResourceAccessRulesOperationId)]
        public async Task<OpenApiResult> GetClaimPermissionResourceAccessRulesAsync(
            IOpenApiContext context,
            string claimPermissionsId)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (claimPermissionsId is null)
            {
                throw new ArgumentNullException(nameof(claimPermissionsId));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(GetClaimPermissionsResourceAccessRulesOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
                IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);

                try
                {
                    ClaimPermissions claimPermissions = await store.GetAsync(claimPermissionsId).ConfigureAwait(false);
                    return this.OkResult(claimPermissions.AllResourceAccessRules);
                }
                catch (ClaimPermissionsNotFoundException)
                {
                    return this.NotFoundResult();
                }
            }
        }

        /// <summary>
        /// Handles a request to update a claim permissions' resource access rules.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="claimPermissionsId">The claim permissions ID.</param>
        /// <param name="operation">Add or remove operation.</param>
        /// <param name="body">The set of resource access rules to add/remove.</param>
        /// <returns>The <see cref="OpenApiResult" />.</returns>
        [OperationId(UpdateClaimPermissionsResourceAccessRulesOperationId)]
        public async Task<OpenApiResult> UpdateClaimPermissionsResourceAccessRulesAsync(
            IOpenApiContext context,
            string claimPermissionsId,
            UpdateOperation operation,
            IEnumerable<ResourceAccessRule> body)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (claimPermissionsId is null)
            {
                throw new ArgumentNullException(nameof(claimPermissionsId));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(UpdateClaimPermissionsResourceAccessRulesOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
                IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);

                ClaimPermissions claimPermissions;

                try
                {
                    claimPermissions = await store.GetAsync(claimPermissionsId).ConfigureAwait(false);
                }
                catch (ClaimPermissionsNotFoundException)
                {
                    return this.NotFoundResult();
                }

                switch (operation)
                {
                    case UpdateOperation.Add:
                        claimPermissions.ResourceAccessRules.AddRange(body);
                        break;
                    case UpdateOperation.Remove:
                        body.ForEach(rc => claimPermissions.ResourceAccessRules.Remove(rc));
                        break;
                }

                await store.PersistAsync(claimPermissions).ConfigureAwait(false);

                return this.CreatedResult();
            }
        }

        /// <summary>
        /// Handles a request to set a claim permissions' resource access rules.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="claimPermissionsId">The claim permissions ID.</param>
        /// <param name="body">The set of resource access rules to set.</param>
        /// <returns>The <see cref="OpenApiResult" />.</returns>
        [OperationId(SetClaimPermissionsResourceAccessRulesOperationId)]
        public async Task<OpenApiResult> SetClaimPermissionsResourceAccessRulesAsync(
            IOpenApiContext context,
            string claimPermissionsId,
            IEnumerable<ResourceAccessRule> body)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (claimPermissionsId is null)
            {
                throw new ArgumentNullException(nameof(claimPermissionsId));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(SetClaimPermissionsResourceAccessRulesOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
                IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);

                ClaimPermissions claimPermissions;

                try
                {
                    claimPermissions = await store.GetAsync(claimPermissionsId).ConfigureAwait(false);
                }
                catch (ClaimPermissionsNotFoundException)
                {
                    return this.NotFoundResult();
                }

                claimPermissions.ResourceAccessRules = body.ToList();

                await store.PersistAsync(claimPermissions).ConfigureAwait(false);

                return this.OkResult();
            }
        }

        /// <summary>
        /// Handles a request to update a claim permissions' resource access rule sets.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="claimPermissionsId">The claim permissions ID.</param>
        /// <param name="operation">Add or remove operation.</param>
        /// <param name="body">The set of resource access rules to add/remove.</param>
        /// <returns>The <see cref="OpenApiResult" />.</returns>
        [OperationId(UpdateClaimPermissionsResourceAccessRuleSetsOperationId)]
        public async Task<OpenApiResult> UpdateClaimPermissionsResourceAccessRuleSetsAsync(
            IOpenApiContext context,
            string claimPermissionsId,
            UpdateOperation operation,
            IEnumerable<ResourceAccessRuleSet> body)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (claimPermissionsId is null)
            {
                throw new ArgumentNullException(nameof(claimPermissionsId));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(UpdateClaimPermissionsResourceAccessRuleSetsOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

                IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);

                ClaimPermissions claimPermissions;

                try
                {
                    claimPermissions = await store.GetAsync(claimPermissionsId).ConfigureAwait(false);
                }
                catch (ClaimPermissionsNotFoundException)
                {
                    return this.NotFoundResult();
                }

                switch (operation)
                {
                    case UpdateOperation.Add:
                        claimPermissions.ResourceAccessRuleSets.AddRange(body);
                        break;
                    case UpdateOperation.Remove:
                        body.ForEach(rc => claimPermissions.ResourceAccessRuleSets.Remove(rc));
                        break;
                }

                await store.PersistAsync(claimPermissions).ConfigureAwait(false);

                return this.CreatedResult();
            }
        }

        /// <summary>
        /// Handles a request to set a claim permissions' resource access rule sets.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="claimPermissionsId">The claim permissions ID.</param>
        /// <param name="body">The set of resource access rules to set.</param>
        /// <returns>The <see cref="OpenApiResult" />.</returns>
        [OperationId(SetClaimPermissionsResourceAccessRuleSetsOperationId)]
        public async Task<OpenApiResult> SetClaimPermissionsResourceAccessRuleSetsAsync(
            IOpenApiContext context,
            string claimPermissionsId,
            IEnumerable<ResourceAccessRuleSet> body)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (claimPermissionsId is null)
            {
                throw new ArgumentNullException(nameof(claimPermissionsId));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(SetClaimPermissionsResourceAccessRuleSetsOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

                IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);

                ClaimPermissions claimPermissions;

                try
                {
                    claimPermissions = await store.GetAsync(claimPermissionsId).ConfigureAwait(false);
                }
                catch (ClaimPermissionsNotFoundException)
                {
                    return this.NotFoundResult();
                }

                claimPermissions.ResourceAccessRuleSets = body.ToList();

                await store.PersistAsync(claimPermissions).ConfigureAwait(false);

                return this.OkResult();
            }
        }

        /// <summary>
        /// Handles a request to get a permission result for a claim permissions.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="claimPermissionsId">The claim permissions ID.</param>
        /// <param name="resourceUri">The target resource URI.</param>
        /// <param name="accessType">The target access type.</param>
        /// <returns>
        /// A task that produces the <see cref="PermissionResult"/> wrapped as an <see cref="OpenApiResult"/>.
        /// </returns>
        [OperationId(GetClaimPermissionsPermissionOperationId)]
        public async Task<OpenApiResult> GetClaimPermissionsPermissionAsync(
            IOpenApiContext context,
            string claimPermissionsId,
            string resourceUri,
            string accessType)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (claimPermissionsId is null)
            {
                throw new ArgumentNullException(nameof(claimPermissionsId));
            }

            if (resourceUri is null)
            {
                throw new ArgumentNullException(nameof(resourceUri));
            }

            if (accessType is null)
            {
                throw new ArgumentNullException(nameof(accessType));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(GetClaimPermissionsPermissionOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

                IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);

                ClaimPermissions claimPermissions;

                try
                {
                    claimPermissions = await store.GetAsync(claimPermissionsId).ConfigureAwait(false);
                }
                catch (ClaimPermissionsNotFoundException)
                {
                    return this.NotFoundResult();
                }

                var result =
                    new PermissionResult
                    {
                        Permission = claimPermissions.HasPermissionFor(new Uri(resourceUri, UriKind.RelativeOrAbsolute), accessType) ? Permission.Allow : Permission.Deny,
                    };

                return this.OkResult(result);
            }
        }

        /// <summary>
        /// Handles a request to get permission results for a batch of claim permissions.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="body">The list of claims permissions to get results for.</param>
        /// <returns>
        /// A task that produces the list of <see cref="ClaimPermissionsBatchResponseItem"/>
        /// wrapped as an <see cref="OpenApiResult"/>.
        /// </returns>
        [OperationId(GetClaimPermissionsPermissionBatchOperationId)]
        public async Task<OpenApiResult> GetClaimPermissionsPermissionBatchAsync(
            IOpenApiContext context,
            ClaimPermissionsBatchRequestItem[] body)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(GetClaimPermissionsPermissionOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

                IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);

                ClaimPermissionsCollection claimPermissions = await store.GetBatchAsync(body.Select(x => x.ClaimPermissionsId)).ConfigureAwait(false);

                var serializer = JsonSerializer.Create(this.serializerSettingsProvider.Instance);

                ClaimPermissionsBatchResponseItem[] result = body.Select(x =>
                {
                    var responseItem = new ClaimPermissionsBatchResponseItem
                    {
                        ClaimPermissionsId = x.ClaimPermissionsId,
                        ResourceUri = x.ResourceUri,
                        ResourceAccessType = x.ResourceAccessType,
                    };

                    ClaimPermissions claimPermission = claimPermissions.Permissions.FirstOrDefault(c => c.Id == x.ClaimPermissionsId);
                    Permission permission = Permission.Deny;

                    if (claimPermission == null)
                    {
                        responseItem.ResponseCode = 404;
                    }
                    else
                    {
                        responseItem.ResponseCode = 200;
                        permission = claimPermission.HasPermissionFor(new Uri(x.ResourceUri, UriKind.RelativeOrAbsolute), x.ResourceAccessType) ? Permission.Allow : Permission.Deny;
                    }

                    // Use the serializer to create a JToken from the permission value, then use this to populate
                    // the Permission property of the response. We can't just do a ToString because the serializer
                    // can be told how to format enum values and may be set to serialize them as lower case. Doing
                    // this means that we can be sure this value is treated in a consistent way.
                    var permissionToken = JToken.FromObject(permission, serializer);
                    responseItem.Permission = permissionToken.Value<string>();

                    return responseItem;
                }).ToArray();

                return this.OkResult(result);
            }
        }

        /// <summary>
        /// Handles a request to create the initial 'bootstrap' permissions for the Claims service.
        /// </summary>
        /// <param name="context">Provides access to information about the client.</param>
        /// <param name="body">The Claims Permission to create.</param>
        /// <returns>The <see cref="ClaimPermissions"/>.</returns>
        [OperationId(InitializeTenantOperationId)]
        public async Task<OpenApiResult> BootstrapTenantAsync(
            IOpenApiContext context,
            JObject body)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            using (this.telemetryClient.StartOperation<RequestTelemetry>(InitializeTenantOperationId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
                IClaimPermissionsStore permissionsStore = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);
                IResourceAccessRuleSetStore ruleSetStore = await this.permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(tenant).ConfigureAwait(false);

                bool alreadyInitialized = await permissionsStore.AnyPermissions().ConfigureAwait(false);
                if (alreadyInitialized)
                {
                    var response = new JObject
                    {
                        ["status"] = 400,
                        ["detail"] = "Tenant already initialized",
                    };
                    return new OpenApiResult
                    {
                        StatusCode = 400,
                        Results = { { "application/json", response } },
                    };
                }

                (string accessType, string resourceUri, string displayName)[] ruleData =
                {
                    ("GET", "api/{0}/claimPermissions/**/*", "Read Claim Permissions"),
                    ("PUT", "api/{0}/claimPermissions/**/*", "Modify Claim Permissions"),
                    ("POST", "api/{0}/claimPermissions", "Create Claim Permissions"),
                    ("POST", "api/{0}/claimPermissions/**/*", "Add to Claim Permissions"),
                    ("GET", "api/{0}/resourceAccessRuleSet/**/*", "Read Resource Access Rules"),
                    ("PUT", "api/{0}/resourceAccessRuleSet/**/*", "Modify Resource Access Rules"),
                    ("POST", "api/{0}/resourceAccessRuleSet", "Create Resource Access Rules"),
                    ("POST", "api/{0}/resourceAccessRuleSet/**/*", "Add to Resource Access Rules"),
                };
                var ruleSet = new ResourceAccessRuleSet
                {
                    Id = "marainClaimsAdministrator",
                    DisplayName = "Claims Administrator Permissions",
                    Rules = ruleData
                        .Select(rule =>
                            new ResourceAccessRule(
                            rule.accessType,
                            new Resource(new Uri(ClaimsResourcePrefix + string.Format(rule.resourceUri, context.CurrentTenantId), UriKind.Relative), rule.displayName),
                            Permission.Allow))
                        .ToList(),
                };
                await ruleSetStore.PersistAsync(ruleSet).ConfigureAwait(false);

                var rulesetByIdOnlyForClaimPermissionsPersistence = new ResourceAccessRuleSet { Id = ruleSet.Id };
                var permissions = new ClaimPermissions
                {
                    Id = body["administratorRoleClaimValue"].Value<string>(),
                    ResourceAccessRuleSets = new[] { rulesetByIdOnlyForClaimPermissionsPersistence },
                };
                await permissionsStore.PersistAsync(permissions).ConfigureAwait(false);
                return this.OkResult();
            }
        }
    }
}
