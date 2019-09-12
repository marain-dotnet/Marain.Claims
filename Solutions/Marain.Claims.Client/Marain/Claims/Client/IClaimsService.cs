// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Marain.Claims.Client
{
    using Microsoft.Rest;
    using Models;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    public partial interface IClaimsService : System.IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        JsonSerializerSettings DeserializationSettings { get; }

        /// <summary>
        /// Subscription credentials which uniquely identify client
        /// subscription.
        /// </summary>
        ServiceClientCredentials Credentials { get; }


        /// <summary>
        /// Create a Claim Permissions
        /// </summary>
        /// <remarks>
        /// Creates a permissions definition for a claim
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ClaimPermissionsWithGetExample>> CreateClaimPermissionsWithHttpMessagesAsync(string xEndjinTenant, ClaimPermissionsWithPostExample body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get a claim permissions
        /// </summary>
        /// <remarks>
        /// Gets a claim permissions by ID
        /// </remarks>
        /// <param name='claimPermissionsId'>
        /// An identifier uniquely associated with a claim permissions
        /// </param>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ClaimPermissionsWithGetExample>> GetClaimPermissionsWithHttpMessagesAsync(string claimPermissionsId, string xEndjinTenant, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all resource access rules for a claim permissions
        /// </summary>
        /// <remarks>
        /// Gets all resource access rules for a claim permissions, combining
        /// direct resource access rules and resource access rules from
        /// resource access rules sets
        /// </remarks>
        /// <param name='claimPermissionsId'>
        /// An identifier uniquely associated with a claim permissions
        /// </param>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<IList<ResourceAccessRule>>> GetClaimPermissionsResourceAccessRulesWithHttpMessagesAsync(string claimPermissionsId, string xEndjinTenant, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds/removes resource access rules to/from a claim permissions
        /// </summary>
        /// <remarks>
        /// Adds/removes resource access rules to/from a claims permission by
        /// ID
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='claimPermissionsId'>
        /// An identifier uniquely associated with a claim permissions
        /// </param>
        /// <param name='operation'>
        /// Possible values include: 'add', 'remove'
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ProblemDetails>> UpdateClaimPermissionsResourceAccessRulesWithHttpMessagesAsync(string xEndjinTenant, string claimPermissionsId, string operation, IList<ResourceAccessRule> body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Set resource access rules for a claim permissions
        /// </summary>
        /// <remarks>
        /// Sets resource access rules for a claim permissions by ID
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='claimPermissionsId'>
        /// An identifier uniquely associated with a claim permissions
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ProblemDetails>> SetClaimPermissionsResourceAccessRulesWithHttpMessagesAsync(string xEndjinTenant, string claimPermissionsId, IList<ResourceAccessRule> body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds/removes resource access rule sets to/from a claim permissions
        /// </summary>
        /// <remarks>
        /// Adds/removes resource access rule sets to/from a claim permissions
        /// by ID
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='claimPermissionsId'>
        /// An identifier uniquely associated with a claim permissions
        /// </param>
        /// <param name='operation'>
        /// Possible values include: 'add', 'remove'
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ProblemDetails>> UpdateClaimPermissionsResourceAccessRuleSetsWithHttpMessagesAsync(string xEndjinTenant, string claimPermissionsId, string operation, IList<ResourceAccessRuleSetId> body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Set resource access rule sets for a claim permissions
        /// </summary>
        /// <remarks>
        /// Sets resource access rule sets for a claim permissions by ID
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='claimPermissionsId'>
        /// An identifier uniquely associated with a claim permissions
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ProblemDetails>> SetClaimPermissionsResourceAccessRuleSetsWithHttpMessagesAsync(string xEndjinTenant, string claimPermissionsId, IList<ResourceAccessRuleSetId> body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a permission result for a claim permissions for a target
        /// resource and access type
        /// </summary>
        /// <remarks>
        /// Gets a permission result for a claim permissions for a target
        /// resource and access type
        /// </remarks>
        /// <param name='claimPermissionsId'>
        /// An identifier uniquely associated with a claim permissions
        /// </param>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='resourceUri'>
        /// </param>
        /// <param name='accessType'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<object>> GetClaimPermissionsPermissionWithHttpMessagesAsync(string claimPermissionsId, string xEndjinTenant, string resourceUri, string accessType, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets permission results for a set of target resources and access
        /// types
        /// </summary>
        /// <remarks>
        /// Gets permission results for a set of target resources and access
        /// types
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<object>> GetClaimPermissionsPermissionBatchWithHttpMessagesAsync(string xEndjinTenant, IList<ClaimPermissionsBatchRequestItemWithPostExample> body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a resource access rule set
        /// </summary>
        /// <remarks>
        /// Creates a resource access rule set
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<object>> CreateResourceAccessRuleSetWithHttpMessagesAsync(string xEndjinTenant, ResourceAccessRuleSetWithPostExample body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get a resource access rule set
        /// </summary>
        /// <remarks>
        /// Gets a resource access rule set by ID
        /// </remarks>
        /// <param name='resourceAccessRuleSetId'>
        /// An identifier uniquely associated with a resource access rule set
        /// </param>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ResourceAccessRuleSetWithGetExample>> GetResourceAccessRuleSetWithHttpMessagesAsync(string resourceAccessRuleSetId, string xEndjinTenant, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds/removes resource access rules to/from a resource access rule
        /// set
        /// </summary>
        /// <remarks>
        /// Adds/removes resource access rules to/from a resource access rule
        /// set by ID
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='resourceAccessRuleSetId'>
        /// An identifier uniquely associated with a resource access rule set
        /// </param>
        /// <param name='operation'>
        /// Possible values include: 'add', 'remove'
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ProblemDetails>> UpdateResourceAccessRuleSetResourceAccessRulesWithHttpMessagesAsync(string xEndjinTenant, string resourceAccessRuleSetId, string operation, IList<ResourceAccessRule> body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Set resource access rules for a resource access rule set
        /// </summary>
        /// <remarks>
        /// Sets resource access rules for a resource access rule set by ID
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='resourceAccessRuleSetId'>
        /// An identifier uniquely associated with a resource access rule set
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ProblemDetails>> SetResourceAccessRuleSetResourceAccessRulesWithHttpMessagesAsync(string xEndjinTenant, string resourceAccessRuleSetId, IList<ResourceAccessRule> body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Set up the initial administrator permissions for a tenant
        /// </summary>
        /// <remarks>
        /// Creates a resource access rule set providing access to all
        /// endpoints and a Claim Permission granting that access to the
        /// specified role
        /// </remarks>
        /// <param name='xEndjinTenant'>
        /// The tenant within which the request should operate
        /// </param>
        /// <param name='body'>
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<ProblemDetails>> InitializeTenantWithHttpMessagesAsync(string xEndjinTenant, Body body, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// View swagger definition for this API
        /// </summary>
        /// <remarks>
        /// View swagger definition for this API
        /// </remarks>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<object>> GetSwaggerWithHttpMessagesAsync(Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

    }
}
