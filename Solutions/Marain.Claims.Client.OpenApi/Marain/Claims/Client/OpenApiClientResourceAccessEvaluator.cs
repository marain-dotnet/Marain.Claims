// <copyright file="OpenApiClientResourceAccessEvaluator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Marain.Claims.Client.Models;
    using Marain.Claims.OpenApi;
    using Microsoft.Extensions.Logging;
    using Microsoft.Rest;

    /// <summary>
    /// This evaluates resource access submissions using the Marain Claims service.
    /// </summary>
    internal class OpenApiClientResourceAccessEvaluator : IResourceAccessEvaluator
    {
        private readonly IClaimsService claimsClient;
        private readonly ILogger<OpenApiClientResourceAccessEvaluator> logger;

        /// <summary>
        /// Creates an <see cref="OpenApiClientResourceAccessEvaluator"/>.
        /// </summary>
        /// <param name="claimsClient">The claims service client.</param>
        /// <param name="logger">The logger.</param>
        public OpenApiClientResourceAccessEvaluator(
            IClaimsService claimsClient,
            ILogger<OpenApiClientResourceAccessEvaluator> logger)
        {
            this.claimsClient = claimsClient;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<List<ResourceAccessEvaluation>> EvaluateAsync(string tenantId, IEnumerable<ResourceAccessSubmission> submissions)
        {
            // Now translate the set of requested evaluations into a set of requests for the claims service. This is built
            // from the cartesian product of the roles and requests lists.
            var batchRequest = submissions.Select(submission => new ClaimPermissionsBatchRequestItem
            {
                ClaimPermissionsId = submission.ClaimPermissionsId,
                ResourceAccessType = submission.ResourceAccessType,
                ResourceUri = submission.ResourceUri,
            }).ToList();

            // Now send this batch of requests to the claims service.
            HttpOperationResponse<object> batchResponse = await this.claimsClient.GetClaimPermissionsPermissionBatchWithHttpMessagesAsync(tenantId, batchRequest).ConfigureAwait(false);

            // If evaluation failed entirely, log this and throw an exception.
            if (!batchResponse.Response.IsSuccessStatusCode)
            {
                string details = string.Join(Environment.NewLine, batchRequest.Select(x => $"\tID [{x.ClaimPermissionsId}] accessing [{x.ResourceUri}], [{x.ResourceAccessType}]"));
                string ids = string.Join(",", submissions.Select(s => s.ClaimPermissionsId));

                this.logger.LogError(
                    "Permission evaluation for claim permission IDs [{ids}] failed with status code [{statusCode}]. Details follow:\r\n{details}",
                    ids,
                    batchResponse.Response.StatusCode,
                    details);

                batchResponse.Response.EnsureSuccessStatusCode();
            }

            var batchResponseBody = (IList<ClaimPermissionsBatchResponseItem>)batchResponse.Body;

            // If any of the requests didn't return an OK result, we need to log a warning, as this is most likely due to
            // misconfiguration of the claims service (e.g. a missing ClaimPermissionsId). The caller may still be able to carry
            // on and evaluate the remaining results.
            foreach (ClaimPermissionsBatchResponseItem currentEvaluatedPermission in batchResponseBody.Where(x => x.ResponseCode != (int)HttpStatusCode.OK))
            {
                this.logger.LogWarning(
                    "Claims service returned [{statusCode}] permission evaluation for claim permission ID [{claimPermissionID}] accessing resource [{resourceUri}], [{httpMethod}]",
                    currentEvaluatedPermission.ResponseCode,
                    currentEvaluatedPermission.ClaimPermissionsId,
                    currentEvaluatedPermission.ResourceUri,
                    currentEvaluatedPermission.ResourceAccessType);
            }

            return batchResponseBody
                .Where(item => item.ResponseCode == (int)HttpStatusCode.OK)
                .Select(item =>
                    new ResourceAccessEvaluation
                    {
                        Result = new Claims.PermissionResult { Permission = Enum.TryParse(item.Permission, true, out Permission permission) ? permission : throw new FormatException() },
                        Submission = new ResourceAccessSubmission { ClaimPermissionsId = item.ClaimPermissionsId, ResourceAccessType = item.ResourceAccessType, ResourceUri = item.ResourceUri },
                    })
                .ToList();
        }
    }
}