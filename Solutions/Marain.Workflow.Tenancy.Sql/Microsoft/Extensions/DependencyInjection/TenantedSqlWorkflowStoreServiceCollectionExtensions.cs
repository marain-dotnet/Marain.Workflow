// <copyright file="TenantedSqlWorkflowStoreServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;

    using Corvus.Extensions.Json;
    using Corvus.Storage.Sql;

    using Marain.Workflows;

    /// <summary>
    /// Service collection extensions to add the Sql implementation of workflow stores.
    /// </summary>
    public static class TenantedSqlWorkflowStoreServiceCollectionExtensions
    {
        /// <summary>
        /// Gets the tenant properties key under which the SQL Database configuration will be
        /// stored.
        /// </summary>
        public static string WorkflowConnectionKey { get; } = "workflow";

        /// <summary>
        /// Adds Sql-based implementation of <see cref="ITenantedWorkflowStoreFactory"/> to the service container.
        /// </summary>
        /// <param name="services">The collection.</param>
        /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddTenantedSqlWorkflowStore(
            this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType is ITenantedWorkflowStoreFactory))
            {
                return services;
            }

            services.AddTenantSqlConnectionFactory();
            services.AddSingleton<ITenantedWorkflowStoreFactory>(svc => new TenantedSqlWorkflowStoreFactory(
                svc.GetRequiredService<IJsonSerializerSettingsProvider>(),
                svc.GetRequiredService<ISqlConnectionFromDynamicConfiguration>(),
                WorkflowConnectionKey));

            return services;
        }

        /// <summary>
        /// Adds Sql-based implementation of <see cref="ITenantedWorkflowStoreFactory"/> to the service container.
        /// </summary>
        /// <param name="services">The collection.</param>
        /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddTenantedSqlWorkflowInstanceStore(
            this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType is ITenantedWorkflowInstanceStoreFactory))
            {
                return services;
            }

            services.AddTenantSqlConnectionFactory();
            services.AddSingleton<ITenantedWorkflowInstanceStoreFactory>(svc => new TenantedSqlWorkflowInstanceStoreFactory(
                svc.GetRequiredService<IJsonSerializerSettingsProvider>(),
                svc.GetRequiredService<ISqlConnectionFromDynamicConfiguration>(),
                WorkflowConnectionKey));

            return services;
        }
    }
}