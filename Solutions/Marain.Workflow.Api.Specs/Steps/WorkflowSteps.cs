// <copyright file="WorkflowSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Corvus.Leasing;
    using Corvus.Retry;
    using Corvus.Retry.Policies;
    using Corvus.Retry.Strategies;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

    [Binding]
    public class WorkflowSteps
    {
        private readonly ScenarioContext context;
        private readonly IServiceProvider serviceProvider;

        public WorkflowSteps(
            ScenarioContext scenarioContext,
            FeatureContext featureContext)
        {
            this.context = scenarioContext;
            this.serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
        }

        [Given("I have added the workflow '(.*)' to the workflow store with Id '(.*)'")]
        public async Task GivenIHaveAddedTheToTheWorkflowStoreWithId(string workflowName, string workflowId)
        {
            Workflow workflow = TestWorkflowFactory.Get(workflowName);
            workflow.Id = workflowId;

            IWorkflowEngineFactory engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            ITenantProvider tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            IWorkflowEngine engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root).ConfigureAwait(false);

            await engine.UpsertWorkflowAsync(workflow).ConfigureAwait(false);
        }

        [Given("I have started an instance of the workflow '(.*)' with instance id '(.*)' and using context object '(.*)'")]
        public async Task GivenIHaveStartedAnInstanceOfTheWorkflowWithInstanceIdAndUsingContextObject(string workflowId, string instanceId, string contextInstanceName)
        {
            IWorkflowEngineFactory engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            ITenantProvider tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            IWorkflowEngine engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root).ConfigureAwait(false);
            IDictionary<string, string> context = this.context.Get<IDictionary<string, string>>(contextInstanceName);

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
            IWorkflowEngineFactory engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            ITenantProvider tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            IWorkflowEngine engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root).ConfigureAwait(false);

            IEnumerable<string> instanceIds = await engine.GetMatchingWorkflowInstanceIdsForSubjectsAsync(new string[0], int.MaxValue, 0).ConfigureAwait(false);
            foreach (string current in instanceIds)
            {
                await engine.DeleteWorkflowInstanceAsync(current).ConfigureAwait(false);
            }
        }

        [Then("there should be (.*) workflow instance in the workflow instance store")]
        [Then("there should be (.*) workflow instances in the workflow instance store")]
        public async Task ThenThereShouldBeANewWorkflowInstanceInTheWorkflowInstanceStore(int expected)
        {
            IWorkflowEngineFactory engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            ITenantProvider tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            IWorkflowEngine engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root).ConfigureAwait(false);
            IEnumerable<string> instances = await engine.GetMatchingWorkflowInstanceIdsForSubjectsAsync(new string[0], int.MaxValue, 0).ConfigureAwait(false);

            Assert.AreEqual(expected, instances.Count());
        }

        [Then("there should be a workflow instance with the id '(.*)' in the workflow instance store")]
        public Task ThenThereShouldBeAWorkflowInstanceWithTheIdInTheWorkflowInstanceStore(string instanceId)
        {
            return this.GetWorkflowInstance(instanceId);
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
                new AnyException(),
                false).ConfigureAwait(false);
        }

        [Then("the workflow instance with id '(.*)' should have a context dictionary that matches '(.*)'")]
        public async Task ThenTheWorkflowInstanceWithIdShouldHaveAContextDictionaryThatMatches(string instanceId, string itemName)
        {
            WorkflowInstance instance = await this.GetWorkflowInstance(instanceId).ConfigureAwait(false);
            var expected = (IDictionary<string, string>)this.context[itemName];
            IDictionary<string, string> actual = instance.Context;

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
                new AnyException(),
                false).ConfigureAwait(false);
        }

        [Then("the workflow instance with id '(.*)' should be in the state with name '(.*)'")]
        public Task ThenTheWorkflowInstanceWithIdShouldBeInTheStateWithName(string instanceId, string expectedStateName)
        {
            return this.VerifyWorkflowInstanceState(instanceId, expectedStateName, true);
        }

        private async Task VerifyWorkflowInstanceState(string instanceId, string expectedStateName, bool useAssert = true)
        {
            WorkflowInstance instance = await this.GetWorkflowInstance(instanceId).ConfigureAwait(false);

            IWorkflowEngineFactory engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            ITenantProvider tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            IWorkflowEngine engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root).ConfigureAwait(false);

            Workflow workflow = await engine.GetWorkflowAsync(instance.WorkflowId).ConfigureAwait(false);
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

            IWorkflowEngineFactory engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            ITenantProvider tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            IWorkflowEngine engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root).ConfigureAwait(false);

            try
            {
                WorkflowInstance instance = await engine.GetWorkflowInstanceAsync(id).ConfigureAwait(false);
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
    }
}
