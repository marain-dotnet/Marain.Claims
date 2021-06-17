// <copyright file="WorkflowClaimPermissionsSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Steps
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Marain.Claims.OpenApi.Specs.MultiHost;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    [Binding]
    public class ClaimPermissionsSteps
    {
        private readonly ITestableClaimsService serviceWrapper;
        private readonly Dictionary<string, string> claimIds = new Dictionary<string, string>();

        private int statusCodeFromClaimsService;
        private ClaimPermissions claimPermissionsFromClaimsService;

        public ClaimPermissionsSteps(ITestableClaimsService serviceWrapper)
        {
            this.serviceWrapper = serviceWrapper;
        }

        [Given(@"a unique ClaimsPermission id named '(.*)'")]
        public void GivenAUniqueClaimsPermissionIdNamed(string claimIdName)
        {
            this.claimIds.Add(claimIdName, $"claimId{this.claimIds.Count}");
        }

        [When(@"ClaimsPermission with id named '(.*)' is fetched from the getClaimPermissions endpoint")]
        public async Task WhenClaimsPermissionWithIdNamedIsFetchedFromTheGetClaimPermissionsEndpoint(string claimIdName)
        {
            string claimId = this.claimIds[claimIdName];
            (this.statusCodeFromClaimsService, this.claimPermissionsFromClaimsService) = await this.serviceWrapper.GetClaimIdAsync(claimId);
        }

        [Then(@"the HTTP status returned by the Claims service is (.*)")]
        public void ThenTheHTTPStatusReturnedByTheClaimsServiceIs(int expectedStatus)
        {
            Assert.AreEqual(expectedStatus, this.statusCodeFromClaimsService);
        }
    }
}
