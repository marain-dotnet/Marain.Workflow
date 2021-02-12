// <copyright file="EventGridCloudEventPublishingServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Marain.Workflows.CloudEventPublishing.EventGrid;
    using Marain.Workflows.CloudEvents;

    /// <summary>
    /// Extension methods to add the <see cref="EventGridCloudEventSink"/> to a service collection.
    /// </summary>
    public static class EventGridCloudEventPublishingServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="EventGridCloudEventSink"/> to the specified service collection, along with
        /// the cloud event sink supporting direct publishing to Uris specified in a workflow definition.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configurationCallback">A callback method that will return the configuration used to connect to event grid.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddEventGridCloudEventPublisherSink(
            this IServiceCollection services,
            Func<IServiceProvider, EventGridConfiguration> configurationCallback)
        {
            Type sinkType = typeof(EventGridCloudEventSink);

            if (services.Any(service => service.ServiceType == sinkType))
            {
                return services;
            }

            services.AddSingleton(configurationCallback);
            services.AddSingleton<ICloudEventPublisherSink, EventGridCloudEventSink>();

            return services;
        }
    }
}
