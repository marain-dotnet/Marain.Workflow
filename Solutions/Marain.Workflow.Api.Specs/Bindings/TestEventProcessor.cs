// <copyright file="TestEventProcessor.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.Specs.Bindings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.SpecFlow.Extensions;
    using Marain.Workflows;

    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.EventHubs.Processor;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;

    using TechTalk.SpecFlow;

    /// <summary>
    /// Event Hub <see cref="IEventProcessor"/> that records the events it receives so that they can be verified as part
    /// of test assertions.
    /// </summary>
    public class TestEventProcessor : IEventProcessor
    {
        private static readonly object SyncRoot = new object();

        private readonly JsonSerializerSettings serializationSettings;
        private readonly ScenarioContext scenarioContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestEventProcessor"/> class.
        /// </summary>
        /// <param name="featureContext">The current <see cref="FeatureContext"/>.</param>
        /// <param name="scenarioContext">The current <see cref="ScenarioContext"/>.</param>
        public TestEventProcessor(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;

            IJsonSerializerSettingsProvider serializerSettingsProvider =
                ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();

            this.serializationSettings = serializerSettingsProvider.Instance;

            if (!scenarioContext.TryGetValue<List<TestEventProcessor>>(out List<TestEventProcessor> listProcessor))
            {
                lock (SyncRoot)
                {
                    if (!scenarioContext.TryGetValue(out listProcessor))
                    {
                        listProcessor = new List<TestEventProcessor>();
                        scenarioContext.Set(listProcessor);
                    }
                }
            }

            listProcessor.Add(this);
        }

        /// <summary>
        /// Gets the list of events received by this processor.
        /// </summary>
        public ConcurrentBag<WorkflowMessageEnvelope> ReceivedEvents { get; } = new ConcurrentBag<WorkflowMessageEnvelope>();

        /// <inheritdoc/>
        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"TestEventProcessor.OpenAsync called for partition {context.PartitionId}");
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"TestEventProcessor.CloseAsync called for partition {context.PartitionId}");
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Console.WriteLine($"TestEventProcessor.ProcessEventsAsync called for partition {context.PartitionId}");

            foreach (EventData current in messages)
            {
                string data = Encoding.UTF8.GetString(current.Body.Array);
                WorkflowMessageEnvelope envelope = JsonConvert.DeserializeObject<WorkflowMessageEnvelope>(data, this.serializationSettings);
                this.ReceivedEvents.Add(envelope);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"TestEventProcessor.ProcessErrorAsync called for partition {context.PartitionId} with exception {error}");

            this.scenarioContext.Set(error);

            return Task.CompletedTask;
        }
    }
}
