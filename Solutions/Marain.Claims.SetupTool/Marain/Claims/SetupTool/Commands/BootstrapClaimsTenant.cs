// <copyright file="BootstrapClaimsTenant.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool.Commands
{
    using System;
    using System.Collections.Generic;
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

    /// <summary>
    /// Command to invoke the bootstrapping endpoint of the Claims service in order to set up the
    /// initial administrative permissions for modifying claims.
    /// </summary>
    public class BootstrapClaimsTenant : Command<BootstrapClaimsTenant>
    {
#pragma warning disable IDE0044, CS0649 // These items are set by reflection
        private string claimsAppId;
        private string claimsServiceUrl;
        private string adminRoleName = "ClaimsAdministrator";
        private string keyVault;
        private string secretName = "ClaimsSetupApp";
        private string marainTenantId = RootTenant.RootTenantId;
        private string tenantId = null;
        private bool useAzCliDevAuth;
#pragma warning restore IDE0044, CS0649 // These items are set by reflection

        /// <summary>
        /// Create a <see cref="BootstrapClaimsTenant"/>.
        /// </summary>
        public BootstrapClaimsTenant()
            : base("bootstrap-claims-tenant", "Boostraps the claims for a new tenant")
        {
        }

        /// <inheritdoc/>
        public override void AddOptions(CommandLineApplication command)
        {
            this.AddBooleanOption(command, "-d|--devAzCliAuth", "Authenticate using the token last fetched by the 'az' CLI", () => this.useAzCliDevAuth);
            this.AddSingleOption(command, "-t|--tenantId <value>", "The tenant against which to authenticate", () => this.tenantId);
            this.AddSingleOption(command, "-c|--claimsAppId <value>", "The Client ID (AppId) of the Azure AD App being used by the Claim Service with Easy Auth", () => this.claimsAppId);
            this.AddSingleOption(command, "-u|--claimsServiceUrl <value>", "The base URL for the Claims Service", () => this.claimsServiceUrl);
            this.AddSingleOption(command, "-r|--adminRoleName <value>", "The name of the Application Role in the Claims Service that is to be granted administrative control over the Claims Service", () => this.adminRoleName);
            this.AddSingleOption(command, "-m|--marainTenant <value>", "The Marain tenant ID to pass", () => this.marainTenantId);
            this.AddSingleOption(command, "-v|--keyVault <value>", "The key vault containing the details of the AAD App to use when authenticating to the Claims service", () => this.keyVault);
            this.AddSingleOption(command, "-s|--secretName <value>", "The name of the key vault secret containing the details of the AAD App to use when authenticating to the Claims service", () => this.secretName);
        }

        /// <inheritdoc/>
        public override async Task<int> ExecuteAsync(CancellationToken token)
        {
            var authenticationOptions = AuthenticationOptions.BuildFrom(this.useAzCliDevAuth, this.tenantId);

            ServiceClientCredentials credentials = await authenticationOptions.GetServiceClientCredentialsFromKeyVault(
                this.claimsAppId, this.keyVault, this.secretName).ConfigureAwait(false);
            using (var claimsClient = new ClaimsService(new Uri(this.claimsServiceUrl), credentials))
            {
                try
                {
                    HttpOperationResponse<ProblemDetails> result = await claimsClient.InitializeTenantWithHttpMessagesAsync(
                            this.marainTenantId,
                            new Body { AdministratorRoleClaimValue = this.adminRoleName })
                        .ConfigureAwait(false);

                    if (result.Response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Succeeded");
                    }
                    else
                    {
                        Console.WriteLine($"Failed with status code {result.Response.StatusCode}");
                        if (result.Body != null)
                        {
                            Console.WriteLine(result.Body.Title);
                            Console.WriteLine(result.Body.Detail);
                        }
                    }
                }
                catch (HttpOperationException x)
                {
                    Console.WriteLine($"Failed with status code {x.Response.StatusCode}");
                    if (!string.IsNullOrWhiteSpace(x.Response.Content))
                    {
                        Console.WriteLine("Response content:");
                        Console.WriteLine(x.Response.Content);
                    }

                    if (x.Response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Check that you have specified the correct tenant and claims id");
                    }

                    if (x.Response.Headers.TryGetValue("WWW-Authenticate", out IEnumerable<string> values))
                    {
                        var valueList = values.ToList();
                        if (valueList.Count > 0)
                        {
                            Console.WriteLine("WWW-Authenticate header{0}:", valueList.Count > 1 ? "s" : string.Empty);
                            foreach (string value in valueList)
                            {
                                Console.WriteLine(value);
                            }
                        }
                    }

                    return -1;
                }
            }

            return 0;
        }
    }
}
