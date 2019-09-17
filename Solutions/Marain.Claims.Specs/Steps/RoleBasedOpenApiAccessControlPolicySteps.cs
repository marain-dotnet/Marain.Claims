// <copyright file="RoleBasedOpenApiAccessControlPolicySteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Elements should be documented

namespace Marain.Claims.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Idg.AsyncTest.TaskExtensions;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using Marain.Claims.OpenApi;
    using Menes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Rest;
    using Moq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class RoleBasedOpenApiAccessControlPolicySteps
    {
        private ClaimsPrincipal claimsPrincipal;
        private string tenantId;
        private string resourcePrefix;
        private bool allowOnlyIfAll;
        private Mock<IClaimsService> claimsClient;
        private List<(GetClaimPermissionsPermissionBatchArgs args, TaskCompletionSource<HttpOperationResponse<object>> taskSource)> getPermissionCalls;
        private List<ClaimPermissionsBatchResponseItemWithExample> responseBody;
        private Task<IDictionary<AccessCheckOperationDescriptor, AccessControlPolicyResult>> policyResultTask;

        [Given("I am accepting the default unauthenticated behaviour")]
        public void GivenIAmAcceptingTheDefaultUnauthenticatedBehaviour()
        {
            // No action required - this exists purely so the spec makes sense when you read it
        }

        [Given("I am not authenticated")]
        public void GivenIAmNotAuthenticated()
        {
            var identity = new ClaimsIdentity();
            this.claimsPrincipal = new ClaimsPrincipal(identity);
            this.tenantId = Guid.NewGuid().ToString();
        }

        [Given("I have a ClaimsPrincipal with (.*) roles claims")]
        public void GivenIHaveAClaimsPrincipalWithRolesClaims(int roleCount)
        {
            var identity = new ClaimsIdentity("SuperSecureTm");
            for (int i = 0; i < roleCount; ++i)
            {
                identity.AddClaim(new Claim("roles", GetRoleId(i)));
            }

            this.claimsPrincipal = new ClaimsPrincipal(identity);
            this.tenantId = Guid.NewGuid().ToString();
        }

        [Given("the policy has a resource prefix of '(.*)'")]
        public void GivenThePolicyHasAResourcePrefixOf(string resourcePrefix)
        {
            this.resourcePrefix = resourcePrefix;
        }

        [Given("the policy is configured in allow only if all mode")]
        public void GivenThePolicyIsConfiguredInAllowOnlyIfAllMode()
        {
            this.allowOnlyIfAll = true;
        }

        [When("I invoke the policy with a path of '(.*)' and a method of '(.*)'")]
        public void WhenIInvokeThePolicyWithAPathOfAndAMethodOf(string path, string method)
        {
            this.getPermissionCalls = new List<(GetClaimPermissionsPermissionBatchArgs args, TaskCompletionSource<HttpOperationResponse<object>> taskSource)>();
            this.responseBody = new List<ClaimPermissionsBatchResponseItemWithExample>();
            this.claimsClient = new Mock<IClaimsService>();
            this.claimsClient
                .Setup(m => m.GetClaimPermissionsPermissionBatchWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<IList<ClaimPermissionsBatchRequestItemWithPostExample>>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns((string tenantId, IList<ClaimPermissionsBatchRequestItemWithPostExample> body, Dictionary<string, List<string>> _, CancellationToken cancellationToken) =>
                {
                    var args = new GetClaimPermissionsPermissionBatchArgs
                    {
                        Requests = body,
                        TenantId = tenantId,
                        CancellationToken = cancellationToken,
                    };

                    var completionSource = new TaskCompletionSource<HttpOperationResponse<object>>();
                    this.getPermissionCalls.Add((args, completionSource));

                    return completionSource.Task;
                });

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new Mock<ILogger<RoleBasedOpenApiAccessControlPolicy>>().Object);
            serviceCollection.AddRoleBasedOpenApiAccessControl(
                this.resourcePrefix ?? "",
                this.allowOnlyIfAll);
            serviceCollection.AddSingleton(this.claimsClient.Object);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            IOpenApiAccessControlPolicy policy = serviceProvider.GetRequiredService<IOpenApiAccessControlPolicy>();

            this.policyResultTask = policy.ShouldAllowAsync(new SimpleOpenApiContext { CurrentPrincipal = this.claimsPrincipal, CurrentTenantId = this.tenantId }, new AccessCheckOperationDescriptor(path, "op123", method));
        }

        [When("the claims service returns the following results")]
        public void WhenTheClaimsServiceReturnsTheFollowingResults(Table table)
        {
            var responses = new Dictionary<TaskCompletionSource<HttpOperationResponse<object>>, List<ClaimPermissionsBatchResponseItemWithExample>>();

            foreach (TableRow current in table.Rows)
            {
                string roleId = GetRoleId(Convert.ToInt32(current["Role"]));

                (GetClaimPermissionsPermissionBatchArgs args, TaskCompletionSource<HttpOperationResponse<object>> taskSource) = this.getPermissionCalls.FirstOrDefault(x => x.args.Requests.Any(r => r.ClaimPermissionsId == roleId));

                if (!responses.TryGetValue(taskSource, out List<ClaimPermissionsBatchResponseItemWithExample> taskSourceResults))
                {
                    taskSourceResults = new List<ClaimPermissionsBatchResponseItemWithExample>();
                    responses.Add(taskSource, taskSourceResults);
                }

                ClaimPermissionsBatchRequestItemWithPostExample targetRequest = args.Requests.FirstOrDefault(x => x.ClaimPermissionsId == roleId);
                taskSourceResults.Add(new ClaimPermissionsBatchResponseItemWithExample(targetRequest.ClaimPermissionsId, targetRequest.ResourceUri, targetRequest.ResourceAccessType, (int)HttpStatusCode.OK, current["Result"]));
            }

            foreach (KeyValuePair<TaskCompletionSource<HttpOperationResponse<object>>, List<ClaimPermissionsBatchResponseItemWithExample>> current in responses)
            {
                var result = new HttpOperationResponse<object>
                {
                    Body = current.Value,
                    Response = new HttpResponseMessage(HttpStatusCode.OK),
                };

                current.Key.SetResult(result);
            }
        }

        [When("the claims service returns '(.*)' for role (.*)")]
        public void WhenTheClaimsServiceReturnsForRole(string allowOrDeny, int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (GetClaimPermissionsPermissionBatchArgs args, TaskCompletionSource<HttpOperationResponse<object>> taskSource) = this.getPermissionCalls.FirstOrDefault(x => x.args.Requests.Any(r => r.ClaimPermissionsId == roleId));

            ClaimPermissionsBatchRequestItemWithPostExample targetRequest = args.Requests.FirstOrDefault(x => x.ClaimPermissionsId == roleId);

            this.responseBody.Add(new ClaimPermissionsBatchResponseItemWithExample(targetRequest.ClaimPermissionsId, targetRequest.ResourceUri, targetRequest.ResourceAccessType, (int)HttpStatusCode.OK, allowOrDeny));

            var result = new HttpOperationResponse<object>
            {
                Body = this.responseBody,
                Response = new HttpResponseMessage(HttpStatusCode.OK),
            };

            taskSource.SetResult(result);
        }

        [When("the claims service returns produces a 404 not found for role (.*)")]
        public void WhenTheClaimsServiceReturnsProducesANotFoundRole(int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (GetClaimPermissionsPermissionBatchArgs args, TaskCompletionSource<HttpOperationResponse<object>> taskSource) = this.getPermissionCalls.FirstOrDefault(x => x.args.Requests.Any(r => r.ClaimPermissionsId == roleId));

            ClaimPermissionsBatchRequestItemWithPostExample targetRequest = args.Requests.FirstOrDefault(x => x.ClaimPermissionsId == roleId);

            this.responseBody.Add(new ClaimPermissionsBatchResponseItemWithExample(targetRequest.ClaimPermissionsId, targetRequest.ResourceUri, targetRequest.ResourceAccessType, (int)HttpStatusCode.NotFound, null));

            var result = new HttpOperationResponse<object>
            {
                Response = new HttpResponseMessage(HttpStatusCode.OK),
                Body = this.responseBody,
            };
            taskSource.SetResult(result);
        }

        [Then("the policy should not have attempted to use the claims service")]
        public void ThenThePolicyShouldNotHaveAttemptedToUseTheClaimsService()
        {
            Assert.IsEmpty(this.getPermissionCalls);
        }

        [Then("the policy should pass the claim permissions id for role (.*) to the claims service")]
        public void ThenThePolicyShouldPassTheClaimPermissionsIdForRoleToTheClaimsService(int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (GetClaimPermissionsPermissionBatchArgs args, _) = this.getPermissionCalls.FirstOrDefault(x => x.args.Requests.Any(r => r.ClaimPermissionsId == roleId));

            Assert.IsNotNull(args);
        }

        [Then("the policy should pass the tenant id to the claims service in call for role (.*)")]
        public void ThenThePolicyShouldPassTheTenantIdToTheClaimsServiceInCallForRole(int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (GetClaimPermissionsPermissionBatchArgs args, _) = this.getPermissionCalls.FirstOrDefault(x => x.args.Requests.Any(r => r.ClaimPermissionsId == roleId));

            Assert.AreEqual(this.tenantId, args?.TenantId);
        }

        [Then("the policy should pass a resource URI of '(.*)' to the claims service in call for role (.*)")]
        public void ThenThePolicyShouldPassAResourceURIOfToTheClaimsServiceInCallForRole(
            string resourceUri,
            int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (GetClaimPermissionsPermissionBatchArgs args, _) = this.getPermissionCalls.FirstOrDefault(x => x.args.Requests.Any(r => r.ClaimPermissionsId == roleId));

            Assert.IsTrue(args.Requests.Any(x => x.ResourceUri == resourceUri));
        }

        [Then("the policy should pass an access type of '(.*)' to the claims service in call for role (.*)")]
        public void ThenThePolicyShouldPassAnAccessTypeOfToTheClaimsServiceInCallForRole(
            string accessType,
            int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (GetClaimPermissionsPermissionBatchArgs args, _) = this.getPermissionCalls.FirstOrDefault(x => x.args.Requests.Any(r => r.ClaimPermissionsId == roleId));

            Assert.IsTrue(args.Requests.Any(x => x.ResourceAccessType == accessType));
        }

        [Then("the result should grant access")]
        public async Task ThenTheResultShouldGrantAccessAsync()
        {
            IDictionary<AccessCheckOperationDescriptor, AccessControlPolicyResult> result = await this.policyResultTask.WithTimeout().ConfigureAwait(false);
            Assert.IsTrue(result.Values.First().Allow);
        }

        [Then("the result type should be '(.*)'")]
        public async Task ThenTheResultShouldDenyAccess(AccessControlPolicyResultType resultType)
        {
            IDictionary<AccessCheckOperationDescriptor, AccessControlPolicyResult> result = await this.policyResultTask.WithTimeout().ConfigureAwait(false);
            Assert.AreEqual(resultType, result.Values.First().ResultType);
        }

        private static string GetRoleId(int roleNumber) => $"RoleId{roleNumber}";

        private class GetClaimPermissionsPermissionBatchArgs
        {
            public IList<ClaimPermissionsBatchRequestItemWithPostExample> Requests { get; set; }

            public string TenantId { get; set; }

            public CancellationToken CancellationToken { get; set; }
        }
    }
}
