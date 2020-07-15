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
        [Option(Description = "The Azure AD app ID for the claims service.", LongName = "claimsAppId", ShortName = "c")]
        public string ClaimsAppId { get; set; }

        /// <summary>
        /// Gets or sets the claims service URL.
        /// </summary>
        [Option(Description = "The base URL for the Claims Service", LongName = "claimsServiceUrl", ShortName = "u")]
        public string ClaimsServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the admin principal object ID.
        /// </summary>
        [Option(Description = "The object ID of the principal that is to be granted administrative control over the Claims Service. If left blank, will use the object ID of the principal making the initialistion request.", LongName = "adminPrincipalObjectId", ShortName = "a")]
        public string AdminPrincipalObjectId { get; set; }

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
                try
                {
                    HttpOperationResponse<ProblemDetails> result = await claimsClient.InitializeTenantWithHttpMessagesAsync(
                            this.MarainTenantId,
                            new Body { AdministratorPrincipalObjectId = this.AdminPrincipalObjectId })
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
