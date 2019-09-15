// <copyright file="ClaimPermissionsBatchResponseItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi
{
    using Newtonsoft.Json;

    /// <summary>
    /// The response for a batched claims request.
    /// </summary>
    public class ClaimPermissionsBatchResponseItem : ClaimPermissionsBatchRequestItem
    {
        /// <summary>
        /// Initializes a new instance of the ClaimPermissionsBatchResponseItem
        /// class.
        /// </summary>
        public ClaimPermissionsBatchResponseItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ClaimPermissionsBatchResponseItem
        /// class.
        /// </summary>
        /// <param name="claimPermissionsId">The id of the claim permissions.</param>
        /// <param name="resourceUri">The uri of the resource for which to validate claims.</param>
        /// <param name="resourceAccessType">The type of access required to the resource.</param>
        /// <param name="responseCode">The response code.</param>
        /// <param name="permission">Possible values include: 'allow',
        /// 'deny'.</param>
        public ClaimPermissionsBatchResponseItem(string claimPermissionsId, string resourceUri, string resourceAccessType, int? responseCode, string permission)
            : base(claimPermissionsId, resourceUri, resourceAccessType)
        {
            this.ResponseCode = responseCode;
            this.Permission = permission;
        }

        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        [JsonProperty("responseCode")]
        public int? ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'allow', 'deny'.
        /// </summary>
        [JsonProperty("permission")]
        public string Permission { get; set; }
    }
}
