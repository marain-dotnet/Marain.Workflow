// <copyright file="CloudEventPublisherServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Workflows.CloudEvents;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extension methods to add the <see cref="CloudEventPublisher"/> to a service collection.
    /// </summary>
    public static class CloudEventPublisherServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="CloudEventPublisher"/> to the specified service collection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddCloudEventPublisher(this IServiceCollection services)
        {
            Type cloudEventPublisherType = typeof(ICloudEventDataPublisher);

            if (services.Any(service => service.ServiceType == cloudEventPublisherType))
            {
                return services;
            }

            services.AddSingleton<ICloudEventDataPublisher>(sp =>
            {
                return new CloudEventPublisher(
                    HttpClientFactory.Create(),
                    sp.GetRequiredService<IServiceIdentityTokenSource>(),
                    sp.GetRequiredService<IJsonSerializerSettingsProvider>(),
                    sp.GetRequiredService<ILogger<CloudEventPublisher>>());
            });

            return services;
        }
    }
}
