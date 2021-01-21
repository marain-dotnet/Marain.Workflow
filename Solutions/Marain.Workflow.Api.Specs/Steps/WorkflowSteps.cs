// <copyright file="WorkflowSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Leasing;
    using Corvus.Retry;
    using Corvus.Retry.Policies;
    using Corvus.Retry.Strategies;
    using Corvus.Testing.SpecFlow;
    using Marain.TenantManagement.Testing;
    using Marain.Workflows.Api.Specs.Helpers;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public class WorkflowSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly IServiceProvider serviceProvider;
        private readonly TransientTenantManager transientTenantManager;
        private readonly FeatureContext featureContext;

        public WorkflowSteps(
            ScenarioContext scenarioContext,
            FeatureContext featureContext)
        {
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
            this.serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            this.transientTenantManager = TransientTenantManager.GetInstance(featureContext);
        }

        [Given("I have added the workflow '(.*)' to the workflow store with Id '(.*)'")]
        public Task GivenIHaveAddedTheToTheWorkflowStoreWithId(string workflowName, string workflowId)
        {
            return this.AddWorkflowToStore(workflowName, workflowId, new WorkflowEventSubscription[0]);
        }

        [Given("I have added the workflow '(.*)' to the workflow store with Id '(.*)' and event subscriptions")]
        public Task GivenIHaveAddedTheWorkflowToTheWorkflowStoreWithIdAndEventSubscriptions(string workflowName, string workflowId, Table table)
        {
            WorkflowEventSubscription[] subscriptions = table.CreateSet<WorkflowEventSubscription>().ToArray();
            return this.AddWorkflowToStore(workflowName, workflowId, subscriptions);
        }

        [Given("there is an event subscriber listening on port '(.*)' called '(.*)'")]
        public void GivenThereIsAnEventSubscriberListeningOnPortCalled(int port, string name)
        {
            var subscriber = new StubWorkflowEventSubscriber(port, HttpStatusCode.Accepted);
            subscriber.Start();
            this.scenarioContext.Set(subscriber, name);
        }

        [Given("there is an event subscriber that will return the status '(.*)' listening on port '(.*)' called '(.*)'")]
        public void GivenThereIsAnEventSubscriberListeningOnPortCalled(HttpStatusCode response, int port, string name)
        {
            var subscriber = new StubWorkflowEventSubscriber(port, response);
            subscriber.Start();
            this.scenarioContext.Set(subscriber, name);
        }

        [Then("a CloudEvent should have been published to the subscriber called '(.*)'")]
        [Then("CloudEvents should have been published to the subscriber called '(.*)'")]
        public void ThenACloudEventShouldHaveBeenPublishedToTheSubscriberCalled(string subscriberName, Table table)
        {
            StubWorkflowEventSubscriber subscriber = this.scenarioContext.Get<StubWorkflowEventSubscriber>(subscriberName);

            // Get the data from the requests as JObjects so we can check their values...
            JObject[] requestPayloads = subscriber.ReceivedRequests.Select(x => JObject.Parse(x.Content)).ToArray();

            foreach (TableRow row in table.Rows)
            {
                int index = int.Parse(row[0]);
                string path = row[1];
                string expectedValue = row[2];

                // We might need to substitute the tenant Id in...
                expectedValue = expectedValue.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);

                Assert.IsTrue(index < requestPayloads.Length, $"Expected an event at index {index} but was not present.");

                JToken targetToken = requestPayloads[index].SelectToken(path);
                Assert.IsNotNull(targetToken, $"Expected to find a data item at index '{index}' and path '{path}', but was not present.");

                Assert.AreEqual(expectedValue, targetToken.ToString(), $"Value did not match at index '{index}' and path '{path}'.");
            }
        }

        [Given("I have started an instance of the workflow '(.*)' with instance id '(.*)' and using context object '(.*)'")]
        public async Task GivenIHaveStartedAnInstanceOfTheWorkflowWithInstanceIdAndUsingContextObject(string workflowId, string instanceId, string contextInstanceName)
        {
            ITenantedWorkflowEngineFactory engineFactory = this.serviceProvider.GetRequiredService<ITenantedWorkflowEngineFactory>();
            IWorkflowEngine engine = await engineFactory.GetWorkflowEngineAsync(this.transientTenantManager.PrimaryTransientClient).ConfigureAwait(false);
            IDictionary<string, string> context = this.scenarioContext.Get<IDictionary<string, string>>(contextInstanceName);

            var request =
                new StartWorkflowInstanceRequest
                {
                    WorkflowId = workflowId,
                    WorkflowInstanceId = instanceId,
                    Context = context,
                };

            await engine.StartWorkflowInstanceAsync(request).ConfigureAwait(false);
        }

        [Given("The workflow instance store is empty")]
        public async Task TheWorkflowInstanceStoreIsEmpty()
        {
            ITenantedWorkflowInstanceStoreFactory storeFactory = this.serviceProvider.GetRequiredService<ITenantedWorkflowInstanceStoreFactory>();
            IWorkflowInstanceStore store = await storeFactory.GetWorkflowInstanceStoreForTenantAsync(this.transientTenantManager.PrimaryTransientClient).ConfigureAwait(false);

            IEnumerable<string> instanceIds = await store.GetMatchingWorkflowInstanceIdsForSubjectsAsync(new string[0], int.MaxValue, 0).ConfigureAwait(false);
            foreach (string current in instanceIds)
            {
                await store.DeleteWorkflowInstanceAsync(current).ConfigureAwait(false);
            }
        }

        [Given("I have an instance of the workflow '(.*)' with Id '(.*)'")]
        public void GivenIHaveAnInstanceOfTheWorkflowWithId(string workflowName, string workflowId)
        {
            Workflow workflow = TestWorkflowFactory.Get(workflowName);
            workflow.Id = workflowId;

            this.scenarioContext.Set(workflow, workflowName);
        }

        [Given("the workflow called '(.*)' has an etag value of '(.*)'")]
        public void GivenTheWorkflowCalledHasAnEtagValueOf(string workflowName, string etag)
        {
            Workflow workflow = this.scenarioContext.Get<Workflow>(workflowName);
            workflow.ETag = etag;
        }

        [Then("there should be (.*) workflow instance in the workflow instance store")]
        [Then("there should be (.*) workflow instances in the workflow instance store")]
        public async Task ThenThereShouldBeANewWorkflowInstanceInTheWorkflowInstanceStore(int expected)
        {
            ITenantedWorkflowInstanceStoreFactory storeFactory = this.serviceProvider.GetRequiredService<ITenantedWorkflowInstanceStoreFactory>();
            IWorkflowInstanceStore store = await storeFactory.GetWorkflowInstanceStoreForTenantAsync(this.transientTenantManager.PrimaryTransientClient).ConfigureAwait(false);
            IEnumerable<string> instances = await store.GetMatchingWorkflowInstanceIdsForSubjectsAsync(new string[0], int.MaxValue, 0).ConfigureAwait(false);

            Assert.AreEqual(expected, instances.Count());
        }

        [Then("there should be a workflow instance with the id '(.*)' in the workflow instance store")]
        public Task ThenThereShouldBeAWorkflowInstanceWithTheIdInTheWorkflowInstanceStore(string instanceId)
        {
            return this.GetWorkflowInstance(instanceId);
        }

        [Then("there should be a workflow with the id '(.*)' in the workflow store")]
        public async Task ThenThereShouldBeAWorkflowWithTheIdInTheWorkflowStore(string id)
        {
            ITenantedWorkflowStoreFactory storeFactory = this.serviceProvider.GetRequiredService<ITenantedWorkflowStoreFactory>();
            IWorkflowStore store = await storeFactory.GetWorkflowStoreForTenantAsync(this.transientTenantManager.PrimaryTransientClient).ConfigureAwait(false);

            try
            {
                _ = await store.GetWorkflowAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Couldn't find a workflow with id {id}: {ex}");
            }
        }

        [Then("the workflow instance with id '(.*)' should be an instance of the workflow with id '(.*)'")]
        public async Task ThenTheWorkflowInstanceWithIdShouldBeAnInstanceOfTheWorkflowWithId(string instanceId, string workflowId)
        {
            WorkflowInstance instance = await this.GetWorkflowInstance(instanceId).ConfigureAwait(false);

            Assert.AreEqual(workflowId, instance.WorkflowId);
        }

        [Then("there should be a workflow instance with the id '(.*)' in the workflow instance store within (.*) seconds")]
        public async Task ThenAfterSecondsAtMostThereShouldBeAWorkflowInstanceWithTheIdInTheWorkflowInstanceStore(string instanceId, int maximumWaitTime)
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(TimeSpan.FromSeconds(maximumWaitTime));
            await Retriable.RetryAsync(
                () => this.GetWorkflowInstance(instanceId, false),
                tokenSource.Token,
                new Linear(TimeSpan.FromSeconds(1), int.MaxValue),
                new AnyExceptionPolicy(),
                false).ConfigureAwait(false);
        }

        [Then("the workflow instance with id '(.*)' should have a context dictionary that matches '(.*)'")]
        public async Task ThenTheWorkflowInstanceWithIdShouldHaveAContextDictionaryThatMatches(string instanceId, string itemName)
        {
            WorkflowInstance instance = await this.GetWorkflowInstance(instanceId).ConfigureAwait(false);
            var expected = (IDictionary<string, string>)this.scenarioContext[itemName];
            IImmutableDictionary<string, string> actual = instance.Context;

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Then("the workflow instance with id '(.*)' should be in the state with name '(.*)' within (.*) seconds")]
        public async Task ThenAfterSecondsAtMostTheWorkflowInstanceWithIdShouldBeInTheStateWithName(string instanceId, string expectedStateName, int maxWaitTime)
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(TimeSpan.FromSeconds(maxWaitTime));
            await Retriable.RetryAsync(
                () => this.VerifyWorkflowInstanceState(instanceId, expectedStateName, false),
                tokenSource.Token,
                new Linear(TimeSpan.FromSeconds(1), int.MaxValue),
                new AnyExceptionPolicy(),
                false).ConfigureAwait(false);
        }

        [Then("the workflow instance with id '(.*)' should be in the state with name '(.*)'")]
        public Task ThenTheWorkflowInstanceWithIdShouldBeInTheStateWithName(string instanceId, string expectedStateName)
        {
            return this.VerifyWorkflowInstanceState(instanceId, expectedStateName, true);
        }

        [Then("the workflow instance with id '(.*)' should have the status '(.*)'")]
        public async Task ThenTheWorkflowInstanceWithIdShouldHaveTheStatus(string instanceId, WorkflowStatus expectedStatus)
        {
            WorkflowInstance instance = await this.GetWorkflowInstance(instanceId).ConfigureAwait(false);
            Assert.AreEqual(expectedStatus, instance.Status);
        }

        [Then("the response should contain the the workflow '(.*)'")]
        public void ThenTheResponseShouldContainTheTheWorkflow(string expectedWorkflowName)
        {
            Workflow expectedWorkflow = this.scenarioContext.Get<Workflow>(expectedWorkflowName);

            IJsonSerializerSettingsProvider serializationSettingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
            string actualWorkflowJson = this.scenarioContext.Get<string>("ResponseBody");
            Workflow actualWorkflow = JsonConvert.DeserializeObject<Workflow>(actualWorkflowJson, serializationSettingsProvider.Instance);

            Assert.AreEqual(expectedWorkflow.Id, actualWorkflow.Id);
        }

        [Then("the response should contain an ETag header")]
        public void ThenTheResponseShouldContainAnETagHeader()
        {
            HttpWebResponse response = this.scenarioContext.Get<HttpWebResponse>();
            string etagHeader = response.Headers.Get("ETag");
            Assert.IsNotNull(etagHeader);
        }

        private async Task VerifyWorkflowInstanceState(string instanceId, string expectedStateName, bool useAssert = true)
        {
            WorkflowInstance instance = await this.GetWorkflowInstance(instanceId).ConfigureAwait(false);

            ITenantedWorkflowStoreFactory storeFactory = this.serviceProvider.GetRequiredService<ITenantedWorkflowStoreFactory>();
            IWorkflowStore store = await storeFactory.GetWorkflowStoreForTenantAsync(this.transientTenantManager.PrimaryTransientClient).ConfigureAwait(false);

            Workflow workflow = await store.GetWorkflowAsync(instance.WorkflowId).ConfigureAwait(false);
            WorkflowState state = workflow.GetState(instance.StateId);

            if (useAssert)
            {
                Assert.AreEqual(expectedStateName, state.DisplayName);
            }
            else
            {
                if (expectedStateName != state.DisplayName)
                {
                    throw new Exception();
                }
            }
        }

        private async Task<WorkflowInstance> GetWorkflowInstance(string id, bool failTestOnException = true)
        {
            await this.EnsureWorkflowInstanceIsNotBeingModified(id).ConfigureAwait(false);

            ITenantedWorkflowInstanceStoreFactory storeFactory = this.serviceProvider.GetRequiredService<ITenantedWorkflowInstanceStoreFactory>();
            IWorkflowInstanceStore store = await storeFactory.GetWorkflowInstanceStoreForTenantAsync(this.transientTenantManager.PrimaryTransientClient).ConfigureAwait(false);

            try
            {
                WorkflowInstance instance = await store.GetWorkflowInstanceAsync(id).ConfigureAwait(false);
                return instance;
            }
            catch (Exception ex) when (failTestOnException)
            {
                Assert.Fail($"Couldn't find an instance with id {id}: {ex}");
            }

            return null;
        }

        private async Task EnsureWorkflowInstanceIsNotBeingModified(string instanceId)
        {
            // The instance can be persisted but potentially also be being modified. To make sure it's not, attempt to take
            // out a lease on the instance.
            ILeaseProvider leaseProvider = this.serviceProvider.GetRequiredService<ILeaseProvider>();
            await leaseProvider.ExecuteWithMutexAsync(
                _ => Console.WriteLine($"Acquired lease for instance {instanceId}"),
                instanceId,
                new Linear(TimeSpan.FromSeconds(1), 30)).ConfigureAwait(false);
        }

        private async Task AddWorkflowToStore(string workflowName, string workflowId, WorkflowEventSubscription[] subscriptions)
        {
            Workflow workflow = TestWorkflowFactory.Get(workflowName);
            workflow.WorkflowEventSubscriptions = subscriptions;
            workflow.Id = workflowId;

            ITenantedWorkflowStoreFactory storeFactory = this.serviceProvider.GetRequiredService<ITenantedWorkflowStoreFactory>();
            IWorkflowStore store = await storeFactory.GetWorkflowStoreForTenantAsync(
                this.transientTenantManager.PrimaryTransientClient).ConfigureAwait(false);

            try
            {
                await store.UpsertWorkflowAsync(workflow).ConfigureAwait(false);
            }
            catch (WorkflowConflictException)
            {
                // The workflow already exists. Move on.
            }

            // Get the workflow so we have the correct etag.
            workflow = await store.GetWorkflowAsync(workflow.Id).ConfigureAwait(false);
            this.scenarioContext.Set(workflow, workflowName);
        }
    }
}
