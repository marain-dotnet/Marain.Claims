// <copyright file="ResourceAccessRuleSetNotFoundException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1194 // Roslynator's 'all the constructors' fixation

namespace Marain.Claims.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when a resource access rule set with the specified id cannot be found.
    /// </summary>
    [Serializable]
    public class ResourceAccessRuleSetNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAccessRuleSetNotFoundException"/> class.
        /// </summary>
        /// <param name="id">The id of the rule set that couldn't be found.</param>
        /// <param name="innerException"> The inner exception. </param>
        public ResourceAccessRuleSetNotFoundException(string id, Exception innerException)
            : base("Resource access rule set not found", innerException)
        {
            this.ResourceAccessRuleSetIds = new[] { id };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAccessRuleSetNotFoundException"/> class.
        /// </summary>
        /// <param name="id">The id of the rule set that couldn't be found.</param>
        public ResourceAccessRuleSetNotFoundException(string id)
            : base("Resource access rule set not found")
        {
            this.ResourceAccessRuleSetIds = new[] { id };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAccessRuleSetNotFoundException"/> class.
        /// </summary>
        /// <param name="ids">The ids of the rule sets that couldn't be found.</param>
        /// <param name="innerException"> The inner exception. </param>
        public ResourceAccessRuleSetNotFoundException(IEnumerable<string> ids, Exception innerException)
            : base("Resource access rule sets not found", innerException)
        {
            this.ResourceAccessRuleSetIds = ids.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAccessRuleSetNotFoundException"/> class.
        /// </summary>
        /// /// <param name="info"> The serialization info. </param>
        /// <param name="context"> The context. </param>
        protected ResourceAccessRuleSetNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the id of the <see cref="ResourceAccessRuleSet"/> that couldn't be found.
        /// </summary>
        public string[] ResourceAccessRuleSetIds { get; }
    }
}