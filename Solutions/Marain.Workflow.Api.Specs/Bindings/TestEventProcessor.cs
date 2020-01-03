#pragma warning disable

namespace Marain.Workflow.Functions.SpecFlow.Bindings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Marain.Serialization.Json;
    using Marain.Workflows;

    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.EventHubs.Processor;

    using Newtonsoft.Json;

    using TechTalk.SpecFlow;

    public class TestEventProcessor : IEventProcessor
    {
        private static readonly object SyncRoot = new object();

        private readonly JsonSerializerSettings serializationSettings;

        public TestEventProcessor()
        {
            this.serializationSettings = SerializerSettings.CreateSerializationSettings();

            if (!ScenarioContext.Current.TryGetValue<List<TestEventProcessor>>(out var listProcessor))
            {
                lock (SyncRoot)
                {
                    if (!ScenarioContext.Current.TryGetValue(out listProcessor))
                    {
                        listProcessor = new List<TestEventProcessor>(); 
                        ScenarioContext.Current.Set(listProcessor);
                    }
                }
            }

            listProcessor.Add(this);
        }

        public ConcurrentBag<WorkflowMessageEnvelope> ReceivedEvents { get; } = new ConcurrentBag<WorkflowMessageEnvelope>();

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"TestEventProcessor.OpenAsync called for partition {context.PartitionId}");
            return Task.CompletedTask;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"TestEventProcessor.CloseAsync called for partition {context.PartitionId}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Console.WriteLine($"TestEventProcessor.ProcessEventsAsync called for partition {context.PartitionId}");

            foreach (var current in messages)
            {
                var data = Encoding.UTF8.GetString(current.Body.Array);
                var envelope = JsonConvert.DeserializeObject<WorkflowMessageEnvelope>(data, this.serializationSettings);
                this.ReceivedEvents.Add(envelope);
            }

            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"TestEventProcessor.ProcessErrorAsync called for partition {context.PartitionId} with exception {error}");

            ScenarioContext.Current.Set(error);

            return Task.CompletedTask;
        }
    }
}

#pragma warning restore
