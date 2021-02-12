// <copyright file="EventGridCloudEventSink.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEventPublishing.EventGrid
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Messaging.EventGrid;
    using Corvus.Retry;
    using Marain.Workflows.CloudEvents;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implementation of <see cref="ICloudEventPublisherSink"/> that sends CloudEvents to a supplied list of subscribers.
    /// </summary>
    public class EventGridCloudEventSink : ICloudEventPublisherSink
    {
        private readonly ILogger<EventGridCloudEventSink> logger;
        private readonly EventGridPublisherClient? eventGridPublisherClient;
        private readonly EventGridConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventGridCloudEventSink"/> class.
        /// </summary>
        /// <param name="configuration">Configuration for event grid publishing.</param>
        /// <param name="logger">The logger.</param>
        public EventGridCloudEventSink(
            EventGridConfiguration configuration,
            ILogger<EventGridCloudEventSink> logger)
        {
            this.configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            if (this.configuration.Enabled)
            {
                this.eventGridPublisherClient = new EventGridPublisherClient(
                    this.configuration.TopicEndpoint,
                    new AzureKeyCredential(this.configuration.TopicEndpointKey));
            }
            else
            {
                this.logger.LogWarning("EventGridCloudEventSink is registered but disabled by configuration.");
            }
        }

        /// <inheritdoc/>
        public async Task PublishAsync(
            MarainCloudEvent marainCloudEvent,
            IEnumerable<WorkflowEventSubscription> directSubscriptions,
            CancellationToken cancellationToken)
        {
            if (marainCloudEvent is null)
            {
                throw new ArgumentNullException(nameof(marainCloudEvent));
            }

            if (this.eventGridPublisherClient is null)
            {
                return;
            }

            var cloudEvent = new CloudEvent(
                marainCloudEvent.Source,
                marainCloudEvent.Type,
                marainCloudEvent.Data)
            {
                Id = marainCloudEvent.Id,
                DataContentType = marainCloudEvent.DataContentType,
                Subject = marainCloudEvent.Subject,
                Time = marainCloudEvent.Time,
            };

            cloudEvent.ExtensionAttributes.Add("maraintenantid", marainCloudEvent.MarainTenantId);

            // TODO: Exception handling.
            this.logger.LogDebug(
                "Sending CloudEvent of type '{cloudEventType}' to EventGrid with source '{source}' and subject '{subject}'",
                cloudEvent.Type,
                cloudEvent.Source,
                cloudEvent.Subject);

            await Retriable.RetryAsync(() => this.eventGridPublisherClient.SendEventsAsync(new[] { cloudEvent }, cancellationToken), cancellationToken).ConfigureAwait(false);
        }
    }
}
