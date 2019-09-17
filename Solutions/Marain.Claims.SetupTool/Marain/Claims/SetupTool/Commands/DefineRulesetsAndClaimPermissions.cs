// <copyright file="DefineRulesetsAndClaimPermissions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool.Commands
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using Endjin.Cli;
    using Corvus.Tenancy;
    using Microsoft.Rest;
    using Newtonsoft.Json;

    /// <summary>
    /// Command to define a set of rulesets and claims in the Claims service.
    /// </summary>
    public class DefineRulesetsAndClaimPermissions : Command<DefineRulesetsAndClaimPermissions>
    {
        private AuthenticationOptions authenticationOptions;
        private string claimsAppId;
        private string claimsServiceUrl;
        private string filePath;
        private string keyVault;
        private string secretName = "ClaimsSetupApp";
        private string endjinTenantId = Tenant.Root.Id;

        /// <summary>
        /// Create a <see cref="DefineRulesetsAndClaimPermissions"/>.
        /// </summary>
        public DefineRulesetsAndClaimPermissions()
            : base("define-claims")
        {
        }

        /// <inheritdoc/>
        public override void AddOptionsAndParameters(ArgumentSyntax syntax)
        {
            this.authenticationOptions = AuthenticationOptions.FromSyntax(
                syntax);
            syntax.DefineOption("c|claimsAppId", ref this.claimsAppId, true, "The Client ID (AppId) of the Azure AD App being used by the Claim Service with Easy Auth");
            syntax.DefineOption("u|claimsServiceUrl", ref this.claimsServiceUrl, true, "The base URL for the Claims Service");
            syntax.DefineOption("e|endjinTenant", ref this.endjinTenantId, false, "The endjin tenant ID to pass");
            syntax.DefineOption("v|keyVault", ref this.keyVault, true, "The key vault containing the details of the AAD App to use when authenticating to the Claims service");
            syntax.DefineOption("s|secretName", ref this.secretName, false, "The name of the key vault secret containing the details of the AAD App to use when authenticating to the Claims service");

            syntax.DefineOption("f|file", ref this.filePath, true, "The file defining the rule sets and claim permissions");
        }

        /// <inheritdoc/>
        public override async Task ExecuteAsync()
        {
            RulesetsAndClaimPermissions input = JsonConvert.DeserializeObject<RulesetsAndClaimPermissions>(
                File.ReadAllText(this.filePath));

            ServiceClientCredentials credentials = await this.authenticationOptions.GetServiceClientCredentialsFromKeyVault(
                this.claimsAppId, this.keyVault, this.secretName).ConfigureAwait(false);
            var claimsClient = new ClaimsService(new Uri(this.claimsServiceUrl), credentials);

            foreach (ResourceAccessRuleSet ruleSet in input.RuleSets)
            {
                try
                {
                    Console.WriteLine($"Ruleset {ruleSet.Id} ('{ruleSet.DisplayName}')");
                    HttpOperationResponse<ResourceAccessRuleSetWithGetExample> result = await claimsClient.GetResourceAccessRuleSetWithHttpMessagesAsync(
                        ruleSet.Id, this.endjinTenantId).ConfigureAwait(false);

                    if (result.Response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("Does not yet exist. Creating.");
                        var request = new ResourceAccessRuleSetWithPostExample
                        {
                            Id = ruleSet.Id,
                            DisplayName = ruleSet.DisplayName,
                            Rules = ruleSet.Rules,
                        };
                        await claimsClient.CreateResourceAccessRuleSetAsync(
                            this.endjinTenantId, request).ConfigureAwait(false);
                    }
                    else if (result.Response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Already exists. Updating.");
                        await claimsClient.SetResourceAccessRuleSetResourceAccessRulesAsync(
                            this.endjinTenantId, ruleSet.Id, ruleSet.Rules).ConfigureAwait(false);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + result.Response.StatusCode);
                        string body = await result.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        if (!string.IsNullOrWhiteSpace(body))
                        {
                            Console.WriteLine(body);
                        }
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine(x);
                }
            }

            foreach (ClaimPermissions claimPermissions in input.ClaimPermissions)
            {
                try
                {
                    Console.WriteLine($"Claim Permissions {claimPermissions.Id}");
                    HttpOperationResponse<ClaimPermissionsWithGetExample> result = await claimsClient.GetClaimPermissionsWithHttpMessagesAsync(
                        claimPermissions.Id, this.endjinTenantId).ConfigureAwait(false);

                    if (result.Response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("Does not yet exist. Creating.");
                        var request = new ClaimPermissionsWithPostExample
                        {
                            Id = claimPermissions.Id,
                            ResourceAccessRules = claimPermissions.ResourceAccessRules,
                            ResourceAccessRuleSets = claimPermissions.ResourceAccessRuleSets,
                        };
                        await claimsClient.CreateClaimPermissionsAsync(
                            this.endjinTenantId, request).ConfigureAwait(false);
                    }
                    else if (result.Response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Already exists. Updating resource access rules.");
                        await claimsClient.SetClaimPermissionsResourceAccessRulesAsync(
                            this.endjinTenantId, claimPermissions.Id, claimPermissions.ResourceAccessRules).ConfigureAwait(false);
                        Console.WriteLine("Updating resource access rule sets");
                        var ruleSetIds = claimPermissions
                            .ResourceAccessRuleSets
                            .Select(rs => new ResourceAccessRuleSetId(rs.Id))
                            .ToList();
                        await claimsClient.SetClaimPermissionsResourceAccessRuleSetsAsync(
                            this.endjinTenantId, claimPermissions.Id, ruleSetIds).ConfigureAwait(false);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + result.Response.StatusCode);
                        string body = await result.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        if (!string.IsNullOrWhiteSpace(body))
                        {
                            Console.WriteLine(body);
                        }
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine(x);
                }
            }
        }

        private class RulesetsAndClaimPermissions
        {
            public IList<ResourceAccessRuleSet> RuleSets { get; set; }

            public IList<ClaimPermissions> ClaimPermissions { get; set; }
        }
    }
}
