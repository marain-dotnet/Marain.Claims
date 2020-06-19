// <copyright file="IResourceAccessEvaluator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Evaluates resource access submissions.
    /// </summary>
    public interface IResourceAccessEvaluator
    {
        /// <summary>
        /// This will evaluate a batch of resource access submissions and return the evaluated results.
        /// </summary>
        /// <param name="tenantId">The tenant ID to use for the request.</param>
        /// <param name="submissions">The resource access submissions to evaluate.</param>
        /// <returns>A task producing the evauluated result for each submission for which the claims
        /// permission ID was recognised.</returns>
        /// <remarks>
        /// If a submission's <see cref="ResourceAccessSubmission.ClaimPermissionsId"/> is not recognised,
        /// there will be no corresponding entry in the result.
        /// </remarks>
        Task<List<ResourceAccessEvaluation>> EvaluateAsync(string tenantId, IEnumerable<ResourceAccessSubmission> submissions);
    }
}