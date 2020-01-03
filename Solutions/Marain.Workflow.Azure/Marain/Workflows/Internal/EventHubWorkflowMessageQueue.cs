// <copyright file="EventHubWorkflowMessageQueue.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Marain.Serialization.Json;
    using Marain.Telemetry;
    using Marain.Utilities.Azure;
    using Microsoft.ApplicationInsights;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    /// <summary>
    ///     Workflow trigger queue implementation that pushes triggers
    ///     to an event hub namespace.
    /// </summary>
    public class EventHubWorkflowMessageQueue : IWorkflowMessageQueue
    {
        private readonly Lazy<EventHubClient> client;

        private readonly JsonSerializerSettings serializationSettings;

        private readonly IConfigurationRoot configuration;
        private readonly IMetric enqueuedMetric;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventHubWorkflowMessageQueue" /> class.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <param name="telemetryClient">A <see cref="TelemetryClient"/>.</param>
        public EventHubWorkflowMessageQueue(IConfigurationRoot configuration, TelemetryClient telemetryClient)
        {
            this.configuration = configuration;
            this.client = new Lazy<EventHubClient>(this.BuildEventHubClient);
            this.serializationSettings = SerializerSettings.CreateSerializationSettings();
            this.enqueuedMetric = telemetryClient.CreateMetric("Endjin_WorkflowMessageEnqueued");
        }

        /// <inheritdoc />
        public Task EnqueueTriggerAsync(
            IWorkflowTrigger trigger,
            Guid operationId)
        {
            var envelope = new WorkflowMessageEnvelope { Trigger = trigger, OperationId = operationId };
            return this.EnqueueMessageEnvelopeAsync(envelope);
        }

        /// <inheritdoc />
        public Task EnqueueStartWorkflowInstanceRequestAsync(
            StartWorkflowInstanceRequest request,
            Guid operationId)
        {
            var envelope = new WorkflowMessageEnvelope { StartWorkflowInstanceRequest = request, OperationId = operationId };
            return this.EnqueueMessageEnvelopeAsync(envelope);
        }

        private async Task EnqueueMessageEnvelopeAsync(WorkflowMessageEnvelope envelope)
        {
            string json = JsonConvert.SerializeObject(envelope, this.serializationSettings);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            var data = new EventData(jsonBytes);
            data.Properties.Add("AppInsightsOperationId", TelemetryOperationContext.Current.OperationId);

            foreach (KeyValuePair<string, string> current in TelemetryOperationContext.Current.Properties)
            {
                data.Properties.Add(current.Key, current.Value);
            }

            await this.client.Value.SendAsync(data, envelope.PartitionKey).ConfigureAwait(false);
            this.enqueuedMetric.TrackValue(1);
        }

        private EventHubClient BuildEventHubClient()
        {
            string connectionString = SecretHelper.GetSecretFromConfigurationOrKeyVault(
                this.configuration,
                "kv:triggereventhubconnectionstring",
                this.configuration["KeyVaultName"],
                "triggereventhubconnectionstring");

            return EventHubClient.CreateFromConnectionString(connectionString);
        }
    }
}