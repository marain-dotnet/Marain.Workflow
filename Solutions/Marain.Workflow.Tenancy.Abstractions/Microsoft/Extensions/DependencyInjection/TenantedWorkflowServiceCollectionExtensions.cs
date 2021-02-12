// <copyright file="TenantedWorkflowServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Corvus.Leasing;
    using Marain.Workflows;
    using Marain.Workflows.CloudEventPublishing.EventGrid;
    using Marain.Workflows.CloudEvents;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service collection extensions to add workflow event hub trigger queueing.
    /// </summary>
    public static class TenantedWorkflowServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the standard <see cref="ITenantedWorkflowEngineFactory"/> implementation to the given
        /// <see cref="IServiceCollection"/>. Tenanted storage needs to be added separately.
        /// </summary>
        /// <param name="collection">The Service Collection to add to.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        /// <remarks>
        /// The default workflow engine has dependencies on
        /// - <see cref="ITenantedWorkflowStoreFactory"/>
        /// - <see cref="ITenantedWorkflowInstanceStoreFactory"/>
        /// - <see cref="ILeaseProvider"/>
        /// - <see cref="ILogger"/>
        /// Implementations of these services must be added to the service collection separately.
        /// </remarks>
        public static IServiceCollection AddTenantedWorkflowEngineFactory(this IServiceCollection collection)
        {
            collection.AddTenantedCloudEventPublisher();

            collection.AddEventGridCloudEventPublisherSink(
                sp =>
                {
                    IConfiguration config = sp.GetRequiredService<IConfiguration>();
                    EventGridConfiguration eventGridConfiguration =
                        config.GetSection("EventGridConfiguration").Get<EventGridConfiguration>()
                        ?? new EventGridConfiguration();

                    eventGridConfiguration.EnsureValid();
                    return eventGridConfiguration;
                });

            collection.AddSingleton(
                sp =>
                {
                    IConfiguration config = sp.GetRequiredService<IConfiguration>();
                    TenantedWorkflowEngineFactoryConfiguration engineFactoryConfiguration =
                        config.GetSection("TenantedWorkflowEngineFactoryConfiguration").Get<TenantedWorkflowEngineFactoryConfiguration>();

                    if (string.IsNullOrEmpty(engineFactoryConfiguration?.CloudEventBaseSourceName))
                    {
                        throw new InvalidOperationException("Cannot find a configuration value called 'TenantedWorkflowEngineFactoryConfiguration:CloudEventBaseSourceName'.");
                    }

                    return engineFactoryConfiguration;
                });

            collection.AddSingleton<ITenantedWorkflowEngineFactory, TenantedWorkflowEngineFactory>();

            return collection;
        }
    }
}