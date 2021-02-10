// <copyright file="TenantedNEventStoreWorkflowInstanceStoreServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Corvus.ContentHandling;
    using Marain.Workflows;
    using Marain.Workflows.Internal;
    using NEventStore.Persistence.CosmosDb.Internal;

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
            AddCommon(services);

            services.AddSingleton<ITenantedNEventStoreFactory, TenantedInMemoryNEventStoreFactory>();

            return services;
        }

        /// <summary>
        /// Adds a CosmosDb backed event store implementation of <see cref="ITenantedWorkflowInstanceStoreFactory"/> to the
        /// supplied <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The collection.</param>
        /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddCosmosDbWorkflowInstanceEventStore(
            this IServiceCollection services)
        {
            AddCommon(services);

            services.AddSingleton<ITenantedNEventStoreFactory, TenantedCosmosDbNEventStoreFactory>();

            return services;
        }

        private static void AddCommon(IServiceCollection services)
        {
            // TODO: Put this somewhere more sensible.
            // Problem is - it's specific to how we create our clients, so doesn't belong in the NEventStore code.
            services.AddSingleton<ITenantedWorkflowInstanceStoreFactory, TenantedNEventStoreWorkflowInstanceStoreFactory>();

            services.AddContent(factory =>
            {
                factory.RegisterContent<CosmosDbCommit>();
                factory.RegisterContent<CosmosDbSnapshot>();
            });
        }
    }
}
