// <copyright file="EventHubSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Functions.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.SpecFlow.Extensions;

    using Marain.Workflow.Functions.SpecFlow.Bindings;

    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.EventHubs.Processor;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    [Binding]
    public class EventHubSteps
    {
        private readonly ScenarioContext context;
        private readonly FeatureContext featureContext;

        public EventHubSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.context = scenarioContext;
            this.featureContext = featureContext;
        }

        [Given(@"I am listening for events from the event hub")]
        public async Task GivenIAmListeningForEventsFromTheEventHubWhoseConnectionStringIsStoredInTheKeyVaultSecretAt()
        {
            var configuration =  ContainerBindings.GetServiceProvider(this.featureContext).GetService<IConfigurationRoot>();

            var host = new EventProcessorHost(
                "endworkflow",
                "$Default",
                configuration["EventHubListenerConnectionString"],
                configuration["EventHubStorageAccountConnectionString"],
                "triggerleases");

            var processorOptions = new EventProcessorOptions { InitialOffsetProvider = s => EventPosition.FromEnd() };

            this.context.Set(host);

            await host.RegisterEventProcessorAsync<TestEventProcessor>(processorOptions);
        }

        [AfterScenario]
        public async Task Teardown(ScenarioContext scenarioContext)
        {
            await scenarioContext.RunAndStoreExceptionsAsync(
                async () =>
                {
                    if (this.context.TryGetValue<EventProcessorHost>(out var processor))
                    {
                        await processor.UnregisterEventProcessorAsync();
                    }
                });
        }

        [Then(@"I should have received a trigger containing JSON data that represents the object called ""(.*)""")]
        public void ThenIShouldHaveReceivedATriggerContainingJSONDataThatRepresentsTheObjectCalled(string instanceName)
        {
            this.ThenIShouldHaveReceivedAtLeastTriggersContainingJSONDataThatRepresentsTheObjectCalled(1, instanceName);
        }

        [Then(
            @"I should have received at least (.*) triggers containing JSON data that represents the object called ""(.*)""")]
        public void ThenIShouldHaveReceivedAtLeastTriggersContainingJSONDataThatRepresentsTheObjectCalled(
            int count,
            string instanceName)
        {
            var eventProcessors = this.context.Get<List<TestEventProcessor>>();
            var receivedEvents = eventProcessors.SelectMany(x => x.ReceivedEvents.Where(e => e.IsTrigger).Select(e => e.Trigger));

            var matchInstances = this.context.Get<IEnumerable<object>>(instanceName);

            var serializerSettingsProvider =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
            var jsonSerializerSettings = serializerSettingsProvider.Instance;

            var matchJson = matchInstances.Select(x => JsonConvert.SerializeObject(x, jsonSerializerSettings)).ToArray();

            var receivedEventsJson = receivedEvents.Select(x => JsonConvert.SerializeObject(x, jsonSerializerSettings)).ToArray();

            Console.WriteLine($"Received {receivedEventsJson.Length} events from Event Hub.");

            var matchingEvents = receivedEventsJson.Count(e => matchJson.Contains(e));

            Console.WriteLine($"Found {matchingEvents} events that match the triggers that were sent");

            Assert.GreaterOrEqual(matchingEvents, count);
        }

        [Then(@"I should have received a start new workflow instance message containing JSON data that represents the object called ""(.*)""")]
        public void ThenIShouldHaveReceivedAStartNewWorkflowInstanceMessageContainingJSONDataThatRepresentsTheObjectCalled(string instanceName)
        {
            ThenIShouldHaveReceivedAtLeastStartNewWorkflowInstanceMessageContainingJSONDataThatRepresentsTheObjectCalled(
                1,
                instanceName);
        }

        [Then(@"I should have received at least (.*) start new workflow instance messages containing JSON data that represents the object called ""(.*)""")]
        public void ThenIShouldHaveReceivedAtLeastStartNewWorkflowInstanceMessageContainingJSONDataThatRepresentsTheObjectCalled(int count, string instanceName)
        {
            var eventProcessors = this.context.Get<List<TestEventProcessor>>();
            var receivedEvents = eventProcessors.SelectMany(x => x.ReceivedEvents.Where(e => e.IsStartWorkflowRequest).Select(e => e.StartWorkflowInstanceRequest));

            var matchInstances = this.context.Get<IEnumerable<object>>(instanceName);

            var serializerSettingsProvider =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
            var jsonSerializerSettings = serializerSettingsProvider.Instance;

            var matchJson = matchInstances.Select(x => JsonConvert.SerializeObject(x, jsonSerializerSettings)).ToArray();

            var receivedEventsJson = receivedEvents.Select(x => JsonConvert.SerializeObject(x, jsonSerializerSettings)).ToArray();

            Console.WriteLine($"Received {receivedEventsJson.Length} events from Event Hub.");

            var matchingEvents = receivedEventsJson.Count(e => matchJson.Contains(e));

            Console.WriteLine($"Found {matchingEvents} events that match the start new workflow instance requests that were sent");

            Assert.GreaterOrEqual(matchingEvents, count);
        }


        [When(@"wait for up to (.*) seconds for incoming events from the event hub")]
        public Task WhenWaitForToUpSecondsForIncomingEventsFromTheEventHub(int p0)
        {
            return Task.Delay(p0 * 1000);
        }

        [Then(@"I should not have received an exception from processing events")]
        public void ThenIShouldNotHaveReceivedAnExceptionFromProcessingEvents()
        {
            var exceptionPresent = this.context.TryGetValue<Exception>(out var ex);

            Assert.IsFalse(exceptionPresent);
        }
    }
}
