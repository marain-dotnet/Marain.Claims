// <copyright file="IdentityBasedResourceAccessSubmissionBuilder.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Menes;

    /// <summary>
    /// Builds resource access submissions based on the user's identities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This maps the user's oid claims in the <see cref="ClaimsPrincipal"/> to resource access
    /// submissions.
    /// </para>
    /// <para>
    /// For example, given an POST to http://example.com/foo/bar/1234 this will ask the Claims
    /// service to evaluate permissions using the user's oid claim as the <c>claimPermissionsId</c>,
    /// <c>foo/bar/1234</c> as the <c>resourceUri</c>, and <c>POST</c> as the <c>accessType</c>.
    /// </para>
    /// </remarks>
    public class IdentityBasedResourceAccessSubmissionBuilder : IResourceAccessSubmissionBuilder
    {
        /// <inheritdoc/>
        public List<ResourceAccessSubmission> BuildResourceAccessSubmissions(IOpenApiContext context, AccessCheckOperationDescriptor[] requests, Dictionary<string, string> pathToResourceUriMap)
        {
            // Get the list of oids for the user.
            IList<string> oids = context.CurrentPrincipal
                .Claims
                .Where(c => c.Type == Claims.ClaimTypes.Oid || c.Type == Claims.ClaimTypes.ObjectIdentifier)
                .Select(c => c.Value)
                .ToList();

            // Now translate the set of requested evaluations into a set of requests for the claims service. This is built
            // from the cartesian product of the oids and requests lists.
            var submissions = oids.SelectMany(oid => requests.Select(request => new ResourceAccessSubmission
            {
                ClaimPermissionsId = oid,
                ResourceAccessType = request.Method,
                ResourceUri = pathToResourceUriMap[request.Path],
            })).ToList();
            return submissions;
        }
    }
}