// <copyright file="DurableFunctionsOpenApiContext.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessagePreProcessingHost.OpenApi
{
    using System;
    using System.Security.Claims;
    using Menes;
    using Microsoft.Azure.WebJobs;

    /// <summary>
    /// OpenApi context that allows the orchestration client to be supplied to functions.
    /// </summary>
    public class DurableFunctionsOpenApiContext : IOpenApiContext
    {
        private DurableFunctionsOpenApiContextAdditionalProperties additionalProperties;

        /// <inheritdoc/>
        public ClaimsPrincipal CurrentPrincipal { get; set; }

        /// <inheritdoc/>
        public string CurrentTenantId { get; set; }

        /// <summary>
        /// Gets or sets the current functions execution context.
        /// </summary>
        public ExecutionContext ExecutionContext
        {
            get => this.additionalProperties.ExecutionContext;
            set => this.additionalProperties.ExecutionContext = value;
        }

        /// <summary>
        /// Gets or sets the current functions durable functions orchestration client.
        /// </summary>
        public DurableOrchestrationClient OrchestrationClient
        {
            get => this.additionalProperties.OrchestrationClient;
            set => this.additionalProperties.OrchestrationClient = value;
        }

        /// <inheritdoc/>
        public object AdditionalContext
        {
            get
            {
                return this.additionalProperties;
            }

            set
            {
                this.additionalProperties = value as DurableFunctionsOpenApiContextAdditionalProperties
                    ?? throw new ArgumentException("AdditionalContext can only be set to a value of type 'DurableFunctionsOpenApiContextAdditionalProperties'");
            }
        }
    }
}
