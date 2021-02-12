// <copyright file="CloudEventPublisher.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Publishes cloud events to HTTP endpoints.
    /// </summary>
    public class CloudEventPublisher : ICloudEventDataPublisher
    {
        private readonly ILogger<CloudEventPublisher> logger;
        private readonly string marainTenantId;
        private readonly IList<ICloudEventPublisherSink> sinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudEventPublisher"/> class.
        /// </summary>
        /// <param name="marainTenantId">The Marain Tenant Id to add to published events as an extension.</param>
        /// <param name="sinks">The publisher sinks that will be used to send events.</param>
        /// <param name="logger">The logger.</param>
        public CloudEventPublisher(
            string marainTenantId,
            IEnumerable<ICloudEventPublisherSink> sinks,
            ILogger<CloudEventPublisher> logger)
        {
            this.marainTenantId = marainTenantId;

            if (string.IsNullOrEmpty(marainTenantId))
            {
                throw new ArgumentNullException(nameof(marainTenantId));
            }

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.sinks = sinks?.ToList()
                ?? throw new ArgumentNullException(nameof(sinks));
        }

        /// <inheritdoc/>
        public async Task PublishWorkflowEventDataAsync<T>(
            string source,
            string eventType,
            string subject,
            string dataContentType,
            T eventData,
            IEnumerable<WorkflowEventSubscription> subscriptions)
        {
            var cloudEvent = new MarainCloudEvent(
                Guid.NewGuid().ToString(),
                source,
                subject,
                DateTimeOffset.UtcNow,
                eventType,
                this.marainTenantId,
                dataContentType,
                eventData);

            // TODO: Error handling.
            await Task.WhenAll(
                this.sinks.Select(
                    s => s.PublishAsync(cloudEvent, subscriptions, CancellationToken.None))).ConfigureAwait(false);
        }
    }
}
