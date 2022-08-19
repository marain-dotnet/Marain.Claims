// <copyright file="ClaimsServiceTestTenants.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Specs
{
    /// <summary>
    /// Makes transient tenants available to tests.
    /// </summary>
    /// <param name="TransientServiceTenantId">
    /// Gets the tenant ID used for the service in this test.
    /// </param>
    /// <param name="TransientClientTenantId">
    /// Gets the tenant ID of the client in this test.
    /// </param>
    public record ClaimsServiceTestTenants(string TransientServiceTenantId, string TransientClientTenantId);
}