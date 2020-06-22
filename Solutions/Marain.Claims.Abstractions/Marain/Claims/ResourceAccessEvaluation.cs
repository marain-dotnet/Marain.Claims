// <copyright file="ResourceAccessEvaluation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    /// <summary>
    /// This describes an evaluation of a <see cref="ResourceAccessSubmission"/>.
    /// </summary>
    public class ResourceAccessEvaluation
    {
        /// <summary>
        /// Gets or sets the submission for which this is the evaluated result.
        /// </summary>
        public ResourceAccessSubmission Submission { get; set; }

        /// <summary>
        /// Gets or sets the result of the resource access evaluation.
        /// </summary>
        public PermissionResult Result { get; set; }
    }
}
