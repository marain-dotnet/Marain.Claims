// <copyright file="AuthenticationOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool
{
    using System;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.KeyVault.Models;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Rest;
    using Microsoft.Rest.Azure.Authentication;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Handles authentication options common to all commands.
    /// </summary>
    public sealed class AuthenticationOptions
    {
        // Client ID for the Azure AD Application created specially for this tool, enabling it to
        // log users in via AAD. We use this to obtain access tokens to Azure in cases where the
        // user does not want to use the Azure CLI authentication mechanism.
        private const string MarainClaimsSetupAADClientId = "7c814853-82dc-4b38-93e3-f4e627927e00";
        private const string AzureManagementResourceId = "https://management.core.windows.net/";
        private const string GraphApiResourceId = "https://graph.windows.net/";
        private readonly Lazy<AzureServiceTokenProvider> azureServiceTokenProvider;

        /// <summary>
        /// Creates an <see cref="AuthenticationOptions"/>.
        /// </summary>
        /// <param name="tenantId">The Azure AD tenant against which to authenticate.</param>
        /// <param name="azureServiceTokenProviderConnectionString">The connection string for the azure service token provide if additional claims are needed.</param>
        private AuthenticationOptions(
            string tenantId,
            string azureServiceTokenProviderConnectionString)
        {
            this.TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            this.AzureServiceTokenProviderConnectionString = azureServiceTokenProviderConnectionString;

            this.azureServiceTokenProvider =
            new Lazy<AzureServiceTokenProvider>(() => new AzureServiceTokenProvider(this.AzureServiceTokenProviderConnectionString));
        }

        /// <summary>
        /// Gets the id of the Azure AD tenant against which to authenticate.
        /// </summary>
        public string TenantId { get; }

        /// <summary>
        /// Gets the connection string to pass to <c>AzureServiceTokenProvider</c>.
        /// </summary>
        /// <remarks>
        /// This enables use of the cached token from Azure CLI (refresh the cached token using
        /// <c>az account get-access-token</c>) or, in scenarios where it's available, the MSI.
        /// </remarks>
        public string AzureServiceTokenProviderConnectionString { get; }

        private AzureServiceTokenProvider TokenProvider => this.azureServiceTokenProvider.Value;

        /// <summary>
        /// Gets an <see cref="AzureCredentials"/> for the specified subscription.
        /// </summary>
        /// <returns>
        /// A task that produces an <see cref="AzureCredentials"/> configured based on the command
        /// line options.
        /// </returns>
        public AzureCredentials GetAzureCredentials()
        {
            if (string.IsNullOrEmpty(this.AzureServiceTokenProviderConnectionString))
            {
                var deviceCredentialInformation = new DeviceCredentialInformation
                {
                    ClientId = MarainClaimsSetupAADClientId,
                    DeviceCodeFlowHandler = DeviceCodeFlowCallback,
                };

                return new AzureCredentials(deviceCredentialInformation, this.TenantId, AzureEnvironment.AzureGlobalCloud);
            }
            else
            {
                ServiceClientCredentials azureTokenCredentials = this.GetAzureServiceClientCredentials(AzureManagementResourceId);
                ServiceClientCredentials graphTokenCredentials = this.GetAzureServiceClientCredentials(GraphApiResourceId);
                return new AzureCredentials(azureTokenCredentials, graphTokenCredentials, this.TenantId, AzureEnvironment.AzureGlobalCloud);
            }
        }

        /// <summary>
        /// Gets service client credentials by looking them up in a key vault.
        /// </summary>
        /// <param name="resourceId">
        /// The resource id being accessed - typically the AppID of the Azure AD Application
        /// configured for EasyAuth.
        /// </param>
        /// <param name="keyVaultName">
        /// The name of the key vault that contains the credentials.
        /// </param>
        /// <param name="claimsSetupAppCredentialsSecretName">
        /// The name of the secret containing the credentials.
        /// </param>
        /// <returns>
        /// A task that produces service client credentials that authenticate using the details in
        /// the key vault.
        /// </returns>
        public async Task<ServiceClientCredentials> GetServiceClientCredentialsFromKeyVault(
            string resourceId,
            string keyVaultName,
            string claimsSetupAppCredentialsSecretName)
        {
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(this.TokenProvider.KeyVaultTokenCallback));

            SecretBundle accountKey = await keyVaultClient
                     .GetSecretAsync($"https://{keyVaultName}.vault.azure.net/secrets/{claimsSetupAppCredentialsSecretName}")
                     .ConfigureAwait(false);

            var credentials = JObject.Parse(accountKey.Value);
            string appId = credentials["appId"].Value<string>();
            string secret = credentials["secret"].Value<string>();

            var authContext = new AuthenticationContext(ActiveDirectoryServiceSettings.Azure.AuthenticationEndpoint + this.TenantId);

            return new TokenCredentials(new CallbackTokenProvider(
                async () =>
                {
                    AuthenticationResult authResult = await authContext.AcquireTokenAsync(
                        resourceId,
                        new ClientCredential(appId, secret))
                        .ConfigureAwait(false);
                    return authResult.AccessToken;
                }));
        }

        /// <summary>
        /// Build authentication options from settings.
        /// </summary>
        /// <param name="useAzCliDevAuth">Whether to use Azure Dev Authorization.</param>
        /// <param name="tenantId">The Azure AD tenant ID.</param>
        /// <returns>A new instance of the configured authentication options.</returns>
        internal static AuthenticationOptions BuildFrom(bool useAzCliDevAuth, string tenantId)
        {
            string azureServiceTokenProviderConnectionString = null;
            if (useAzCliDevAuth)
            {
                // We could also consider other MSI connection strings. E.g., there's
                //  RunAs=App;
                // That's for when you are running in a context where an MSI is available, although
                // I'm not sure we'd actually be running this tool in such a context. (It's normally
                // the string to use inside of a deployed Function or Web App.)
                // Or more plausibly we might want to use:
                //  RunAs=App;AppId={AppId};TenantId={TenantId};AppKey={ClientSecret}
                // https://docs.microsoft.com/en-us/azure/key-vault/service-to-service-authentication
                azureServiceTokenProviderConnectionString = "RunAs=Developer; DeveloperTool=AzureCLI";
            }

            return new AuthenticationOptions(azureServiceTokenProviderConnectionString, tenantId);
        }

        /// <summary>
        /// Callback used when the device code flow is in use and we need to prompt the user to
        /// visit the relevant web page and type in a code.
        /// </summary>
        /// <param name="result">
        /// Information about the login attempt, including the code the user will need to fill in.
        /// </param>
        /// <returns>True, because we never fail.</returns>
        private static bool DeviceCodeFlowCallback(DeviceCodeResult result)
        {
            Console.WriteLine(result.Message);
            return true;
        }

        /// <summary>
        /// Determines whether a particular resource is one that we're able to obtain tokens for
        /// through Azure CLI.
        /// </summary>
        /// <param name="resourceId">
        /// The resource ID that will be passed when asking AAD for a token.
        /// </param>
        /// <returns>
        /// <para>
        /// True if this is a resource that the Azure CLI (<c>az</c>) can obtain tokens for, false
        /// if not.
        /// </para>
        /// <para>
        /// Azure CLI is what Microsoft often call a 'first party' app, meaning that it comes from
        /// Microsoft, and as such, it can do things that we cannot do with our own apps. For
        /// example, Microsoft can pre-approve first party applications for anyone and everyone:
        /// often when signing in through a Microsoft AAD Application you do not see a consent
        /// prompt in cases where you would expect to if you'd written your own application.
        /// </para>
        /// <para>
        /// Azure CLI is pre-approved for use with Azure Resource Manager and the Graph API,
        /// making it a low-friction way of obtaining tokens for those services. However, it
        /// apparently can't obtain tokens for other applications. So when we attempt to obtain a
        /// token that enables us to access the Claims service (by specifying its associated Azure
        /// AD application id as the resource id) the <c>az</c> approach fails.
        /// </para>
        /// </returns>
        private static bool IsAzCliAccessibleResource(string resourceId) =>
            resourceId == AzureManagementResourceId || resourceId == GraphApiResourceId;

        /// <summary>
        /// Gets a <see cref="ServiceClientCredentials"/> for the specified resource based on the
        /// configuration options.
        /// </summary>
        /// <param name="resourceId">
        /// Identifies the resource to be accessed. This must be either the ARM or the Graph API
        /// resource id.
        /// </param>
        /// <returns>A <see cref="ServiceClientCredentials"/>.</returns>
        private ServiceClientCredentials GetAzureServiceClientCredentials(string resourceId)
        {
            if (!IsAzCliAccessibleResource(resourceId))
            {
                throw new ArgumentException($"Cannot access non-Azure resource {resourceId} this way", nameof(resourceId));
            }

            return new TokenCredentials(new CallbackTokenProvider(
                () => this.TokenProvider.GetAccessTokenAsync(resourceId)));
        }

        private class CallbackTokenProvider : ITokenProvider
        {
            private readonly Func<Task<string>> getTokenCallback;

            public CallbackTokenProvider(
                Func<Task<string>> getTokenCallback)
            {
                this.getTokenCallback = getTokenCallback;
            }

            public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(
                CancellationToken cancellationToken)
            {
                string token = await this.getTokenCallback().ConfigureAwait(false);
                return new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
