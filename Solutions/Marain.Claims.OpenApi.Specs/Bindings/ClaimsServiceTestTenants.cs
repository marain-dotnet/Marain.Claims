// <copyright file="WorkflowClaimsServiceTestTenants.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Bindings
{
    /// <summary>
    /// Makes transient tenants available to tests.
    /// </summary>
    public class ClaimsServiceTestTenants
    {
        public ClaimsServiceTestTenants(
            string transientServiceTenantId,
            string transientClientTenantId)
        {
            this.TransientServiceTenantId = transientServiceTenantId;
            this.TransientClientTenantId = transientClientTenantId;
        }

        /// <summary>
        /// Gets the tenant ID used for the service in this test.
        /// </summary>
        public string TransientServiceTenantId { get; }

        /// <summary>
        /// Gets the tenant ID of the client in this test.
        /// </summary>
        public string TransientClientTenantId { get; }
    }
}