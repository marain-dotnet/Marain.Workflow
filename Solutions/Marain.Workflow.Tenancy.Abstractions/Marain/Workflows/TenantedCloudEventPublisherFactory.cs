// <copyright file="TenantedCloudEventPublisherFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Workflows.CloudEvents;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Standard implementation of the <see cref="ITenantedCloudEventPublisherFactory"/>.
    /// </summary>
    public class TenantedCloudEventPublisherFactory : ITenantedCloudEventPublisherFactory
    {
        private readonly ILogger<CloudEventPublisher> logger;
        private readonly IEnumerable<ICloudEventPublisherSink> eventPublisherSinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudEventPublisher"/> class.
        /// </summary>
        /// <param name="eventPublisherSinks">The set of event publisher sinks in use.</param>
        /// <param name="logger">The logger.</param>
        public TenantedCloudEventPublisherFactory(
            IEnumerable<ICloudEventPublisherSink> eventPublisherSinks,
            ILogger<CloudEventPublisher> logger)
        {
            // TODO: validation & logging if no sinks present.
            this.eventPublisherSinks = eventPublisherSinks;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<CloudEventPublisher> GetCloudEventPublisherAsync(ITenant tenant)
        {
            return Task.FromResult(
                new CloudEventPublisher(
                    tenant.Id,
                    this.eventPublisherSinks,
                    this.logger));
        }
    }
}
