// <copyright file="ShowAppIdentityInformation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    using Azure.ResourceManager;
    using Azure.ResourceManager.AppService;
    using Azure.ResourceManager.AppService.Models;
    using Azure.ResourceManager.Models;

    using McMaster.Extensions.CommandLineUtils;

    /// <summary>
    /// Command to discover information about an Azure Function or Web App's identity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This looks both for a managed identity and an associated Easy Auth application.
    /// </para>
    /// <para>
    /// We're writing this in C# instead of PowerShell because the Azure PowerShell support for
    /// managed service identities seems still to be in pre-release.
    /// </para>
    /// </remarks>
    [Command(Name = "show-app-identity", Description = "Display an app identity.", ShowInHelpText = true)]
    [HelpOption]
    public class ShowAppIdentityInformation
    {
        /// <summary>
        /// Gets or sets the Azure subscription ID.
        /// </summary>
        [Option(Description = "ID of the Azure subscription to inspect", LongName = "subscriptionId", ShortName = "s")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource group containing the service.
        /// </summary>
        [Option(Description = "The resource group containing the service", LongName = "resourceGroupName", ShortName = "g")]
        public string ResourceGroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the web app or function.
        /// </summary>
        [Option(Description = "The name of the web app or function", LongName = "name", ShortName = "n")]
        public string AppName { get; set; }

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
            ArmClient client = new(authenticationOptions.GetAzureCredentials());
            WebSiteResource functionResource = null;
            WebSiteData function = null;
            try
            {
                functionResource = client.GetWebSiteResource(WebSiteResource.CreateResourceIdentifier(this.SubscriptionId, this.ResourceGroupName, this.AppName));
                Azure.Response<WebSiteResource> result = await functionResource.GetAsync(cancellationToken);
                function = function = result.Value.Data;
            }
            catch (Azure.RequestFailedException rfx)
            when (rfx.Status == 404)
            {
            }

            if (function is null)
            {
                app.Error.WriteLine($"Unable to find either a Function or Web App in resource group '{this.ResourceGroupName}' called '{this.AppName}'");
            }
            else
            {
                ManagedServiceIdentity managedIdentity = function.Identity;
                SiteAuthSettings webAppAuthConfig = await functionResource.GetAuthSettingsAsync(cancellationToken);
                SiteAuthSettingsV2 webAppAuthConfigV2 = await functionResource.GetAuthSettingsV2Async(cancellationToken);

                if (webAppAuthConfigV2.IdentityProviders is not null)
                {
                    app.Out.WriteLine($"Default Easy Auth (v2): {webAppAuthConfigV2.GlobalValidation?.RedirectToProvider ?? "no default provider"}");
                    app.Out.WriteLine($" Client ID: {webAppAuthConfigV2.IdentityProviders.AzureActiveDirectory?.Registration?.ClientId ?? "client id not set"}");
                }
                else if (webAppAuthConfig?.IsEnabled == true)
                {
                    app.Out.WriteLine($"Default Easy Auth (v2): {webAppAuthConfig.DefaultProvider}");
                    app.Out.WriteLine($" Client ID: {webAppAuthConfig.ClientId}");
                }
                else
                {
                    app.Out.WriteLine("Easy Auth not enabled");
                }

                if (managedIdentity == null)
                {
                    app.Out.WriteLine("No managed identity");
                }
                else
                {
                    app.Out.WriteLine("Managed identity:");
                    app.Out.WriteLine($" Type:                 {managedIdentity.ManagedServiceIdentityType}");
                    app.Out.WriteLine($" TenantId:             {managedIdentity.TenantId}");
                    app.Out.WriteLine($" PrincipalId:          {managedIdentity.PrincipalId}");

                    if (managedIdentity.UserAssignedIdentities != null)
                    {
                        foreach ((Azure.Core.ResourceIdentifier id, UserAssignedIdentity value) in managedIdentity.UserAssignedIdentities)
                        {
                            app.Out.WriteLine($" UserAssignedIdentity: Id = {id}, ClientId = {value.ClientId}, PrincipalId = {value.PrincipalId}");
                        }
                    }
                }
            }

            return 0;
        }
    }
}