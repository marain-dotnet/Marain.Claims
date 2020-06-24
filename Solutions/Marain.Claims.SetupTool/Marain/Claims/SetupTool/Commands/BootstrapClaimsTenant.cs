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
    using Corvus.Tenancy;
    using Marain.Claims.Client;
    using Marain.Claims.Client.Models;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Rest;

    /// <summary>
    /// Command to invoke the bootstrapping endpoint of the Claims service in order to set up the
    /// initial administrative permissions for modifying claims.
    /// </summary>
    [Command(Name = "bootstrap-claims-tenant", Description = "Bootstraps the claims for a new tenant.", ThrowOnUnexpectedArgument = false)]
    [HelpOption]
    public class BootstrapClaimsTenant
    {
        /// <summary>
        /// Create a <see cref="BootstrapClaimsTenant"/>.
        /// </summary>
        public BootstrapClaimsTenant()
        {
        }

        /// <summary>
        /// Gets or sets the claims app ID.
        /// </summary>
        [Option(Description = "The name of the Application Role in the Claims Service that is to be granted administrative control over the Claims Service", LongName = "claimsAppId", ShortName = "c")]
        public string ClaimsAppId { get; set; }

        /// <summary>
        /// Gets or sets the claims service URL
        /// </summary>
        [Option(Description = "The base URL for the Claims Service", LongName = "claimsServiceUrl", ShortName = "u")]
        public string ClaimsServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the admin role name.
        /// </summary>
        [Option(Description = "The name of the Application Role in the Claims Service that is to be granted administrative control over the Claims Service", LongName = "adminRoleName", ShortName = "r")]
        public string AdminRoleName { get; set; } = "ClaimsAdministrator";

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

        private async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken cancellationToken = default)
        {
            var authenticationOptions = AuthenticationOptions.BuildFrom(this.UseAzCliDevAuth, this.TenantId);

            ServiceClientCredentials credentials = await authenticationOptions.GetServiceClientCredentialsFromKeyVault(
                this.ClaimsAppId, this.KeyVault, this.SecretName).ConfigureAwait(false);

            using (var claimsClient = new ClaimsService(new Uri(this.ClaimsServiceUrl), credentials))
            {
                try
                {
                    HttpOperationResponse<ProblemDetails> result = await claimsClient.InitializeTenantWithHttpMessagesAsync(
                            this.MarainTenantId,
                            new Body { AdministratorRoleClaimValue = this.AdminRoleName })
                        .ConfigureAwait(false);

                    if (result.Response.IsSuccessStatusCode)
                    {
                        app.Out.WriteLine("Succeeded");
                    }
                    else
                    {
                        app.Error.WriteLine($"Failed with status code {result.Response.StatusCode}");
                        if (result.Body != null)
                        {
                            app.Error.WriteLine(result.Body.Title);
                            app.Error.WriteLine(result.Body.Detail);
                        }
                    }
                }
                catch (HttpOperationException x)
                {
                    app.Error.WriteLine($"Failed with status code {x.Response.StatusCode}");
                    if (!string.IsNullOrWhiteSpace(x.Response.Content))
                    {
                        app.Error.WriteLine("Response content:");
                        app.Error.WriteLine(x.Response.Content);
                    }

                    if (x.Response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        app.Error.WriteLine();
                        app.Error.WriteLine("Check that you have specified the correct tenant and claims id");
                    }

                    if (x.Response.Headers.TryGetValue("WWW-Authenticate", out IEnumerable<string> values))
                    {
                        var valueList = values.ToList();
                        if (valueList.Count > 0)
                        {
                            Console.WriteLine("WWW-Authenticate header{0}:", valueList.Count > 1 ? "s" : string.Empty);
                            foreach (string value in valueList)
                            {
                                app.Error.WriteLine(value);
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
