// <copyright file="LocalClaimsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using Menes;
    using Microsoft.Rest;
    using Newtonsoft.Json;

    /// <summary>
    /// In-process implementation of <see cref="IClaimsService"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>RoleBasedOpenApiAccessControlPolicy</c> works by asking the claims service to evaluate
    /// permissions. We use this same policy to secure access to most of the claims service itself,
    /// but to avoid the unnecessary overhead of calling into ourselves over the network, we use
    /// this implementation of the proxy type for the service which invokes the relevant service
    /// method directly.
    /// </para>
    /// <para>
    /// Since the only service operation that <c>RoleBasedOpenApiAccessControlPolicy</c> should be
    /// invoking is <see cref="IClaimsService.GetClaimPermissionsPermissionBatchWithHttpMessagesAsync(string, System.Collections.Generic.IList{Client.Models.ClaimPermissionsBatchRequestItemWithPostExample}, Dictionary{string, List{string}}, CancellationToken)"/>,
    /// almost all of this throws <c>NotSupportedException</c>.
    /// </para>
    /// <para>
    /// This is a slightly messy class because on the side of the service it invokes, it must
    /// deal with the <see cref="Marain.Claims.PermissionResult"/> type defined in the
    /// <c>Endjin.Claims.Abstraction</c> component. However, because the access control policy
    /// expects to consume the Claims service remotely, it needs to use the
    /// <see cref="Marain.Claims.Client.Models.PermissionResult"/> type, a DTO generated from the
    /// Claims service's Swagger by AutoRest, and defined in the <c>Endjin.Claims.Client</c>
    /// component. This confusing state of affairs is unavoidable because the entire point of this
    /// class is to enable a component that normally uses the Claims service remotely to use the
    /// service implementation directly in-process. This class suffers so you don't have to.
    /// </para>
    /// </remarks>
    public class LocalClaimsService : IClaimsService
    {
        private readonly IClaimPermissionsEvaluator service;

        /// <summary>
        /// Create a <see cref="LocalClaimsService"/>.
        /// </summary>
        /// <param name="service">
        /// The service that will handle the requests.
        /// </param>
        public LocalClaimsService(IClaimPermissionsEvaluator service)
        {
            this.service = service;
        }

        /// <inheritdoc />
        Uri IClaimsService.BaseUri { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        /// <inheritdoc />
        JsonSerializerSettings IClaimsService.SerializationSettings => throw new NotSupportedException();

        /// <inheritdoc />
        JsonSerializerSettings IClaimsService.DeserializationSettings => throw new NotSupportedException();

        /// <inheritdoc />
        ServiceClientCredentials IClaimsService.Credentials => throw new NotSupportedException();

        /// <inheritdoc />
        public Task<HttpOperationResponse<object>> GetClaimPermissionsPermissionWithHttpMessagesAsync(
            string claimPermissionsId,
            string xEndjinTenant,
            string resourceUri,
            string accessType,
            Dictionary<string, List<string>> customHeaders = null,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        /// <inheritdoc />
        public async Task<HttpOperationResponse<object>> GetClaimPermissionsPermissionBatchWithHttpMessagesAsync(string xEndjinTenant, IList<ClaimPermissionsBatchRequestItemWithPostExample> body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            ClaimPermissionsBatchRequestItem[] requests = body.Select(x => new ClaimPermissionsBatchRequestItem { ClaimPermissionsId = x.ClaimPermissionsId, ResourceAccessType = x.ResourceAccessType, ResourceUri = x.ResourceUri }).ToArray();
            OpenApiResult permissionResult = await this.service.GetClaimPermissionsPermissionAsync(xEndjinTenant, requests).ConfigureAwait(false);
            var httpResult = new HttpOperationResponse<object>
            {
                Response = new HttpResponseMessage((HttpStatusCode)permissionResult.StatusCode),
            };

            if (permissionResult.StatusCode == 200)
            {
                var results = (IList<ClaimPermissionsBatchResponseItem>)permissionResult.Results["application/json"];
                httpResult.Body = results.Select(x => new ClaimPermissionsBatchResponseItemWithExample
                {
                    ClaimPermissionsId = x.ClaimPermissionsId,
                    Permission = x.Permission,
                    ResourceAccessType = x.ResourceAccessType,
                    ResourceUri = x.ResourceUri,
                    ResponseCode = x.ResponseCode,
                }).ToList();
            }

            return httpResult;
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<ClaimPermissionsWithGetExample>> IClaimsService.CreateClaimPermissionsWithHttpMessagesAsync(string xEndjinTenant, ClaimPermissionsWithPostExample body, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<ClaimPermissionsWithGetExample>> IClaimsService.GetClaimPermissionsWithHttpMessagesAsync(string claimPermissionsId, string xEndjinTenant, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<IList<Marain.Claims.Client.Models.ResourceAccessRule>>> IClaimsService.GetClaimPermissionsResourceAccessRulesWithHttpMessagesAsync(string claimPermissionsId, string xEndjinTenant, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<Marain.Claims.Client.Models.ProblemDetails>> IClaimsService.UpdateClaimPermissionsResourceAccessRulesWithHttpMessagesAsync(string xEndjinTenant, string claimPermissionsId, string operation, IList<Marain.Claims.Client.Models.ResourceAccessRule> body, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<Marain.Claims.Client.Models.ProblemDetails>> IClaimsService.SetClaimPermissionsResourceAccessRulesWithHttpMessagesAsync(string xEndjinTenant, string claimPermissionsId, IList<Marain.Claims.Client.Models.ResourceAccessRule> body, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<Marain.Claims.Client.Models.ProblemDetails>> IClaimsService.UpdateClaimPermissionsResourceAccessRuleSetsWithHttpMessagesAsync(string xEndjinTenant, string claimPermissionsId, string operation, IList<Marain.Claims.Client.Models.ResourceAccessRuleSetId> body, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<Marain.Claims.Client.Models.ProblemDetails>> IClaimsService.SetClaimPermissionsResourceAccessRuleSetsWithHttpMessagesAsync(string xEndjinTenant, string claimPermissionsId, IList<ResourceAccessRuleSetId> body, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<object>> IClaimsService.CreateResourceAccessRuleSetWithHttpMessagesAsync(string xEndjinTenant, ResourceAccessRuleSetWithPostExample body, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<ResourceAccessRuleSetWithGetExample>> IClaimsService.GetResourceAccessRuleSetWithHttpMessagesAsync(string resourceAccessRuleSetId, string xEndjinTenant, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<Marain.Claims.Client.Models.ProblemDetails>> IClaimsService.UpdateResourceAccessRuleSetResourceAccessRulesWithHttpMessagesAsync(string xEndjinTenant, string resourceAccessRuleSetId, string operation, IList<Marain.Claims.Client.Models.ResourceAccessRule> body, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<Marain.Claims.Client.Models.ProblemDetails>> IClaimsService.SetResourceAccessRuleSetResourceAccessRulesWithHttpMessagesAsync(string xEndjinTenant, string resourceAccessRuleSetId, IList<Marain.Claims.Client.Models.ResourceAccessRule> body, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<object>> IClaimsService.GetSwaggerWithHttpMessagesAsync(Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        Task<HttpOperationResponse<ProblemDetails>> IClaimsService.InitializeTenantWithHttpMessagesAsync(string xEndjinTenant, Body body, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            throw new NotSupportedException();
        }
    }
}
