// <copyright file="AuthenticationOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool
{
    using System;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Core;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;

    using Corvus.Identity.ClientAuthentication.Azure;

    using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceIdentityAzureTokenCredentialSource serviceIdAzureTokenCredentialSource;

        /// <summary>
        /// Creates an <see cref="AuthenticationOptions"/>.
        /// </summary>
        /// <param name="tenantId">The Azure AD tenant against which to authenticate.</param>
        /// <param name="serviceIdAzureTokenCredentialSource">Provides Azure token credentials representing the application.</param>
        /// <param name="azureServiceTokenProviderConnectionString">The connection string for the azure service token provide if additional claims are needed.</param>
        private AuthenticationOptions(
            string tenantId,
            IServiceIdentityAzureTokenCredentialSource serviceIdAzureTokenCredentialSource,
            string azureServiceTokenProviderConnectionString)
        {
            this.TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            this.serviceIdAzureTokenCredentialSource = serviceIdAzureTokenCredentialSource;
            this.AzureServiceTokenProviderConnectionString = azureServiceTokenProviderConnectionString;
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

        /// <summary>
        /// Gets an <see cref="TokenCredential"/> for the specified subscription.
        /// </summary>
        /// <returns>
        /// A task that produces an <see cref="TokenCredential"/> configured based on the command
        /// line options.
        /// </returns>
        public TokenCredential GetAzureCredentials()
        {
            if (string.IsNullOrEmpty(this.AzureServiceTokenProviderConnectionString))
            {
                var deviceCredentialInformation = new DeviceCodeCredentialOptions
                {
                    TenantId = this.TenantId,
                    ClientId = MarainClaimsSetupAADClientId,
                };

                return new DeviceCodeCredential(deviceCredentialInformation);
            }
            else
            {
                return new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    TenantId = this.TenantId,
                });
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
            TokenCredential creds = await this.serviceIdAzureTokenCredentialSource.GetTokenCredentialAsync().ConfigureAwait(false);
            var secretClient = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net/"), creds);
            Response<KeyVaultSecret> getSecretResponse = await secretClient.GetSecretAsync(claimsSetupAppCredentialsSecretName);
            KeyVaultSecret accountKey = getSecretResponse.Value;

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

            var services = new ServiceCollection();
            services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(
                azureServiceTokenProviderConnectionString ?? string.Empty);
            ServiceProvider sp = services.BuildServiceProvider();

            return new AuthenticationOptions(
                tenantId,
                sp.GetRequiredService<IServiceIdentityAzureTokenCredentialSource>(),
                azureServiceTokenProviderConnectionString);
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