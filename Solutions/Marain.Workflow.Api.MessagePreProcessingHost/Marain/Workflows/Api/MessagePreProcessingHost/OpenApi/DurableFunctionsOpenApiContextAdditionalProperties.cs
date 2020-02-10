// <copyright file="DurableFunctionsOpenApiContextAdditionalProperties.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessagePreProcessingHost.OpenApi
{
    using Microsoft.Azure.WebJobs;

    /// <summary>
    /// Dto class for passing through the durable functions orchestration client to
    /// OpenApi functions.
    /// </summary>
    public class DurableFunctionsOpenApiContextAdditionalProperties
    {
        /// <summary>
        /// Gets or sets the <see cref="ExecutionContext"/>.
        /// </summary>
        public ExecutionContext ExecutionContext { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DurableOrchestrationClient"/>.
        /// </summary>
        public DurableOrchestrationClient OrchestrationClient { get; set; }
    }
}
