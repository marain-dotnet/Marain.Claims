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
    using Corvus.Tenancy;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Rest;
    using Newtonsoft.Json;

    /// <summary>
    /// Command to define a set of rulesets and claims in the Claims service.
    /// </summary>
    [Command(Name = "define-claims", Description = "Define the rules and claims for a tenant.", ThrowOnUnexpectedArgument = false, ShowInHelpText = true)]
    [HelpOption]
    public class DefineRulesetsAndClaimPermissions
    {
        /// <summary>
        /// Create a <see cref="DefineRulesetsAndClaimPermissions"/>.
        /// </summary>
        public DefineRulesetsAndClaimPermissions()
        {
        }

        /// <summary>
        /// Gets or sets the claims app ID.
        /// </summary>
        [Option(Description = "The name of the Application Role in the Claims Service that is to be granted administrative control over the Claims Service", LongName = "claimsAppId", ShortName = "c")]
        public string ClaimsAppId { get; set; }

        /// <summary>
        /// Gets or sets the admin role name.
        /// </summary>
        [Option(Description = "The base URL for the Claims Service", LongName = "claimsServiceUrl", ShortName = "u")]
        public string ClaimsServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the file path for the rulesets.
        /// </summary>
        [Option(Description = "The file defining the rule sets and claim permissions", LongName = "filePath", ShortName = "f")]
        [FileExists]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the key vault name.
        /// </summary>
        [Option(Description = "The key vault containing the details of the AAD App to use when authenticating to the Claims service", LongName = "keyVault", ShortName = "v")]
        public string KeyVault { get; set; }

        /// <summary>
        /// Gets or sets the secret name.
        /// </summary>
        [Option(Description = "The name of the key vault secret containing the details of the AAD App to use when authenticating to the Claims service", LongName = "secretName", ShortName = "s")]
        public string SecretName { get; set; } = "ClaimsSetupApp";

        /// <summary>
        /// Gets or sets the Marain tenant ID.
        /// </summary>
        [Option(Description = "The Marain tenant ID to pass", LongName = "marainTenant", ShortName = "m")]
        public string MarainTenantId { get; set; } = RootTenant.RootTenantId;

        /// <summary>
        /// Gets or sets the AD tenant ID.
        /// </summary>
        [Option(Description = "The tenant against which to authenticate", LongName = "tenantId", ShortName = "t")]
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the 'az' CLI token.
        /// </summary>
        [Option(Description = "Authenticate using the token last fetched by the 'az' CLI", LongName = "devAzCliAuth", ShortName = "d")]
        public bool UseAzCliDevAuth { get; set; }

        /// <summary>
        /// Gets or sets a value used for roles in X-MARAIN-CLAIMS header.
        /// </summary>
        [Option(Description = "Used for when running against a local debug instance of Claims. Will send X-MARAIN-CLAIMS header with {\"roles\":[\"<this-value>\"]} to authenticate, instead of using OAuth2.", LongName = "marainClaimsHeaderValue", ShortName = "x")]
        public string MarainClaimsHeaderRoleValue { get; set; }

        private async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            RulesetsAndClaimPermissions input = JsonConvert.DeserializeObject<RulesetsAndClaimPermissions>(
                File.ReadAllText(this.FilePath));

            ClaimsService claimsClient;

            if (string.IsNullOrEmpty(this.MarainClaimsHeaderRoleValue))
            {
                var authenticationOptions = AuthenticationOptions.BuildFrom(this.UseAzCliDevAuth, this.TenantId);

                ServiceClientCredentials credentials = await authenticationOptions.GetServiceClientCredentialsFromKeyVault(
                    this.ClaimsAppId, this.KeyVault, this.SecretName).ConfigureAwait(false);

                claimsClient = new ClaimsService(new Uri(this.ClaimsServiceUrl), credentials);
            }
            else
            {
                claimsClient = new ClaimsService(new Uri(this.ClaimsServiceUrl), new BasicAuthenticationCredentials());
                claimsClient.HttpClient.DefaultRequestHeaders.Add("X-MARAIN-CLAIMS", $"{{ \"roles\": [ \"{this.MarainClaimsHeaderRoleValue}\" ] }}");
            }

            using (claimsClient)
            {
                foreach (ResourceAccessRuleSet ruleSet in input.RuleSets)
                {
                    try
                    {
                        app.Out.WriteLine($"Ruleset {ruleSet.Id} ('{ruleSet.DisplayName}')");
                        HttpOperationResponse<ResourceAccessRuleSetWithGetExample> result = await claimsClient.GetResourceAccessRuleSetWithHttpMessagesAsync(
                            ruleSet.Id, this.MarainTenantId).ConfigureAwait(false);

                        if (result.Response.StatusCode == HttpStatusCode.NotFound)
                        {
                            app.Error.WriteLine("Does not yet exist. Creating.");
                            var request = new ResourceAccessRuleSetWithPostExample
                            {
                                Id = ruleSet.Id,
                                DisplayName = ruleSet.DisplayName,
                                Rules = ruleSet.Rules,
                            };
                            await claimsClient.CreateResourceAccessRuleSetAsync(
                                this.MarainTenantId, request).ConfigureAwait(false);
                        }
                        else if (result.Response.IsSuccessStatusCode)
                        {
                            app.Out.WriteLine("Already exists. Updating.");
                            await claimsClient.SetResourceAccessRuleSetResourceAccessRulesAsync(
                                this.MarainTenantId, ruleSet.Id, ruleSet.Rules).ConfigureAwait(false);
                        }
                        else
                        {
                            app.Error.WriteLine("Error: " + result.Response.StatusCode);
                            string body = await result.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            if (!string.IsNullOrWhiteSpace(body))
                            {
                                app.Error.WriteLine(body);
                            }
                        }
                    }
                    catch (Exception x)
                    {
                        app.Error.WriteLine(x);
                        return -1;
                    }
                }

                foreach (ClaimPermissions claimPermissions in input.ClaimPermissions)
                {
                    try
                    {
                        app.Out.WriteLine($"Claim Permissions {claimPermissions.Id}");
                        HttpOperationResponse<ClaimPermissionsWithGetExample> result = await claimsClient.GetClaimPermissionsWithHttpMessagesAsync(
                            claimPermissions.Id, this.MarainTenantId).ConfigureAwait(false);

                        if (result.Response.StatusCode == HttpStatusCode.NotFound)
                        {
                            app.Out.WriteLine("Does not yet exist. Creating.");
                            var request = new ClaimPermissionsWithPostExample
                            {
                                Id = claimPermissions.Id,
                                ResourceAccessRules = claimPermissions.ResourceAccessRules,
                                ResourceAccessRuleSets = claimPermissions.ResourceAccessRuleSets,
                            };
                            await claimsClient.CreateClaimPermissionsAsync(
                                this.MarainTenantId, request).ConfigureAwait(false);
                        }
                        else if (result.Response.IsSuccessStatusCode)
                        {
                            app.Out.WriteLine("Already exists. Updating resource access rules.");
                            await claimsClient.SetClaimPermissionsResourceAccessRulesAsync(
                                this.MarainTenantId, claimPermissions.Id, claimPermissions.ResourceAccessRules).ConfigureAwait(false);
                            app.Out.WriteLine("Updating resource access rule sets");
                            var ruleSetIds = claimPermissions
                                .ResourceAccessRuleSets
                                .Select(rs => new ResourceAccessRuleSetId(rs.Id))
                                .ToList();
                            await claimsClient.SetClaimPermissionsResourceAccessRuleSetsAsync(
                                this.MarainTenantId, claimPermissions.Id, ruleSetIds).ConfigureAwait(false);
                        }
                        else
                        {
                            app.Error.WriteLine("Error: " + result.Response.StatusCode);
                            string body = await result.Response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            if (!string.IsNullOrWhiteSpace(body))
                            {
                                app.Error.WriteLine(body);
                            }
                        }
                    }
                    catch (Exception x)
                    {
                        app.Error.WriteLine(x);
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
