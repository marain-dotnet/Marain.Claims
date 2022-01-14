// <copyright file="AppServiceManagerSource.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool
{
    using Microsoft.Azure.Management.AppService.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;

    /// <summary>
    /// Hack to deal with the fact that we can't acquire global command line options until
    /// after realising ICommand implementations, meaning we have to complete DI config before
    /// knowing what the tenant and sub ID are. So we need to force deferred creation of the
    /// relevant credentials and settings.
    /// </summary>
    public static class AppServiceManagerSource
    {
        /// <summary>
        /// Gets the app service manager.
        /// </summary>
        /// <param name="authenticationOptions">Authenticate settings.</param>
        /// <param name="subscriptionId">The subscription to manage.</param>
        /// <returns>The app service manager.</returns>
        public static IAppServiceManager Get(
            AuthenticationOptions authenticationOptions,
            string subscriptionId)
        {
            AzureCredentials credentials = authenticationOptions.GetAzureCredentials();
            return AppServiceManager.Authenticate(credentials, subscriptionId);
        }
    }
}
