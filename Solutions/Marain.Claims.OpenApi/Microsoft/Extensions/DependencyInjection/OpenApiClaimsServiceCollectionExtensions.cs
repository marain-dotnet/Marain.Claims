// <copyright file="OpenApiClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.OpenApi.Claims
{
    using System;
    using Marain.Claims.Client;
    using Menes;
    using Menes.AccessControlPolicies;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extension methods for configuring OpenApi claims behaviours.
    /// </summary>
    public static class OpenApiClaimsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required to enable role-based OpenApi access control. Requires an
        /// implementation of <see cref="IClaimsService"/> to be registered.
        /// </summary>
        /// <param name="services">The service collection to which to add services.</param>
        /// <param name="resourcePrefix">
        /// An optional prefix to add to the URI path when forming the Resource URI that will be
        /// passed when asking the Claims service what permissions each role has for accessing
        /// the resrouce.
        /// </param>
        /// <param name="allowOnlyIfAll">
        /// Configures the behaviour when multiple <c>roles</c> claims are present, and the Claims
        /// service reports different permissions for the different roles. If false, permission
        /// will be granted as long as at least one role grants access. If true, all roles must
        /// grant access (and at least one <c>roles</c> claim must be present in either case).
        /// </param>
        /// <returns>The modified service collection.</returns>
        /// <remarks>
        /// <para>
        /// See <see cref="RoleBasedOpenApiAccessControlPolicy"/> for details on how this works.
        /// </para>
        /// <para>
        /// You will typically use <see cref="ClaimsClientServiceCollectionExtensions.AddClaimsClient(IServiceCollection, Uri, string)"/>
        /// to configure the <see cref="IClaimsService"/> that this requires.
        /// </para>
        /// </remarks>
        public static IServiceCollection AddRoleBasedOpenApiAccessControl(
            this IServiceCollection services,
            string resourcePrefix = null,
            bool allowOnlyIfAll = false)
        {
            services.AddSingleton<IOpenApiAccessControlPolicy>(sp =>
                new RoleBasedOpenApiAccessControlPolicy(
                    sp.GetRequiredService<IClaimsService>(),
                    sp.GetRequiredService<ILogger<RoleBasedOpenApiAccessControlPolicy>>(),
                    resourcePrefix,
                    allowOnlyIfAll));

            return services;
        }

        /// <summary>
        /// Adds services required to enable role-based OpenApi access control, with the ability
        /// to exempt some operations without the overhead of invoking the service. Requires an
        /// implementation of <see cref="IClaimsService"/> to be registered.
        /// </summary>
        /// <param name="services">The service collection to which to add services.</param>
        /// <param name="exemptionPolicy">
        /// An access control policy that will be evaluated before attempting to contact the Claims
        /// service. If this exemption policy allows the request, then the request will be allowed
        /// through without asking the Claims service to evaluate permission.
        /// </param>
        /// <param name="resourcePrefix">
        /// An optional prefix to add to the URI path when forming the Resource URI that will be
        /// passed when asking the Claims service what permissions each role has for accessing
        /// the resrouce.
        /// </param>
        /// <param name="allowOnlyIfAll">
        /// Configures the behaviour when multiple <c>roles</c> claims are present, and the Claims
        /// service reports different permissions for the different roles. If false, permission
        /// will be granted as long as at least one role grants access. If true, all roles must
        /// grant access (and at least one <c>roles</c> claim must be present in either case).
        /// </param>
        /// <returns>The modified service collection.</returns>
        /// <remarks>
        /// <para>
        /// See <see cref="RoleBasedOpenApiAccessControlPolicy"/> for details on how this works.
        /// </para>
        /// <para>
        /// You will typically use <see cref="ClaimsClientServiceCollectionExtensions.AddClaimsClient(IServiceCollection, Uri, string)"/>
        /// to configure the <see cref="IClaimsService"/> that this requires.
        /// </para>
        /// </remarks>
        public static IServiceCollection AddRoleBasedOpenApiAccessControlWithPreemptiveExemptions(
            this IServiceCollection services,
            IOpenApiAccessControlPolicy exemptionPolicy,
            string resourcePrefix = null,
            bool allowOnlyIfAll = false)
        {
            services.AddSingleton<IOpenApiAccessControlPolicy>(sp =>
            {
                IOpenApiAccessControlPolicy roleBasedPolicy = new RoleBasedOpenApiAccessControlPolicy(
                                sp.GetRequiredService<IClaimsService>(),
                                sp.GetRequiredService<ILogger<RoleBasedOpenApiAccessControlPolicy>>(),
                                resourcePrefix,
                                allowOnlyIfAll);
                return new ShortCircuitingAccessControlPolicyAdapter(
                    exemptionPolicy,
                    new[] { roleBasedPolicy });
            });

            return services;
        }
    }
}
