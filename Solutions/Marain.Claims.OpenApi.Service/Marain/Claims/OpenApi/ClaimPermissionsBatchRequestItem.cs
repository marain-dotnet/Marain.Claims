// <copyright file="ClaimPermissionsBatchRequestItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi
{
    using Newtonsoft.Json;

    /// <summary>
    /// An item in a batch of requests for claim permissions.
    /// </summary>
    public class ClaimPermissionsBatchRequestItem
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.claims.claimpermissionsbatchrequestitem";

        /// <summary>
        /// Initializes a new instance of the ClaimPermissionsBatchRequestItem
        /// class.
        /// </summary>
        public ClaimPermissionsBatchRequestItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ClaimPermissionsBatchRequestItem
        /// class.
        /// </summary>
        /// <param name="claimPermissionsId">The id of the claim permissions.</param>
        /// <param name="resourceUri">The uri of the resource for which to validate claims.</param>
        /// <param name="resourceAccessType">The <see cref="ResourceAccessType"/>.</param>
        public ClaimPermissionsBatchRequestItem(string claimPermissionsId, string resourceUri, string resourceAccessType)
        {
            this.ClaimPermissionsId = claimPermissionsId;
            this.ResourceUri = resourceUri;
            this.ResourceAccessType = resourceAccessType;
        }

        /// <summary>
        /// Gets the content type used when this object is serialized/deserialized.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static - content handling reads this with reflection
        public string ContentType => RegisteredContentType;
#pragma warning restore CA1822

        /// <summary>
        /// Gets or sets the claim permissions ID.
        /// </summary>
        [JsonProperty("claimPermissionsId")]
        public string ClaimPermissionsId { get; set; }

        /// <summary>
        /// Gets or sets the uri for the resource for which to determine permissions.
        /// </summary>
        [JsonProperty("resourceUri")]
        public string ResourceUri { get; set; }

        /// <summary>
        /// Gets or sets the type of access required to the resource.
        /// </summary>
        [JsonProperty("resourceAccessType")]
        public string ResourceAccessType { get; set; }
    }
}