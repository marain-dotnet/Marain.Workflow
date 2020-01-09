// <copyright file="HighTrafficWorkflowMessageIngestionServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Menes;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Extension methods for configuring DI for the the high traffic ingestion Workflow OpenApi services.
    /// </summary>
    public static class HighTrafficWorkflowMessageIngestionServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required by the high traffic ingestion endpoint.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="rootTenantDefaultConfiguration">
        /// Configuration section to read root tenant default repository settings from.
        /// </param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddHighTrafficWorkflowMessageIngestionApi(
            this IServiceCollection services,
            IConfiguration rootTenantDefaultConfiguration,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            services.AddAzureEventHubWorkflowTriggerQueue();
            return services;
        }
    }
}
