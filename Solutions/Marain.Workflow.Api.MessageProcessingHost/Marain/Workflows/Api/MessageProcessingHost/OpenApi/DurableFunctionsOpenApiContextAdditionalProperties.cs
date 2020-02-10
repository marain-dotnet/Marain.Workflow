// <copyright file="DurableFunctionsOpenApiContextAdditionalProperties.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessageProcessingHost.OpenApi
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;

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
        /// Gets or sets the <see cref="IDurableOrchestrationClient"/>.
        /// </summary>
        public IDurableOrchestrationClient OrchestrationClient { get; set; }
    }
}
