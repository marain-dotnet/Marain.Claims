﻿// <copyright file="OpenApiClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Marain.Claims.Client;
    using Marain.Claims.OpenApi;
    using Menes;

    /// <summary>
    /// Extension methods for configuring OpenApi claims behaviours on top of a Marain Claims service.
    /// </summary>
    public static class OpenApiClaimsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required to enable role-based OpenApi access control on top of
        /// a Marain Claims service. Requires an implementation of <see cref="IClaimsService"/> to be
        /// registered.
        /// </summary>
        /// <param name="services">The service collection to which to add services.</param>
        /// <param name="resourcePrefix">
        /// An optional prefix to add to the URI path when forming the Resource URI that will be
        /// passed when asking the Claims service what permissions each role has for accessing
        /// the resource.
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
        /// See <see cref="OpenApiAccessControlPolicy"/> for details on how this works.
        /// </para>
        /// <para>
        /// You will typically use <see cref="ClaimsClientServiceCollectionExtensions.AddClaimsClient(IServiceCollection, Func{IServiceProvider, ClaimsClientOptions})"/>
        /// to configure the <see cref="IClaimsService"/> that this requires.
        /// </para>
        /// </remarks>
        public static IServiceCollection AddClaimsClientRoleBasedOpenApiAccessControl(
            this IServiceCollection services,
            string resourcePrefix = null,
            bool allowOnlyIfAll = false)
        {
            services.AddRoleBasedOpenApiAccessControl(resourcePrefix, allowOnlyIfAll);

            services.AddSingleton<IResourceAccessEvaluator, OpenApiClientResourceAccessEvaluator>();

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
        /// See <see cref="OpenApiAccessControlPolicy"/> for details on how this works.
        /// </para>
        /// <para>
        /// You will typically use <see cref="ClaimsClientServiceCollectionExtensions.AddClaimsClient(IServiceCollection, Func{IServiceProvider, ClaimsClientOptions})"/>
        /// to configure the <see cref="IClaimsService"/> that this requires.
        /// </para>
        /// </remarks>
        public static IServiceCollection AddClaimsClientRoleBasedOpenApiAccessControlWithPreemptiveExemptions(
            this IServiceCollection services,
            IOpenApiAccessControlPolicy exemptionPolicy,
            string resourcePrefix = null,
            bool allowOnlyIfAll = false)
        {
            services.AddRoleBasedOpenApiAccessControlWithPreemptiveExemptions(exemptionPolicy, resourcePrefix, allowOnlyIfAll);

            services.AddSingleton<IResourceAccessEvaluator, OpenApiClientResourceAccessEvaluator>();

            return services;
        }

        /// <summary>
        /// Adds services required to enable identity-based OpenApi access control on top of
        /// a Marain Claims service. Requires an implementation of <see cref="IClaimsService"/> to be
        /// registered.
        /// </summary>
        /// <param name="services">The service collection to which to add services.</param>
        /// <param name="resourcePrefix">
        /// An optional prefix to add to the URI path when forming the Resource URI that will be
        /// passed when asking the Claims service what permissions each role has for accessing
        /// the resource.
        /// </param>
        /// <param name="allowOnlyIfAll">
        /// Configures the behaviour when multiple <c>oid</c> claims are present, and the Claims
        /// service reports different permissions for the different oids. If false, permission
        /// will be granted as long as at least one oid grants access. If true, all oids must
        /// grant access (and at least one <c>oid</c> claim must be present in either case).
        /// </param>
        /// <returns>The modified service collection.</returns>
        /// <remarks>
        /// <para>
        /// See <see cref="OpenApiAccessControlPolicy"/> for details on how this works.
        /// </para>
        /// <para>
        /// You will typically use <see cref="ClaimsClientServiceCollectionExtensions.AddClaimsClient(IServiceCollection, Func{IServiceProvider, ClaimsClientOptions})"/>
        /// to configure the <see cref="IClaimsService"/> that this requires.
        /// </para>
        /// </remarks>
        public static IServiceCollection AddClaimsClientIdentityBasedOpenApiAccessControl(
            this IServiceCollection services,
            string resourcePrefix = null,
            bool allowOnlyIfAll = false)
        {
            services.AddIdentityBasedOpenApiAccessControl(resourcePrefix, allowOnlyIfAll);

            services.AddSingleton<IResourceAccessEvaluator, OpenApiClientResourceAccessEvaluator>();

            return services;
        }

        /// <summary>
        /// Adds services required to enable identity-based OpenApi access control, with the ability
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
        /// Configures the behaviour when multiple <c>oid</c> claims are present, and the Claims
        /// service reports different permissions for the different oids. If false, permission
        /// will be granted as long as at least one oid grants access. If true, all oids must
        /// grant access (and at least one <c>oid</c> claim must be present in either case).
        /// </param>
        /// <returns>The modified service collection.</returns>
        /// <remarks>
        /// <para>
        /// See <see cref="OpenApiAccessControlPolicy"/> for details on how this works.
        /// </para>
        /// <para>
        /// You will typically use <see cref="ClaimsClientServiceCollectionExtensions.AddClaimsClient(IServiceCollection, Func{IServiceProvider, ClaimsClientOptions})"/>
        /// to configure the <see cref="IClaimsService"/> that this requires.
        /// </para>
        /// </remarks>
        public static IServiceCollection AddClaimsClientIdentityBasedOpenApiAccessControlWithPreemptiveExemptions(
            this IServiceCollection services,
            IOpenApiAccessControlPolicy exemptionPolicy,
            string resourcePrefix = null,
            bool allowOnlyIfAll = false)
        {
            services.AddIdentityBasedOpenApiAccessControlWithPreemptiveExemptions(exemptionPolicy, resourcePrefix, allowOnlyIfAll);

            services.AddSingleton<IResourceAccessEvaluator, OpenApiClientResourceAccessEvaluator>();

            return services;
        }
    }
}