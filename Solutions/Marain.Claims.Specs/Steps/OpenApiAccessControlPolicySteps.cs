// <copyright file="OpenApiAccessControlPolicySteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable CA1822 // Mark members as static - step methods invoked through reflection

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
    using NSubstitute;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class OpenApiAccessControlPolicySteps
    {
        private readonly FeatureContext featureContext;

        private ClaimsPrincipal claimsPrincipal;
        private string tenantId;
        private string resourcePrefix;
        private bool allowOnlyIfAll;
        private IResourceAccessEvaluator resourceAccessEvaluator;
        private List<(ResourceAccessEvaluatorArgs Args, TaskCompletionSource<List<ResourceAccessEvaluation>> TaskSource)> evaluateCalls;
        private List<ResourceAccessEvaluation> evaluations;
        private Task<IDictionary<AccessCheckOperationDescriptor, AccessControlPolicyResult>> policyResultTask;

        public OpenApiAccessControlPolicySteps(FeatureContext featureContext)
        {
            this.featureContext = featureContext;
        }

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
                identity.AddClaim(new Claim("roles", GetClaimPermissionsId(i)));
            }

            this.claimsPrincipal = new ClaimsPrincipal(identity);
            this.tenantId = Guid.NewGuid().ToString();
        }

        [Given("I have a ClaimsPrincipal with (.*) oid claims")]
        public void GivenIHaveAClaimsPrincipalWithOidClaims(int oidCount)
        {
            var identity = new ClaimsIdentity("SuperSecureTm");
            for (int i = 0; i < oidCount; ++i)
            {
                identity.AddClaim(new Claim(Claims.ClaimTypes.ObjectIdentifier, GetClaimPermissionsId(i)));
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
            this.evaluateCalls = [];
            this.evaluations = [];
            this.resourceAccessEvaluator = Substitute.For<IResourceAccessEvaluator>();
            this.resourceAccessEvaluator
                .EvaluateAsync(Arg.Any<string>(), Arg.Any<IEnumerable<ResourceAccessSubmission>>())
                .Returns(call =>
                {
                    string tenantId = call.Arg<string>();
                    IList<ResourceAccessSubmission> submissions = call.Arg<IEnumerable<ResourceAccessSubmission>>().ToList();
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
            serviceCollection.AddSingleton(Substitute.For<ILogger<OpenApiAccessControlPolicy>>());

            if (this.featureContext.FeatureInfo.Tags.Contains("rolebased"))
            {
                serviceCollection.AddRoleBasedOpenApiAccessControl(
                   this.resourcePrefix ?? string.Empty,
                   this.allowOnlyIfAll);
            }

            if (this.featureContext.FeatureInfo.Tags.Contains("identitybased"))
            {
                serviceCollection.AddIdentityBasedOpenApiAccessControl(
                   this.resourcePrefix ?? string.Empty,
                   this.allowOnlyIfAll);
            }

            serviceCollection.AddSingleton(this.resourceAccessEvaluator);
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
                string claimPermissionsId = GetClaimPermissionsId(Convert.ToInt32(current["ClaimPermissionsId"]));

                (ResourceAccessEvaluatorArgs args, TaskCompletionSource<List<ResourceAccessEvaluation>> taskSource) = this.evaluateCalls.Find(x => x.Args.Submissions.Any(r => r.ClaimPermissionsId == claimPermissionsId));

                if (!responses.TryGetValue(taskSource, out List<ResourceAccessEvaluation> taskSourceResults))
                {
                    taskSourceResults = [];
                    responses.Add(taskSource, taskSourceResults);
                }

                ResourceAccessSubmission submission = args.Submissions.FirstOrDefault(x => x.ClaimPermissionsId == claimPermissionsId);
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
                            Permission = Enum.TryParse(current["Result"], true, out Permission permission) ? permission : throw new FormatException(),
                        },
                    });
            }

            foreach (KeyValuePair<TaskCompletionSource<List<ResourceAccessEvaluation>>, List<ResourceAccessEvaluation>> current in responses)
            {
                current.Key.SetResult(current.Value);
            }
        }

        [When("the evaluator returns '(.*)' for claim permissions ID (.*)")]
        public void WhenTheEvaluatorReturnsForClaimPermissionsId(string allowOrDeny, int index)
        {
            string claimPermissionsId = GetClaimPermissionsId(index);

            (ResourceAccessEvaluatorArgs args, TaskCompletionSource<List<ResourceAccessEvaluation>> taskSource) = this.evaluateCalls.Find(x => x.Args.Submissions.Any(r => r.ClaimPermissionsId == claimPermissionsId));

            ResourceAccessSubmission submission = args.Submissions.FirstOrDefault(x => x.ClaimPermissionsId == claimPermissionsId);

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
                    Permission = Enum.TryParse(allowOrDeny, true, out Permission permission) ? permission : throw new FormatException(),
                },
            });

            taskSource.SetResult(this.evaluations);
        }

        [When("the evaluator does not find the claim permissions ID (.*)")]
        public void WhenThEvaluatorDoesNotFindTheClaimPermissionsId(int index)
        {
            string claimPermissionsId = GetClaimPermissionsId(index);

            (ResourceAccessEvaluatorArgs args, TaskCompletionSource<List<ResourceAccessEvaluation>> taskSource) = this.evaluateCalls.Find(x => x.Args.Submissions.Any(r => r.ClaimPermissionsId == claimPermissionsId));

            ResourceAccessSubmission submission = args.Submissions.FirstOrDefault(x => x.ClaimPermissionsId == claimPermissionsId);

            taskSource.SetResult(this.evaluations);
        }

        [Then("the policy should not have attempted to use the claims service")]
        public void ThenThePolicyShouldNotHaveAttemptedToUseTheClaimsService()
        {
            Assert.IsEmpty(this.evaluateCalls);
        }

        [Then("the policy should pass the claim permissions id (.*) to the claims service")]
        public void ThenThePolicyShouldPassTheClaimPermissionsIdToTheClaimsService(int index)
        {
            string claimPermissionsId = GetClaimPermissionsId(index);

            (ResourceAccessEvaluatorArgs args, _) = this.evaluateCalls.Find(x => x.Args.Submissions.Any(r => r.ClaimPermissionsId == claimPermissionsId));

            Assert.IsNotNull(args);
        }

        [Then("the policy should pass the tenant id to the claims service in call for claims permissions ID (.*)")]
        public void ThenThePolicyShouldPassTheTenantIdToTheClaimsServiceInCallForClaimsPermissionsId(int index)
        {
            string claimPermissionId = GetClaimPermissionsId(index);

            (ResourceAccessEvaluatorArgs args, _) = this.evaluateCalls.Find(x => x.Args.Submissions.Any(r => r.ClaimPermissionsId == claimPermissionId));

            Assert.AreEqual(this.tenantId, args?.TenantId);
        }

        [Then("the policy should pass a resource URI of '(.*)' to the claims service in call for claims permissions ID (.*)")]
        public void ThenThePolicyShouldPassAResourceURIOfToTheClaimsServiceInCallForClaimsPermissionsId(
            string resourceUri,
            int index)
        {
            string claimPermissionId = GetClaimPermissionsId(index);

            (ResourceAccessEvaluatorArgs args, _) = this.evaluateCalls.Find(x => x.Args.Submissions.Any(r => r.ClaimPermissionsId == claimPermissionId));

            Assert.IsTrue(args.Submissions.Any(x => x.ResourceUri == resourceUri));
        }

        [Then("the policy should pass an access type of '(.*)' to the claims service in call for claims permissions ID (.*)")]
        public void ThenThePolicyShouldPassAnAccessTypeOfToTheClaimsServiceInCallForClaimsPermissionsId(
            string accessType,
            int index)
        {
            string claimPermissionsId = GetClaimPermissionsId(index);

            (ResourceAccessEvaluatorArgs args, _) = this.evaluateCalls.Find(x => x.Args.Submissions.Any(r => r.ClaimPermissionsId == claimPermissionsId));

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

        private static string GetClaimPermissionsId(int number) => $"ClaimPermissionsId{number}";

        private class ResourceAccessEvaluatorArgs
        {
            public IList<ResourceAccessSubmission> Submissions { get; set; }

            public string TenantId { get; set; }
        }
    }
}