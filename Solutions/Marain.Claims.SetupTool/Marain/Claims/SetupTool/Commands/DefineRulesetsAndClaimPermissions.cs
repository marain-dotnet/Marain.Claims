// <copyright file="DefineRulesetsAndClaimPermissions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Cli;
    using Corvus.Tenancy;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Rest;
    using Newtonsoft.Json;

    /// <summary>
    /// Command to define a set of rulesets and claims in the Claims service.
    /// </summary>
    public class DefineRulesetsAndClaimPermissions : Command<DefineRulesetsAndClaimPermissions>
    {
#pragma warning disable IDE0044, CS0649 // These items are set by reflection
        private string claimsAppId;
        private string claimsServiceUrl;
        private string filePath;
        private string keyVault;
        private string secretName = "ClaimsSetupApp";
        private string marainTenantId = RootTenant.RootTenantId;
        private bool useAzCliDevAuth;
        private string tenantId;
#pragma warning restore IDE0044, CS0649 // These items are set by reflection

        /// <summary>
        /// Create a <see cref="DefineRulesetsAndClaimPermissions"/>.
        /// </summary>
        public DefineRulesetsAndClaimPermissions()
            : base("define-claims", "Define the rules and claims for a tenant.")
        {
        }

        /// <inheritdoc/>
        public override void AddOptions(CommandLineApplication command)
        {
            this.AddBooleanOption(command, "-d|--devAzCliAuth", "Authenticate using the token last fetched by the 'az' CLI", () => this.useAzCliDevAuth);
            this.AddSingleOption(command, "-t|--tenantId <value>", "The tenant against which to authenticate", () => this.tenantId);
            this.AddSingleOption(command, "-c|--claimsAppId <value>", "The Client ID (AppId) of the Azure AD App being used by the Claim Service with Easy Auth", () => this.claimsAppId);
            this.AddSingleOption(command, "-u|--claimsServiceUrl <value>", "The base URL for the Claims Service", () => this.claimsServiceUrl);
            this.AddSingleOption(command, "-m|--marainTenant <value>", "The Marain tenant ID to pass", () => this.marainTenantId);
            this.AddSingleOption(command, "-v|--keyVault <value>", "The key vault containing the details of the AAD App to use when authenticating to the Claims service", () => this.keyVault);
            this.AddSingleOption(command, "-s|--secretName <value>", "The name of the key vault secret containing the details of the AAD App to use when authenticating to the Claims service", () => this.secretName);
            this.AddSingleOption(command, "-f|--file <value>", "The file defining the rule sets and claim permissions", () => this.filePath);
        }

        /// <inheritdoc/>
        public override async Task<int> ExecuteAsync(CancellationToken token)
        {
            var authenticationOptions = AuthenticationOptions.BuildFrom(this.useAzCliDevAuth, this.tenantId);

            RulesetsAndClaimPermissions input = JsonConvert.DeserializeObject<RulesetsAndClaimPermissions>(
                File.ReadAllText(this.filePath));

            ServiceClientCredentials credentials = await authenticationOptions.GetServiceClientCredentialsFromKeyVault(
                this.claimsAppId, this.keyVault, this.secretName).ConfigureAwait(false);
            using (var claimsClient = new ClaimsService(new Uri(this.claimsServiceUrl), credentials))
            {
                foreach (ResourceAccessRuleSet ruleSet in input.RuleSets)
                {
                    try
                    {
                        Console.WriteLine($"Ruleset {ruleSet.Id} ('{ruleSet.DisplayName}')");
                        HttpOperationResponse<ResourceAccessRuleSetWithGetExample> result = await claimsClient.GetResourceAccessRuleSetWithHttpMessagesAsync(
                            ruleSet.Id, this.marainTenantId).ConfigureAwait(false);

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
                                this.marainTenantId, request).ConfigureAwait(false);
                        }
                        else if (result.Response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Already exists. Updating.");
                            await claimsClient.SetResourceAccessRuleSetResourceAccessRulesAsync(
                                this.marainTenantId, ruleSet.Id, ruleSet.Rules).ConfigureAwait(false);
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
                        return -1;
                    }
                }

                foreach (ClaimPermissions claimPermissions in input.ClaimPermissions)
                {
                    try
                    {
                        Console.WriteLine($"Claim Permissions {claimPermissions.Id}");
                        HttpOperationResponse<ClaimPermissionsWithGetExample> result = await claimsClient.GetClaimPermissionsWithHttpMessagesAsync(
                            claimPermissions.Id, this.marainTenantId).ConfigureAwait(false);

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
                                this.marainTenantId, request).ConfigureAwait(false);
                        }
                        else if (result.Response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Already exists. Updating resource access rules.");
                            await claimsClient.SetClaimPermissionsResourceAccessRulesAsync(
                                this.marainTenantId, claimPermissions.Id, claimPermissions.ResourceAccessRules).ConfigureAwait(false);
                            Console.WriteLine("Updating resource access rule sets");
                            var ruleSetIds = claimPermissions
                                .ResourceAccessRuleSets
                                .Select(rs => new ResourceAccessRuleSetId(rs.Id))
                                .ToList();
                            await claimsClient.SetClaimPermissionsResourceAccessRuleSetsAsync(
                                this.marainTenantId, claimPermissions.Id, ruleSetIds).ConfigureAwait(false);
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
                        return -1;
                    }
                }
            }

            return 0;
        }

        private class RulesetsAndClaimPermissions
        {
            public IList<ResourceAccessRuleSet> RuleSets { get; set; }

            public IList<ClaimPermissions> ClaimPermissions { get; set; }
        }
    }
}
