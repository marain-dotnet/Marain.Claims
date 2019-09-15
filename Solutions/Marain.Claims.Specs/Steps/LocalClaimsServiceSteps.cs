// <copyright file="RepositorySteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Elements should be documented

namespace Marain.Claims.SpecFlow.Steps
{
    using System.Net;
    using System.Threading.Tasks;
    using Marain.Claims.Client.Models;
    using Idg.AsyncTest;
    using Idg.AsyncTest.TaskExtensions;
    using Microsoft.Rest;
    using Moq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;
    using System.Linq;
    using System.Collections.Generic;
    using Menes;

    [Binding]
    public class LocalClaimsServiceSteps
    {
        private readonly Mock<IClaimPermissionsEvaluator> service = new Mock<IClaimPermissionsEvaluator>();
        private readonly CompletionSource<OpenApiResult> serviceCompletionSource = new CompletionSource<OpenApiResult>();

        private Task<HttpOperationResponse<object>> getPermissionsTask;

        [When("I have passed a claim permissions id of '(.*)', a resource URI of '(.*)', an access type of '(.*)', and a tenant id of '(.*)'")]
        public void WhenIHavePassedAClaimPermissionsIdOfAResourceURIOfAnAccessTypeOfAndATenantIdOf(
            string claimPermissionsId,
            string resourceUri,
            string accessType,
            string tenantId)
        {
            this.service
                .Setup(m => m.GetClaimPermissionsPermissionAsync(It.IsAny<string>(), It.IsAny<ClaimPermissionsBatchRequestItem[]>()))
                .Returns((string _, ClaimPermissionsBatchRequestItem[] __) => this.serviceCompletionSource.GetTask());
#pragma warning disable IDE0067 // Dispose objects before losing scope
            var client = new LocalClaimsService(this.service.Object);
#pragma warning restore IDE0067 // Dispose objects before losing scope

            var requests = new List<Client.Models.ClaimPermissionsBatchRequestItemWithPostExample>
            {
                new Client.Models.ClaimPermissionsBatchRequestItemWithPostExample(claimPermissionsId, resourceUri, accessType),
            };

            this.getPermissionsTask = client.GetClaimPermissionsPermissionBatchWithHttpMessagesAsync(tenantId, requests);
        }

        [When("the service returns an OK result of '(.*)'")]
        public void WhenTheServiceReturnsAnOKResultOf(Permission permission)
        {
            ClaimPermissionsBatchResponseItem[] resultBody =
            {
                new ClaimPermissionsBatchResponseItem
                {
                    Permission = permission == Permission.Allow ? "allow" : "deny"
                }
            };
            var result = new OpenApiResult
            {
                StatusCode = 200,
                Results = { { "application/json", resultBody } },
            };
            this.serviceCompletionSource.SupplyResult(result);
        }

        [When("the service returns a Not Found result")]
        public void WhenTheServiceReturnsAnNotFoundResult()
        {
            var result = new OpenApiResult
            {
                StatusCode = 404,
            };
            this.serviceCompletionSource.SupplyResult(result);
        }

        [Then("the service should have been passed a claim permissions id of '(.*)'")]
        public void ThenTheServiceShouldHaveBeenPassedAClaimPermissionsIdOf(string claimPermissionsId)
        {
            this.service.Verify(m => m.GetClaimPermissionsPermissionAsync(
                It.IsAny<string>(),
                It.Is<ClaimPermissionsBatchRequestItem[]>(x => x.First().ClaimPermissionsId == claimPermissionsId)));
        }

        [Then("the service should have been passed a resource URI of '(.*)'")]
        public void ThenTheServiceShouldHaveBeenPassedAResourceURIOf(string resourceUri)
        {
            this.service.Verify(m => m.GetClaimPermissionsPermissionAsync(
                It.IsAny<string>(),
                It.Is<ClaimPermissionsBatchRequestItem[]>(x => x.First().ResourceUri == resourceUri)));
        }

        [Then("the service should have been passed an access type of '(.*)'")]
        public void ThenTheServiceShouldHaveBeenPassedAnAccessTypeOf(string accessType)
        {
            this.service.Verify(m => m.GetClaimPermissionsPermissionAsync(
                It.IsAny<string>(),
                It.Is<ClaimPermissionsBatchRequestItem[]>(x => x.First().ResourceAccessType == accessType)));
        }

        [Then("the service should have been passed a tenant id of '(.*)'")]
        public void ThenTheServiceShouldHaveBeenPassedATenantIdOf(string tenantId)
        {
            this.service.Verify(m => m.GetClaimPermissionsPermissionAsync(
                tenantId,
                It.IsAny<ClaimPermissionsBatchRequestItem[]>()));
        }

        [Then("the response wrapper should have a status code of (.*)")]
        public async Task ThenTheResponseWrapperShouldHaveAStatusCodeOf(int statusCode)
        {
            HttpOperationResponse<object> result = await this.getPermissionsTask.WithTimeout().ConfigureAwait(false);
            Assert.AreEqual((HttpStatusCode)statusCode, result.Response.StatusCode);
        }

        [Then("the response body should contain a single permissions batch response item containing '(.*)'")]
        public async Task ThenTheResponseBodyShouldContainASingleClaimPermissionsBatchResponseItemWithExampleAsync(string permission)
        {
            HttpOperationResponse<object> result = await this.getPermissionsTask.WithTimeout().ConfigureAwait(false);
            var body = (IList<ClaimPermissionsBatchResponseItemWithExample>)result.Body;
            Assert.AreEqual(1, body.Count);
            Assert.AreEqual(permission, body[0].Permission);
        }

        [Then("the response should not have a body")]
        public async Task ThenTheResponseShouldNotHaveABodyAsync()
        {
            HttpOperationResponse<object> result = await this.getPermissionsTask.WithTimeout().ConfigureAwait(false);
            Assert.IsNull(result.Body);
        }
    }
}
