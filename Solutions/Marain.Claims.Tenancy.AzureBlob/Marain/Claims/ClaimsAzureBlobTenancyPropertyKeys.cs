// <copyright file="ClaimsAzureBlobTenancyPropertyKeys.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims;

/// <summary>
/// The keys under which blob container configuration entries are stored in tenant properties.
/// </summary>
public static class ClaimsAzureBlobTenancyPropertyKeys
{
    /// <summary>
    /// The key under which configuration for the claim permissions blob container must be stored
    /// in tenant properties.
    /// </summary>
    public const string ClaimPermissions = "Marain:Claims:BlobContainerConfiguration:ClaimPermissions";

    /// <summary>
    /// The key under which configuration for the resource access rule set blob container must be stored
    /// in tenant properties.
    /// </summary>
    public const string ResourceAccessRuleSet = "Marain:Claims:BlobContainerConfiguration:ResourceAccessRuleSets";
}