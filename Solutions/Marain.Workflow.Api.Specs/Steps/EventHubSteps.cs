// <copyright file="EventHubSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

    [Binding]
    public class EventHubSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly FeatureContext featureContext;

        public EventHubSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
        }

        [Given("I am listening for events from the event hub")]
        public async Task GivenIAmListeningForEventsFromTheEventHubWhoseConnectionStringIsStoredInTheKeyVaultSecretAt()
        {
            IConfigurationRoot configuration = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IConfigurationRoot>();

            var host = new EventProcessorHost(
                "endworkflow",
                "$Default",
                configuration["EventHubListenerConnectionString"],
                configuration["EventHubStorageAccountConnectionString"],
                "triggerleases");

            var processorOptions = new EventProcessorOptions
            {
                InitialOffsetProvider = _ => EventPosition.FromEnd(),
            };

            this.scenarioContext.Set(host);

            var eventProcessorFactory = new TestEventProcessorFactory(this.featureContext, this.scenarioContext);

            await host.RegisterEventProcessorFactoryAsync(eventProcessorFactory, processorOptions).ConfigureAwait(false);
        }

        [AfterScenario]
        public async Task Teardown(ScenarioContext scenarioContext)
        {
            await scenarioContext.RunAndStoreExceptionsAsync(
                async () =>
                {
                    if (this.scenarioContext.TryGetValue<EventProcessorHost>(out EventProcessorHost processor))
                    {
                        await processor.UnregisterEventProcessorAsync().ConfigureAwait(false);
                    }
                }).ConfigureAwait(false);
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
            List<TestEventProcessor> eventProcessors = this.scenarioContext.Get<List<TestEventProcessor>>();
            IEnumerable<IWorkflowTrigger> receivedEvents = eventProcessors.SelectMany(x => x.ReceivedEvents.Where(e => e.IsTrigger).Select(e => e.Trigger));

            IEnumerable<object> matchInstances = this.scenarioContext.Get<IEnumerable<object>>(instanceName);

            IJsonSerializerSettingsProvider serializerSettingsProvider =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
            JsonSerializerSettings jsonSerializerSettings = serializerSettingsProvider.Instance;

            string[] matchJson = matchInstances.Select(x => JsonConvert.SerializeObject(x, jsonSerializerSettings)).ToArray();

            string[] receivedEventsJson = receivedEvents.Select(x => JsonConvert.SerializeObject(x, jsonSerializerSettings)).ToArray();

            Console.WriteLine($"Received {receivedEventsJson.Length} events from Event Hub.");

            int matchingEvents = receivedEventsJson.Count(e => matchJson.Contains(e));

            Console.WriteLine($"Found {matchingEvents} events that match the triggers that were sent");

            Assert.GreaterOrEqual(matchingEvents, count);
        }

        [Then(@"I should have received a start new workflow instance message containing JSON data that represents the object called ""(.*)""")]
        public void ThenIShouldHaveReceivedAStartNewWorkflowInstanceMessageContainingJSONDataThatRepresentsTheObjectCalled(string instanceName)
        {
            this.ThenIShouldHaveReceivedAtLeastStartNewWorkflowInstanceMessageContainingJSONDataThatRepresentsTheObjectCalled(
                1,
                instanceName);
        }

        [Then(@"I should have received at least (.*) start new workflow instance messages containing JSON data that represents the object called ""(.*)""")]
        public void ThenIShouldHaveReceivedAtLeastStartNewWorkflowInstanceMessageContainingJSONDataThatRepresentsTheObjectCalled(int count, string instanceName)
        {
            List<TestEventProcessor> eventProcessors = this.scenarioContext.Get<List<TestEventProcessor>>();
            IEnumerable<StartWorkflowInstanceRequest> receivedEvents = eventProcessors.SelectMany(x => x.ReceivedEvents.Where(e => e.IsStartWorkflowRequest).Select(e => e.StartWorkflowInstanceRequest));

            IEnumerable<object> matchInstances = this.scenarioContext.Get<IEnumerable<object>>(instanceName);

            IJsonSerializerSettingsProvider serializerSettingsProvider =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
            JsonSerializerSettings jsonSerializerSettings = serializerSettingsProvider.Instance;

            string[] matchJson = matchInstances.Select(x => JsonConvert.SerializeObject(x, jsonSerializerSettings)).ToArray();

            string[] receivedEventsJson = receivedEvents.Select(x => JsonConvert.SerializeObject(x, jsonSerializerSettings)).ToArray();

            Console.WriteLine($"Received {receivedEventsJson.Length} events from Event Hub.");

            int matchingEvents = receivedEventsJson.Count(e => matchJson.Contains(e));

            Console.WriteLine($"Found {matchingEvents} events that match the start new workflow instance requests that were sent");

            Assert.GreaterOrEqual(matchingEvents, count);
        }

        [When("wait for up to (.*) seconds for incoming events from the event hub")]
        public Task WhenWaitForToUpSecondsForIncomingEventsFromTheEventHub(int p0)
        {
            return Task.Delay(p0 * 1000);
        }

        [Then("I should not have received an exception from processing events")]
        public void ThenIShouldNotHaveReceivedAnExceptionFromProcessingEvents()
        {
            bool exceptionPresent = this.scenarioContext.TryGetValue<Exception>(out _);

            Assert.IsFalse(exceptionPresent);
        }
    }
}
