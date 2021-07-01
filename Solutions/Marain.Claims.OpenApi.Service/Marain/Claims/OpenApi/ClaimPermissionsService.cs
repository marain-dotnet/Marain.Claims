// <copyright file="ClaimPermissionsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Marain.Claims.Storage;
    using Marain.Services.Tenancy;
    using Menes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     Handles claim permissions requests.
    /// </summary>
    [EmbeddedOpenApiDefinition("Marain.Claims.OpenApi.ClaimsServices.yaml")]
    public class ClaimPermissionsService : IOpenApiService
    {
        /// <summary>
        /// Uri template passed to <c>OpenApiClaimsServiceCollectionExtensions.AddRoleBasedOpenApiAccessControlWithPreemptiveExemptions</c>
        /// to distinguish between rules defining access control policy for the Claims service vs those for other services.
        /// </summary>
        public const string ClaimsResourceTemplate = "{tenantId}/marain/claims/";

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
        private readonly IMarainServicesTenancy marainServicesTenancy;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimPermissionsService"/> class.
        /// </summary>
        /// <param name="permissionsStoreFactory">Provides access to the permissions store.</param>
        /// <param name="marainServicesTenancy">The Marain services tenancy provider.</param>
        /// <param name="serializerSettingsProvider">The serializer settings provider.</param>
        public ClaimPermissionsService(
            IPermissionsStoreFactory permissionsStoreFactory,
            IMarainServicesTenancy marainServicesTenancy,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.permissionsStoreFactory = permissionsStoreFactory ?? throw new ArgumentNullException(nameof(permissionsStoreFactory));
            this.marainServicesTenancy = marainServicesTenancy ?? throw new ArgumentNullException(nameof(marainServicesTenancy));
            this.serializerSettingsProvider = serializerSettingsProvider ?? throw new ArgumentNullException(nameof(serializerSettingsProvider));
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

            (bool allRulesExist, OpenApiResult failureResult) = await this.CheckRuleSetsExist(tenant, body.ResourceAccessRuleSets);
            if (!allRulesExist)
            {
                return failureResult;
            }

            IClaimPermissionsStore claimPermissionsStore = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);

            try
            {
                ClaimPermissions result = await claimPermissionsStore.CreateAsync(body).ConfigureAwait(false);

                return this.OkResult(result, "application/json");
            }
            catch (InvalidOperationException)
            {
                var response = new JObject
                {
                    ["status"] = 400,
                    ["detail"] = "A ClaimPermissions with this ID has already been created",
                };
                return new OpenApiResult
                {
                    StatusCode = 400,
                    Results = { { "application/json", response } },
                };
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

            IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);
            try
            {
                ClaimPermissions claimPermissions = await store.GetAsync(claimPermissionsId).ConfigureAwait(false);
                return this.OkResult(claimPermissions, "application/json");
            }
            catch (ClaimPermissionsNotFoundException)
            {
                return this.NotFoundResult();
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

            IClaimPermissionsStore store = await this.permissionsStoreFactory.GetClaimPermissionsStoreAsync(tenant).ConfigureAwait(false);

            try
            {
                ClaimPermissions claimPermissions = await store.GetAsync(claimPermissionsId).ConfigureAwait(false);
                return this.OkResult(claimPermissions.AllResourceAccessRules, "application/json");
            }
            catch (ClaimPermissionsNotFoundException)
            {
                return this.NotFoundResult();
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

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

            var inputRules = body.ToList();
            if (inputRules.Distinct().Count() != inputRules.Count)
            {
                var response = new JObject
                {
                    ["status"] = 400,
                    ["detail"] = "Request contains duplicate rules",
                };
                return new OpenApiResult
                {
                    StatusCode = 400,
                    Results = { { "application/json", response } },
                };
            }

            var existingRules = new HashSet<ResourceAccessRule>(claimPermissions.ResourceAccessRules);
            var incomingRulesMatchingExistingRules = inputRules.Where(inputRule => existingRules.Contains(inputRule)).ToList();
            switch (operation)
            {
                case UpdateOperation.Add:
                    if (incomingRulesMatchingExistingRules.Count != 0)
                    {
                        string existingRulesText = string.Join(
                            ", ",
                            incomingRulesMatchingExistingRules.Select(FormatRuleForError));
                        var response = new JObject
                        {
                            ["status"] = 400,
                            ["detail"] = $"Request contains rules that are already present: {existingRulesText}",
                        };
                        return new OpenApiResult
                        {
                            StatusCode = 400,
                            Results = { { "application/json", response } },
                        };
                    }

                    claimPermissions.ResourceAccessRules.AddRange(body);
                    break;
                case UpdateOperation.Remove:
                    if (incomingRulesMatchingExistingRules.Count != inputRules.Count)
                    {
                        string wrongRulesText = string.Join(
                            ", ",
                            inputRules.Where(r => !existingRules.Contains(r)).Select(FormatRuleForError));
                        var response = new JObject
                        {
                            ["status"] = 400,
                            ["detail"] = $"Request contains rules that are not present: {wrongRulesText}",
                        };
                        return new OpenApiResult
                        {
                            StatusCode = 400,
                            Results = { { "application/json", response } },
                        };
                    }

                    body.ForEach(rc => claimPermissions.ResourceAccessRules.RemoveAll(r => r == rc));
                    break;
            }

            await store.UpdateAsync(claimPermissions).ConfigureAwait(false);

            return this.CreatedResult();

            static string FormatRuleForError(ResourceAccessRule rule) => $"'{rule.AccessType} {rule.Resource.Uri} ({rule.Resource.DisplayName}): {rule.Permission}'";
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

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

            await store.UpdateAsync(claimPermissions).ConfigureAwait(false);

            return this.OkResult();
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

            (bool allRulesExist, OpenApiResult failureResult) = await this.CheckRuleSetsExist(tenant, body);
            if (!allRulesExist)
            {
                return failureResult;
            }

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

            var inputRuleSets = body.ToList();
            if (inputRuleSets.Select(rs => rs.Id).Distinct().Count() != inputRuleSets.Count)
            {
                var response = new JObject
                {
                    ["status"] = 400,
                    ["detail"] = "Request contains duplicate ruleset ids",
                };
                return new OpenApiResult
                {
                    StatusCode = 400,
                    Results = { { "application/json", response } },
                };
            }

            var existingRuleIds = new HashSet<string>(claimPermissions.ResourceAccessRuleSets.Select(rs => rs.Id));
            var incomingRuleSetsMatchingExistingRuleSets = inputRuleSets.Where(inputRuleSet => existingRuleIds.Contains(inputRuleSet.Id)).ToList();
            switch (operation)
            {
                case UpdateOperation.Add:
                    if (incomingRuleSetsMatchingExistingRuleSets.Count != 0)
                    {
                        string existingRuleSetsText = string.Join(
                            ", ",
                            incomingRuleSetsMatchingExistingRuleSets.Select(rs => rs.Id));
                        var response = new JObject
                        {
                            ["status"] = 400,
                            ["detail"] = $"Request contains rulesets that are already present: {existingRuleSetsText}",
                        };
                        return new OpenApiResult
                        {
                            StatusCode = 400,
                            Results = { { "application/json", response } },
                        };
                    }

                    claimPermissions.ResourceAccessRuleSets.AddRange(body);
                    break;
                case UpdateOperation.Remove:
                    if (incomingRuleSetsMatchingExistingRuleSets.Count != inputRuleSets.Count)
                    {
                        string wrongRuleSetsText = string.Join(
                            ", ",
                            inputRuleSets.Where(rs => !existingRuleIds.Contains(rs.Id)).Select(rs => rs.Id));
                        var response = new JObject
                        {
                            ["status"] = 400,
                            ["detail"] = $"Request contains rulesets that are not present: {wrongRuleSetsText}",
                        };
                        return new OpenApiResult
                        {
                            StatusCode = 400,
                            Results = { { "application/json", response } },
                        };
                    }

                    body.ForEach(rc => claimPermissions.ResourceAccessRuleSets.RemoveAll(r => r.Id == rc.Id));
                    break;
            }

            await store.UpdateAsync(claimPermissions).ConfigureAwait(false);

            return this.CreatedResult();
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

            (bool allRulesExist, OpenApiResult failureResult) = await this.CheckRuleSetsExist(tenant, body);
            if (!allRulesExist)
            {
                return failureResult;
            }

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

            await store.UpdateAsync(claimPermissions).ConfigureAwait(false);

            return this.OkResult();
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

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

            return this.OkResult(result, "application/json");
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

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

            return this.OkResult(result, "application/json");
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

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

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
                    ("GET", "claimPermissions/**/*", "Read Claim Permissions"),
                    ("PUT", "claimPermissions/**/*", "Modify Claim Permissions"),
                    ("POST", "claimPermissions", "Create Claim Permissions"),
                    ("POST", "claimPermissions/**/*", "Add to Claim Permissions"),
                    ("GET", "resourceAccessRuleSet/**/*", "Read Resource Access Rules"),
                    ("PUT", "resourceAccessRuleSet/**/*", "Modify Resource Access Rules"),
                    ("POST", "resourceAccessRuleSet", "Create Resource Access Rules"),
                    ("POST", "resourceAccessRuleSet/**/*", "Add to Resource Access Rules"),
            };

            string prefix = ClaimsResourceTemplate.Replace("{tenantId}", context.CurrentTenantId);

            var ruleSet = new ResourceAccessRuleSet
            {
                Id = "marainClaimsAdministrator",
                DisplayName = "Claims Administrator Permissions",
                Rules = ruleData
                    .Select(rule =>
                        new ResourceAccessRule(
                        rule.accessType,
                        new Resource(new Uri(prefix + rule.resourceUri, UriKind.Relative), rule.displayName),
                        Permission.Allow))
                    .ToList(),
            };

            ruleSet = await ruleSetStore.PersistAsync(ruleSet).ConfigureAwait(false);

            var rulesetByIdOnlyForClaimPermissionsPersistence = new ResourceAccessRuleSet { Id = ruleSet.Id };

            string administratorPrincipalObjectId;
            if (body.TryGetValue("administratorPrincipalObjectId", out JToken administratorPrincipalObjectIdJToken))
            {
                administratorPrincipalObjectId = administratorPrincipalObjectIdJToken.Value<string>();
            }
            else
            {
                administratorPrincipalObjectId =
                    (context.CurrentPrincipal.FindFirst(ClaimTypes.Oid) ?? context.CurrentPrincipal.FindFirst(ClaimTypes.ObjectIdentifier)).Value;
            }

            var permissions = new ClaimPermissions
            {
                Id = administratorPrincipalObjectId,
                ResourceAccessRuleSets = new[] { rulesetByIdOnlyForClaimPermissionsPersistence },
            };
            await permissionsStore.CreateAsync(permissions).ConfigureAwait(false);
            return this.OkResult();
        }

        private async Task<(bool, OpenApiResult)> CheckRuleSetsExist(
            ITenant tenant,
            IEnumerable<ResourceAccessRuleSet> ruleSets)
        {
            IResourceAccessRuleSetStore ruleSetStore = await this.permissionsStoreFactory.GetResourceAccessRuleSetStoreAsync(tenant).ConfigureAwait(false);
            IEnumerable<Task<(string Id, bool Exists)>> checkRuleSetsExistTasks = ruleSets.Select(async rs =>
            {
                try
                {
                    await ruleSetStore.GetAsync(rs.Id);
                    return (rs.Id, true);
                }
                catch (ResourceAccessRuleSetNotFoundException)
                {
                    return (rs.Id, false);
                }
            });
            (string Id, bool Exists)[] checkRuleSetsExistResults = await Task.WhenAll(checkRuleSetsExistTasks).ConfigureAwait(false);
            var missingRuleSets = checkRuleSetsExistResults.Where(r => !r.Exists).Select(r => r.Id).ToList();
            if (!missingRuleSets.IsEmpty())
            {
                var response = new JObject
                {
                    ["status"] = 400,
                    ["detail"] = $"These rule set ids do not exist: {string.Join(", ", missingRuleSets)}",
                };
                return (false, new OpenApiResult
                {
                    StatusCode = 400,
                    Results = { { "application/json", response } },
                });
            }

            return (true, null);
        }
    }
}
