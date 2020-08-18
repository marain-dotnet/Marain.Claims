// <copyright file="ClaimTypes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    /// <summary>
    /// Constants for well-known claim types.
    /// </summary>
    public static class ClaimTypes
    {
        /// <summary>
        /// 'oid' claim type.
        /// </summary>
        public const string Oid = "oid";

        /// <summary>
        /// 'http://schemas.microsoft.com/identity/claims/objectidentifier' claim type.
        /// </summary>
        public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    }
}
