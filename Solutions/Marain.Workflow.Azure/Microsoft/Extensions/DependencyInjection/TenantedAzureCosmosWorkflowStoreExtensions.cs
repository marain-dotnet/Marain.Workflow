// <copyright file="TenantedAzureCosmosWorkflowStoreExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Service collection extensions to add workflow stores.
    /// </summary>
    public static class TenantedAzureCosmosWorkflowStoreExtensions
    {
        /// <summary>
        /// Adds multitenant-capable stores for the workflow engine.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Configuration section to read root tenant default repository settings from.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddTenantedAzureCosmosWorkflowStore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services;
        }
    }
}
