// <copyright file="ExecutionContextExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessagePreProcessingHost.Shared
{
    using Microsoft.Azure.WebJobs;

    /// <summary>
    /// The execution context extensions.
    /// </summary>
    public static class ExecutionContextExtensions
    {
        /// <summary>
        /// The get function name for bespoke start operation.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFunctionNameForBespokeStartOperation(this ExecutionContext context)
        {
            return context.FunctionName + "-internal";
        }
    }
}