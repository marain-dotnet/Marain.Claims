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
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Idg.AsyncTest.TaskExtensions;
    using Marain.Claims.OpenApi;
    using Menes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
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
        private Mock<IResourceAccessEvaluator> resourceAccessEvaluator;
        private List<(ResourceAccessEvaluatorArgs args, TaskCompletionSource<List<ResourceAccessEvaluation>> taskSource)> evaluateCalls;
        private List<ResourceAccessEvaluation> evaluations;
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
            this.evaluateCalls = new List<(ResourceAccessEvaluatorArgs args, TaskCompletionSource<List<ResourceAccessEvaluation>> taskSource)>();
            this.evaluations = new List<ResourceAccessEvaluation>();
            this.resourceAccessEvaluator = new Mock<IResourceAccessEvaluator>();
            this.resourceAccessEvaluator
                .Setup(m => m.EvaluateAsync(It.IsAny<string>(), It.IsAny<IEnumerable<ResourceAccessSubmission>>()))
                .Returns((string tenantId, IList<ResourceAccessSubmission> submissions) =>
                {
                    var args = new ResourceAccessEvaluatorArgs
                    {
                        Submissions = submissions,
                        TenantId = tenantId,
                    };

                    var completionSource = new TaskCompletionSource<List<ResourceAccessEvaluation>>();
                    this.evaluateCalls.Add((args, completionSource));

                    return completionSource.Task;
                });

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new Mock<ILogger<RoleBasedOpenApiAccessControlPolicy>>().Object);
            serviceCollection.AddRoleBasedOpenApiAccessControl(
                this.resourcePrefix ?? "",
                this.allowOnlyIfAll);
            serviceCollection.AddSingleton(this.resourceAccessEvaluator.Object);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            IOpenApiAccessControlPolicy policy = serviceProvider.GetRequiredService<IOpenApiAccessControlPolicy>();

            this.policyResultTask = policy.ShouldAllowAsync(new SimpleOpenApiContext { CurrentPrincipal = this.claimsPrincipal, CurrentTenantId = this.tenantId }, new AccessCheckOperationDescriptor(path, "op123", method));
        }

        [When("the evaluator returns the following results")]
        public void WhenTheEvaluatorReturnsTheFollowingResults(Table table)
        {
            var responses = new Dictionary<TaskCompletionSource<List<ResourceAccessEvaluation>>, List<ResourceAccessEvaluation>>();

            foreach (TableRow current in table.Rows)
            {
                string roleId = GetRoleId(Convert.ToInt32(current["Role"]));

                (ResourceAccessEvaluatorArgs args, TaskCompletionSource<List<ResourceAccessEvaluation>> taskSource) = this.evaluateCalls.FirstOrDefault(x => x.args.Submissions.Any(r => r.ClaimPermissionsId == roleId));

                if (!responses.TryGetValue(taskSource, out List<ResourceAccessEvaluation> taskSourceResults))
                {
                    taskSourceResults = new List<ResourceAccessEvaluation>();
                    responses.Add(taskSource, taskSourceResults);
                }

                ResourceAccessSubmission submission = args.Submissions.FirstOrDefault(x => x.ClaimPermissionsId == roleId);
                taskSourceResults.Add(
                    new ResourceAccessEvaluation
                    {
                        Submission = new ResourceAccessSubmission
                        {
                            ClaimPermissionsId = submission.ClaimPermissionsId,
                            ResourceUri = submission.ResourceUri,
                            ResourceAccessType = submission.ResourceAccessType,
                        },
                        Result = new Claims.PermissionResult
                        {
                            Permission = Enum.TryParse(current["Result"], true, out Permission permission) ? permission : throw new FormatException()
                        }
                    });
            }

            foreach (KeyValuePair<TaskCompletionSource<List<ResourceAccessEvaluation>>, List<ResourceAccessEvaluation>> current in responses)
            {
                current.Key.SetResult(current.Value);
            }
        }

        [When("the evaluator returns '(.*)' for role (.*)")]
        public void WhenTheEvaluatorReturnsForRole(string allowOrDeny, int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (ResourceAccessEvaluatorArgs args, TaskCompletionSource<List<ResourceAccessEvaluation>> taskSource) = this.evaluateCalls.FirstOrDefault(x => x.args.Submissions.Any(r => r.ClaimPermissionsId == roleId));

            ResourceAccessSubmission submission = args.Submissions.FirstOrDefault(x => x.ClaimPermissionsId == roleId);

            this.evaluations.Add(new ResourceAccessEvaluation
            {
                Submission = new ResourceAccessSubmission
                {
                    ClaimPermissionsId = submission.ClaimPermissionsId,
                    ResourceUri = submission.ResourceUri,
                    ResourceAccessType = submission.ResourceAccessType,
                },
                Result = new Claims.PermissionResult
                {
                    Permission = Enum.TryParse(allowOrDeny, true, out Permission permission) ? permission : throw new FormatException()
                }
            });

            taskSource.SetResult(this.evaluations);
        }

        [When("the evaluator does not find the role (.*)")]
        public void WhenThEvaluatorDoesNotFindTheRole(int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (ResourceAccessEvaluatorArgs args, TaskCompletionSource<List<ResourceAccessEvaluation>> taskSource) = this.evaluateCalls.FirstOrDefault(x => x.args.Submissions.Any(r => r.ClaimPermissionsId == roleId));

            ResourceAccessSubmission submission = args.Submissions.FirstOrDefault(x => x.ClaimPermissionsId == roleId);

            taskSource.SetResult(this.evaluations);
        }

        [Then("the policy should not have attempted to use the claims service")]
        public void ThenThePolicyShouldNotHaveAttemptedToUseTheClaimsService()
        {
            Assert.IsEmpty(this.evaluateCalls);
        }

        [Then("the policy should pass the claim permissions id for role (.*) to the claims service")]
        public void ThenThePolicyShouldPassTheClaimPermissionsIdForRoleToTheClaimsService(int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (ResourceAccessEvaluatorArgs args, _) = this.evaluateCalls.FirstOrDefault(x => x.args.Submissions.Any(r => r.ClaimPermissionsId == roleId));

            Assert.IsNotNull(args);
        }

        [Then("the policy should pass the tenant id to the claims service in call for role (.*)")]
        public void ThenThePolicyShouldPassTheTenantIdToTheClaimsServiceInCallForRole(int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (ResourceAccessEvaluatorArgs args, _) = this.evaluateCalls.FirstOrDefault(x => x.args.Submissions.Any(r => r.ClaimPermissionsId == roleId));

            Assert.AreEqual(this.tenantId, args?.TenantId);
        }

        [Then("the policy should pass a resource URI of '(.*)' to the claims service in call for role (.*)")]
        public void ThenThePolicyShouldPassAResourceURIOfToTheClaimsServiceInCallForRole(
            string resourceUri,
            int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (ResourceAccessEvaluatorArgs args, _) = this.evaluateCalls.FirstOrDefault(x => x.args.Submissions.Any(r => r.ClaimPermissionsId == roleId));

            Assert.IsTrue(args.Submissions.Any(x => x.ResourceUri == resourceUri));
        }

        [Then("the policy should pass an access type of '(.*)' to the claims service in call for role (.*)")]
        public void ThenThePolicyShouldPassAnAccessTypeOfToTheClaimsServiceInCallForRole(
            string accessType,
            int roleIndex)
        {
            string roleId = GetRoleId(roleIndex);

            (ResourceAccessEvaluatorArgs args, _) = this.evaluateCalls.FirstOrDefault(x => x.args.Submissions.Any(r => r.ClaimPermissionsId == roleId));

            Assert.IsTrue(args.Submissions.Any(x => x.ResourceAccessType == accessType));
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

        private class ResourceAccessEvaluatorArgs
        {
            public IList<ResourceAccessSubmission> Submissions { get; set; }

            public string TenantId { get; set; }
        }
    }
}
