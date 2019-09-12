// <copyright file="Initializer.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Endjin.Claims.Functions
{
    using System;
    using Endjin.Claims.ClaimsProviderStrategies;
    using Endjin.Claims.Client;
    using Endjin.Claims.Serialization.Json.Installers;
    using Endjin.Logging;
    using Endjin.OpenApi;
    using Endjin.OpenApi.AccessControlPolicies;
    using Endjin.OpenApi.Claims;
    using Endjin.OpenApi.Tenancy.TenantProviderStrategies;
    using Endjin.Telemetry;
    using Endjin.Tenancy;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using Functions = Endjin.Functions;

    /// <summary>
    /// Shared initialization code for the functions in this project.
    /// </summary>
    public static class Initializer
    {
        /// <summary>
        /// Initialises services required for the functions app.
        /// </summary>
        /// <param name="context">The request context.</param>
        public static void Initialize(ExecutionContext context)
        {
            TelemetryOperationContext.Initialize(context.InvocationId.ToString(), context.FunctionName, "Endjin.Claims.EndjinHost");
            Functions.InitializeContainer(context, (services, configuration) =>
            {
                services.AddOpenApiHttpRequestHosting(host =>
                {
                    host.Documents.RegisterOpenApiServiceWithEmbeddedDefinition(
                        typeof(ClaimPermissionsService).Assembly,
                        "Endjin.Claims.ClaimsServices.yaml");
                    host.Documents.AddSwaggerEndpoint();
                });

                LoggerConfiguration loggerConfig = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Warning()
                    .Enrich.WithProperty("InvocationId", context.InvocationId)
                    .Enrich.With<EventIdEnricher>();

                string appInsightsKey = configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                if (appInsightsKey != null)
                {
                    loggerConfig.WriteTo.ApplicationInsightsEvents(appInsightsKey);
                }
                else
                {
                    loggerConfig.WriteTo.Console().MinimumLevel.Debug();
                }

                Log.Logger = loggerConfig.CreateLogger();

                services.AddEndjinJsonConverters();
                services.AddAzureFunctionsConfiguration(context);
                services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
                services.AddTelemetry();
                services.AddClaimsConverters();
                services.AddClaimsTenancyRepositoryStorage(configuration);
                services.AddTenantKeyVaultOrConfigurationAccountKeyProvider();
                services.AddSingleton<ITenantStore, TenantStore>();

                services.AddOpenApiTenancy();
                services.AddTenantProviderStrategy<RootTenantProviderStrategy>();
                services.AddOpenApiClaims();

#if DEBUG
                services.AddClaimsProviderStrategy<UnsafeJwtAuthorizationBearerTokenStrategy>();
#endif

                services.AddClaimsProviderStrategy<EasyAuthJwtStrategy>();

                string[] openOperationIds =
                {
                    ClaimPermissionsService.GetClaimPermissionsPermissionOperationId,
                    ClaimPermissionsService.GetClaimPermissionsPermissionBatchOperationId,
                    ClaimPermissionsService.InitializeTenantOperationId,
                    OpenApi.Internal.SwaggerService.SwaggerOperationId,
                };
                services.AddRoleBasedOpenApiAccessControlWithPreemptiveExemptions(
                    new ExemptOperationIdsAccessPolicy(openOperationIds),
                    ClaimPermissionsService.ClaimsResourcePrefix);
                services.AddSingleton<IClaimsService, LocalClaimsService>();

                services.AddSingleton<ClaimPermissionsService>();
                services.AddSingleton((Func<IServiceProvider, IOpenApiService>)(sp => sp.GetRequiredService<ClaimPermissionsService>()));
                services.AddSingleton<IClaimPermissionsEvaluator, PermissionsEvaluatorBridge>();
                services.AddSingleton<IOpenApiService, ResourceAccessRuleSetService>();
            });
        }
    }
}
