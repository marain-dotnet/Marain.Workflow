// <copyright file="DirectSubscriberCloudEventSink.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEvents.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Retry;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using NodaTime;
    using NodaTime.Text;

    /// <summary>
    /// Implementation of <see cref="ICloudEventPublisherSink"/> that sends CloudEvents to a supplied list of subscribers.
    /// </summary>
    public class DirectSubscriberCloudEventSink : ICloudEventPublisherSink
    {
        private readonly HttpClient httpClient;
        private readonly IServiceIdentityTokenSource serviceIdentityTokenSource;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly ILogger<DirectSubscriberCloudEventSink> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectSubscriberCloudEventSink"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use when POSTing event data to subscribers.</param>
        /// <param name="serviceIdentityTokenSource">The <see cref="IServiceIdentityTokenSource"/> that will be used to aquire authentication tokens.</param>
        /// <param name="serializerSettingsProvider">The current <see cref="IJsonSerializerSettingsProvider"/>.</param>
        /// <param name="logger">The logger.</param>
        public DirectSubscriberCloudEventSink(
            HttpClient httpClient,
            IServiceIdentityTokenSource serviceIdentityTokenSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ILogger<DirectSubscriberCloudEventSink> logger)
        {
            this.httpClient = httpClient
                ?? throw new ArgumentNullException(nameof(httpClient));
            this.serviceIdentityTokenSource = serviceIdentityTokenSource
                ?? throw new ArgumentNullException(nameof(serviceIdentityTokenSource));
            this.serializerSettingsProvider = serializerSettingsProvider
                ?? throw new ArgumentNullException(nameof(serializerSettingsProvider));
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public Task PublishAsync(
            MarainCloudEvent marainCloudEvent,
            IEnumerable<WorkflowEventSubscription> directSubscriptions,
            CancellationToken cancellationToken)
        {
            return Task.WhenAll(directSubscriptions.Select(destination => this.PublishWithRetryAsync(marainCloudEvent, destination, cancellationToken)));
        }

        private async Task PublishWithRetryAsync(MarainCloudEvent marainCloudEvent, WorkflowEventSubscription destination, CancellationToken cancellationToken)
        {
            try
            {
                await Retriable.RetryAsync(() => this.PublishAsync(marainCloudEvent, destination), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Unexpected exception when trying to a CloudEvent with source '{source}', '{subject}' and destinationUrl '{destinationUrl}' with authentication resource '{msiAuthenticationResource}'.",
                    marainCloudEvent.Source,
                    marainCloudEvent.Subject,
                    destination?.ExternalUrl,
                    destination?.MsiAuthenticationResource);

                throw;
            }
        }

        private async Task PublishAsync(MarainCloudEvent marainCloudEvent, WorkflowEventSubscription destination)
        {
            this.logger.LogDebug(
                "Initialising event publish request for subject '{subject}' and source '{source}' to external URL '{externalUrl}'",
                marainCloudEvent.Subject,
                marainCloudEvent.Source,
                destination.ExternalUrl);

            var request = new HttpRequestMessage(HttpMethod.Post, destination.ExternalUrl);

            if (destination.AuthenticateWithManagedServiceIdentity)
            {
                string token = await this.serviceIdentityTokenSource.GetAccessToken(
                    destination.MsiAuthenticationResource).ConfigureAwait(false);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // TODO: Replace with a custom serializer for MarainCloudEvent.
            var cloudEvent = new
            {
                id = marainCloudEvent.Id,
                source = marainCloudEvent.Source,
                specversion = marainCloudEvent.SpecVersion,
                subject = marainCloudEvent.Subject,
                type = marainCloudEvent.Type,
                time = InstantPattern.ExtendedIso.Format(Instant.FromDateTimeOffset(marainCloudEvent.Time)),
                datacontenttype = marainCloudEvent.DataContentType,
                data = marainCloudEvent.Data,
                maraintenantid = marainCloudEvent.MarainTenantId,
            };

            request.Content = new StringContent(
                JsonConvert.SerializeObject(cloudEvent, this.serializerSettingsProvider.Instance),
                Encoding.UTF8,
                "application/cloudevents");

            HttpResponseMessage httpResponse = await this.httpClient.SendAsync(request).ConfigureAwait(false);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new CloudEventPublisherException(
                    marainCloudEvent.Source,
                    marainCloudEvent.Subject,
                    destination.ExternalUrl,
                    httpResponse.StatusCode,
                    httpResponse.ReasonPhrase);
            }
        }
    }
}
