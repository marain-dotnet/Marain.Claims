// <copyright file="ShowAppIdentityInformation.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool.Commands
{
    using System;
    using System.CommandLine;
    using System.Threading.Tasks;
    using Endjin.Cli;
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
    public class ShowAppIdentityInformation : Command<ShowAppIdentityInformation>
    {
        private readonly AppServiceManagerSource appServiceManagerSource;

        private AuthenticationOptions authenticationOptions;
        private string subscriptionId;
        private string resourceGroupName;
        private string appName;

        /// <summary>
        /// Create a <see cref="ShowAppIdentityInformation"/>.
        /// </summary>
        /// <param name="appServiceManagerSource">
        /// Provides management of Azure app service features.
        /// </param>
        public ShowAppIdentityInformation(
            AppServiceManagerSource appServiceManagerSource)
            : base("show-app-identity")
        {
            this.appServiceManagerSource = appServiceManagerSource;
        }

        /// <inheritdoc/>
        public override void AddOptionsAndParameters(ArgumentSyntax syntax)
        {
            this.authenticationOptions = AuthenticationOptions.FromSyntax(syntax);
            syntax.DefineOption("s|subscriptionId", ref this.subscriptionId, true, "ID of the Azure subscription to inspect");
            syntax.DefineOption("g|ResourceGroupName", ref this.resourceGroupName, "The resource group containing the service");
            syntax.DefineOption("n|Name", ref this.appName, "The name of the web app or function");
        }

        /// <inheritdoc/>
        public override async Task ExecuteAsync()
        {
            IAppServiceManager appServiceManager = this.appServiceManagerSource.Get(
                this.authenticationOptions, this.subscriptionId);
            IWebAppAuthentication webAppAuthConfig;
            ManagedServiceIdentity managedIdentity;
            IFunctionApp function = null;
            try
            {
                function = appServiceManager.FunctionApps.GetByResourceGroup(this.resourceGroupName, this.appName);
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
                IWebApp webApp = appServiceManager.WebApps.GetByResourceGroup(this.resourceGroupName, this.appName);
                if (webApp == null)
                {
                    Console.WriteLine($"Unable to find either a Function or Web App in resource group '{this.resourceGroupName}' called '{this.appName}'");
                    return;
                }

                managedIdentity = webApp.Inner.Identity;
                webAppAuthConfig = await webApp.GetAuthenticationConfigAsync().ConfigureAwait(false);
            }

            if (webAppAuthConfig.Inner.Enabled == true)
            {
                Console.WriteLine($"Default Easy Auth: {webAppAuthConfig.Inner.DefaultProvider}");
                Console.WriteLine($" Client ID: {webAppAuthConfig.Inner.ClientId}");
            }
            else
            {
                Console.WriteLine("Easy Auth not enabled");
            }

            if (managedIdentity == null)
            {
                Console.WriteLine("No managed identity");
            }
            else
            {
                Console.WriteLine("Managed identity:");
                Console.WriteLine($" Type:                 {managedIdentity.Type}");
                Console.WriteLine($" TenantId:             {managedIdentity.TenantId}");
                Console.WriteLine($" PrincipalId:          {managedIdentity.PrincipalId}");

                if (managedIdentity.UserAssignedIdentities != null)
                {
                    foreach ((string id, ManagedServiceIdentityUserAssignedIdentitiesValue value) in managedIdentity.UserAssignedIdentities)
                    {
                        Console.WriteLine($" UserAssignedIdentity: Id = {id}, ClientId = {value.ClientId}, PrincipalId = {value.PrincipalId}");
                    }
                }
            }
        }
    }
}
