// <copyright file="ShowAppIdentityInformation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Cli;
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
    public class ShowAppIdentityInformation : Command<ShowAppIdentityInformation>
    {
        private readonly AppServiceManagerSource appServiceManagerSource;

#pragma warning disable IDE0044, CS0649 // These items are set by reflection
        private string subscriptionId;
        private string resourceGroupName;
        private string appName;
        private bool useAzCliDevAuth;
        private string tenantId;
#pragma warning restore IDE0044, CS0649 // These items are set by reflection

        /// <summary>
        /// Create a <see cref="ShowAppIdentityInformation"/>.
        /// </summary>
        public ShowAppIdentityInformation()
            : base("show-app-identity", "Display an app identity.")
        {
            this.appServiceManagerSource = new AppServiceManagerSource();
        }

        /// <inheritdoc/>
        public override void AddOptions(CommandLineApplication command)
        {
            this.AddBooleanOption(command, "-d|--devAzCliAuth", "Authenticate using the token last fetched by the 'az' CLI", () => this.useAzCliDevAuth);
            this.AddSingleOption(command, "-t|--tenantId <value>", "The tenant against which to authenticate", () => this.tenantId);
            this.AddSingleOption(command, "-s|--subscriptionId <value>", "ID of the Azure subscription to inspect", () => this.subscriptionId);
            this.AddSingleOption(command, "-g|--ResourceGroupName <value>", "The resource group containing the service", () => this.resourceGroupName);
            this.AddSingleOption(command, "-n|--Name <value>", "The name of the web app or function", () => this.appName);
        }

        /// <inheritdoc/>
        public override async Task<int> ExecuteAsync(CancellationToken token)
        {
            var authenticationOptions = AuthenticationOptions.BuildFrom(this.useAzCliDevAuth, this.tenantId);
            IAppServiceManager appServiceManager = this.appServiceManagerSource.Get(
                authenticationOptions, this.subscriptionId);
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
                    return -1;
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

            return 0;
        }
    }
}
