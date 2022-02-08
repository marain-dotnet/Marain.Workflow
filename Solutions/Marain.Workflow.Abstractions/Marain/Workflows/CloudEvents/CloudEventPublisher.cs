// <copyright file="CloudEventPublisher.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using Corvus.Extensions.Json;
    using Corvus.Identity.ClientAuthentication;
    using Corvus.Retry;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using NodaTime;
    using NodaTime.Text;

    /// <summary>
    /// Publishes cloud events to HTTP endpoints.
    /// </summary>
    public class CloudEventPublisher : ICloudEventDataPublisher
    {
        private readonly HttpClient httpClient;
        private readonly IServiceIdentityAccessTokenSource serviceIdentityTokenSource;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly ILogger<CloudEventPublisher> logger;
        private readonly string marainTenantId;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudEventPublisher"/> class.
        /// </summary>
        /// <param name="marainTenantId">The Marain Tenant Id to add to published events as an extension.</param>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use when POSTing event data to subscribers.</param>
        /// <param name="serviceIdentityTokenSource">The <see cref="IServiceIdentityAccessTokenSource"/> that will be used to aquire authentication tokens.</param>
        /// <param name="serializerSettingsProvider">The current <see cref="IJsonSerializerSettingsProvider"/>.</param>
        /// <param name="logger">The logger.</param>
        public CloudEventPublisher(
            string marainTenantId,
            HttpClient httpClient,
            IServiceIdentityAccessTokenSource serviceIdentityTokenSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ILogger<CloudEventPublisher> logger)
        {
            this.marainTenantId = marainTenantId;
            this.httpClient = httpClient;
            this.serviceIdentityTokenSource = serviceIdentityTokenSource;
            this.serializerSettingsProvider = serializerSettingsProvider;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task PublishWorkflowEventDataAsync<T>(
            string source,
            string eventType,
            string subject,
            string dataContentType,
            T eventData,
            IEnumerable<WorkflowEventSubscription> destinationUris)
        {
            var cloudEvent = new
            {
                id = Guid.NewGuid().ToString(),
                source,
                specversion = "1.0",
                subject,
                type = $"io.marain.{eventType}",
                time = InstantPattern.ExtendedIso.Format(SystemClock.Instance.GetCurrentInstant()),
                datacontenttype = dataContentType,
                data = eventData,
                maraintenantid = this.marainTenantId,
            };

            return Task.WhenAll(destinationUris.Select(destination => this.PublishWithRetryAsync(source, subject, cloudEvent, destination)));
        }

        private async Task PublishWithRetryAsync<T>(string source, string subject, T cloudEvent, WorkflowEventSubscription destination)
        {
            try
            {
                await Retriable.RetryAsync(() => this.PublishAsync(source, subject, cloudEvent, destination)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // In this "v1" solution to event publishing, we don't want exceptions to propagate outside the
                // publisher because we don't want failure in event publishing to break anything.
                this.logger.LogError(
                    ex,
                    "Unexpected exception when trying to a CloudEvent with source '{source}', '{subject}' and destinationUrl '{destinationUrl}' with authentication resource '{msiAuthenticationResource}'.",
                    source,
                    subject,
                    destination?.ExternalUrl,
                    destination?.MsiAuthenticationResource);
            }
        }

        private async Task PublishAsync<T>(string source, string subject, T cloudEvent, WorkflowEventSubscription destination)
        {
            this.logger.LogDebug(
                "Initialising event publish request for subject '{subject}' and source '{source}' to external URL '{externalUrl}'",
                subject,
                source,
                destination.ExternalUrl);

            var request = new HttpRequestMessage(HttpMethod.Post, destination.ExternalUrl);

            if (destination.AuthenticateWithManagedServiceIdentity)
            {
                AccessTokenDetail tokenDetails = await this.serviceIdentityTokenSource.GetAccessTokenAsync(
                    new AccessTokenRequest(new[] { $"{destination.MsiAuthenticationResource}/.default" }))
                    .ConfigureAwait(false);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenDetails.AccessToken);
            }

            request.Content = new StringContent(
                JsonConvert.SerializeObject(cloudEvent, this.serializerSettingsProvider.Instance),
                Encoding.UTF8,
                "application/cloudevents");

            HttpResponseMessage httpResponse = await this.httpClient.SendAsync(request).ConfigureAwait(false);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new CloudEventPublisherException(
                    subject,
                    source,
                    destination.ExternalUrl,
                    httpResponse.StatusCode,
                    httpResponse.ReasonPhrase);
            }
        }
    }
}