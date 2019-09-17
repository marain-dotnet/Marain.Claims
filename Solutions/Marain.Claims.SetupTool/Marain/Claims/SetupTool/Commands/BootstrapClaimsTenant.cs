// <copyright file="BootstrapClaimsTenant.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool.Commands
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using Endjin.Cli;
    using Corvus.Tenancy;
    using Microsoft.Rest;

    /// <summary>
    /// Command to invoke the bootstrapping endpoint of the Claims service in order to set up the
    /// initial administrative permissions for modifying claims.
    /// </summary>
    public class BootstrapClaimsTenant : Command<BootstrapClaimsTenant>
    {
        private AuthenticationOptions authenticationOptions;
        private string claimsAppId;
        private string claimsServiceUrl;
        private string adminRoleName = "ClaimsAdministrator";
        private string keyVault;
        private string secretName = "ClaimsSetupApp";
        private string endjinTenantId = RootTenant.RootTenantId;

        /// <summary>
        /// Create a <see cref="BootstrapClaimsTenant"/>.
        /// </summary>
        public BootstrapClaimsTenant()
            : base("bootstrap-claims-tenant")
        {
        }

        /// <inheritdoc/>
        public override void AddOptionsAndParameters(ArgumentSyntax syntax)
        {
            this.authenticationOptions = AuthenticationOptions.FromSyntax(
                syntax);
            syntax.DefineOption("c|claimsAppId", ref this.claimsAppId, true, "The Client ID (AppId) of the Azure AD App being used by the Claim Service with Easy Auth");
            syntax.DefineOption("u|claimsServiceUrl", ref this.claimsServiceUrl, true, "The base URL for the Claims Service");
            syntax.DefineOption("r|adminRoleName", ref this.adminRoleName, false, "The name of the Application Role in the Claims Service that is to be granted administrative control over the Claims Service");
            syntax.DefineOption("e|endjinTenant", ref this.endjinTenantId, false, "The endjin tenant ID to pass");

            syntax.DefineOption("v|keyVault", ref this.keyVault, true, "The key vault containing the details of the AAD App to use when authenticating to the Claims service");
            syntax.DefineOption("s|secretName", ref this.secretName, false, "The name of the key vault secret containing the details of the AAD App to use when authenticating to the Claims service");
        }

        /// <inheritdoc/>
        public override async Task ExecuteAsync()
        {
            ServiceClientCredentials credentials = await this.authenticationOptions.GetServiceClientCredentialsFromKeyVault(
                this.claimsAppId, this.keyVault, this.secretName).ConfigureAwait(false);
            var claimsClient = new ClaimsService(new Uri(this.claimsServiceUrl), credentials);

            try
            {
                HttpOperationResponse<ProblemDetails> result = await claimsClient.InitializeTenantWithHttpMessagesAsync(
                        this.endjinTenantId,
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
            }
        }
    }
}
