// <copyright file="TenantedNEventStoreWorkflowInstanceStoreServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Marain.Workflows;

    /// <summary>
    /// Service collection extensions to add NEventStore based implementations of
    /// <see cref="ITenantedWorkflowInstanceStoreFactory"/> to the service collection.
    /// </summary>
    public static class TenantedNEventStoreWorkflowInstanceStoreServiceCollectionExtensions
    {
        /// <summary>
        /// Adds an in-memory event store implementation of <see cref="ITenantedWorkflowInstanceStoreFactory"/> to the
        /// supplied <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The collection.</param>
        /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddInMemoryWorkflowInstanceEventStore(
            this IServiceCollection services)
        {
            services.AddSingleton<ITenantedWorkflowInstanceStoreFactory, TenantedInMemoryNEventStoreWorkflowInstanceStoreFactory>();

            return services;
        }
    }
}
