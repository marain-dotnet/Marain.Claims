// <copyright file="IResourceAccessSubmissionBuilder.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Internal
{
    using System.Collections.Generic;
    using Menes;

    /// <summary>
    /// This builds a list of resource access submissions to be evaluated
    /// for a given set of requests.
    /// </summary>
    public interface IResourceAccessSubmissionBuilder
    {
        /// <summary>
        /// Builds the resource access submissions for evaluation.
        /// </summary>
        /// <param name="context">The Open API context.</param>
        /// <param name="requests">The list of operation descriptors to check.</param>
        /// <param name="pathToResourceUriMap">Mapping of requested paths to resource URIs.</param>
        /// <returns>The list of resource access submissions for evaluation.</returns>
        List<ResourceAccessSubmission> BuildResourceAccessSubmissions(
            IOpenApiContext context,
            AccessCheckOperationDescriptor[] requests,
            Dictionary<string, string> pathToResourceUriMap);
    }
}
