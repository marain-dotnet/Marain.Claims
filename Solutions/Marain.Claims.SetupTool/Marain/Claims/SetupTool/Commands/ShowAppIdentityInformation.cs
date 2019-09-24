// <copyright file="ShowAppIdentityInformation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool.Commands
{
    using System;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Azure.Management.AppService.Fluent;
    using Microsoft.Azure.Management.AppService.Fluent.Models;

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
    [Command(Name = "show-app-identity", Description = "Display an app identity.", ThrowOnUnexpectedArgument = false, ShowInHelpText = true)]
    [HelpOption]
    public class ShowAppIdentityInformation : CommandLineApplication
    {
        private readonly AppServiceManagerSource appServiceManagerSource;

        /// <summary>
        /// Create a <see cref="ShowAppIdentityInformation"/>.
        /// </summary>
        public ShowAppIdentityInformation()
        {
            this.appServiceManagerSource = new AppServiceManagerSource();
            this.OnExecuteAsync(async ct =>
            {
                var authenticationOptions = AuthenticationOptions.BuildFrom(this.UseAzCliDevAuth, this.TenantId);
                IAppServiceManager appServiceManager = this.appServiceManagerSource.Get(
                    authenticationOptions, this.SubscriptionId);
                IWebAppAuthentication webAppAuthConfig;
                ManagedServiceIdentity managedIdentity;
                IFunctionApp function = null;
                try
                {
                    function = appServiceManager.FunctionApps.GetByResourceGroup(this.ResourceGroupName, this.AppName);
                }
                catch (NullReferenceException)
                {
                    // Unhelpfully, we seem to get a null reference exception if the app isn't found
                }

                if (function != null)
                {
                    managedIdentity = function.Inner.Identity;
                    webAppAuthConfig = await function.GetAuthenticationConfigAsync().ConfigureAwait(false);
                }
                else
                {
                    IWebApp webApp = appServiceManager.WebApps.GetByResourceGroup(this.ResourceGroupName, this.AppName);
                    if (webApp == null)
                    {
                        this.Error.WriteLine($"Unable to find either a Function or Web App in resource group '{this.ResourceGroupName}' called '{this.AppName}'");
                        return -1;
                    }

                    managedIdentity = webApp.Inner.Identity;
                    webAppAuthConfig = await webApp.GetAuthenticationConfigAsync().ConfigureAwait(false);
                }

                if (webAppAuthConfig.Inner.Enabled == true)
                {
                    this.Out.WriteLine($"Default Easy Auth: {webAppAuthConfig.Inner.DefaultProvider}");
                    this.Out.WriteLine($" Client ID: {webAppAuthConfig.Inner.ClientId}");
                }
                else
                {
                    this.Out.WriteLine("Easy Auth not enabled");
                }

                if (managedIdentity == null)
                {
                    this.Out.WriteLine("No managed identity");
                }
                else
                {
                    this.Out.WriteLine("Managed identity:");
                    this.Out.WriteLine($" Type:                 {managedIdentity.Type}");
                    this.Out.WriteLine($" TenantId:             {managedIdentity.TenantId}");
                    this.Out.WriteLine($" PrincipalId:          {managedIdentity.PrincipalId}");

                    if (managedIdentity.UserAssignedIdentities != null)
                    {
                        foreach ((string id, ManagedServiceIdentityUserAssignedIdentitiesValue value) in managedIdentity.UserAssignedIdentities)
                        {
                            this.Out.WriteLine($" UserAssignedIdentity: Id = {id}, ClientId = {value.ClientId}, PrincipalId = {value.PrincipalId}");
                        }
                    }
                }

                return 0;
            });
        }

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
    }
}
