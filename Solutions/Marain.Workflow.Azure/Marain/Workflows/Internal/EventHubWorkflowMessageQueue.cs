// <copyright file="EventHubWorkflowMessageQueue.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Internal
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Extensions.Cosmos.Crypto;
    using Corvus.Extensions.Json;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    /// <summary>
    /// Workflow trigger queue implementation that pushes triggers to an event hub namespace.
    /// </summary>
    public class EventHubWorkflowMessageQueue : IWorkflowMessageQueue
    {
        private readonly Lazy<Task<EventHubClient>> clientProvider;

        private readonly IConfigurationRoot configuration;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubWorkflowMessageQueue" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serializerSettingsProvider">The serializer settings provider to use when serializing message envelopes.</param>
        public EventHubWorkflowMessageQueue(
            IConfigurationRoot configuration,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.configuration = configuration;
            this.clientProvider = new Lazy<Task<EventHubClient>>(this.BuildEventHubClientAsync);
            this.serializerSettingsProvider = serializerSettingsProvider;
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
            string json = JsonConvert.SerializeObject(envelope, this.serializerSettingsProvider.Instance);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            var data = new EventData(jsonBytes);
            EventHubClient client = await this.clientProvider.Value.ConfigureAwait(false);

            await client.SendAsync(data, envelope.PartitionKey).ConfigureAwait(false);
        }

        private async Task<EventHubClient> BuildEventHubClientAsync()
        {
            string connectionString = await SecretHelper.GetSecretFromConfigurationOrKeyVaultAsync(
                this.configuration,
                "kv:triggereventhubconnectionstring",
                this.configuration["KeyVaultName"],
                "triggereventhubconnectionstring").ConfigureAwait(false);

            return EventHubClient.CreateFromConnectionString(connectionString);
        }
    }
}