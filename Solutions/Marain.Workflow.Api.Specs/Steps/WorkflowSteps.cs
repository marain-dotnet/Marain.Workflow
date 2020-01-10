namespace Marain.Workflows.Functions.Specs.Steps
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
            serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
        }

        [Given(@"I have added the workflow ""(.*)"" to the workflow store with Id ""(.*)""")]
        public async Task GivenIHaveAddedTheToTheWorkflowStoreWithId(string workflowName, string workflowId)
        {
            var workflow = TestWorkflowFactory.Get(workflowName);
            workflow.Id = workflowId;

            var engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            var tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            var engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root);

            await engine.UpsertWorkflowAsync(workflow);
        }

        [Given(@"I have started an instance of the workflow ""(.*)"" with instance id ""(.*)"" and using context object ""(.*)""")]
        public async Task GivenIHaveStartedAnInstanceOfTheWorkflowWithInstanceIdAndUsingContextObject(string workflowId, string instanceId, string contextInstanceName)
        {
            var engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            var tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            var engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root);
            var context = this.context.Get<IDictionary<string, string>>(contextInstanceName);

            var request =
                new StartWorkflowInstanceRequest
                {
                    WorkflowId = workflowId,
                    WorkflowInstanceId = instanceId,
                    Context = context
                };

            await engine.StartWorkflowInstanceAsync(request);
        }

        [Given(@"I have cleared down the workflow instance store")]
        public async Task GivenIHaveClearedDownTheWorkflowInstanceStore()
        {
            var engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            var tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            var engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root);

            var instanceIds = await engine.GetMatchingWorkflowInstanceIdsForSubjectsAsync(new string[0], int.MaxValue, null);
            foreach (var current in instanceIds)
            {
                await engine.DeleteWorkflowInstanceAsync(current).ConfigureAwait(false);
            }
        }

        [Then(@"there should be (.*) workflow instance in the workflow instance store")]
        [Then(@"there should be (.*) workflow instances in the workflow instance store")]
        public async Task ThenThereShouldBeANewWorkflowInstanceInTheWorkflowInstanceStore(int expected)
        {
            var engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            var tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            var engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root);
            var instances = await engine.GetMatchingWorkflowInstanceIdsForSubjectsAsync(new string[0], int.MaxValue, null);

            Assert.AreEqual(expected, instances.Count());
        }

        [Then(@"there should be a workflow instance with the id ""(.*)"" in the workflow instance store")]
        public Task ThenThereShouldBeAWorkflowInstanceWithTheIdInTheWorkflowInstanceStore(string instanceId)
        {
            return this.GetWorkflowInstance(instanceId);
        }

        [Then(@"the workflow instance with id ""(.*)"" should be an instance of the workflow with id ""(.*)""")]
        public async Task ThenTheWorkflowInstanceWithIdShouldBeAnInstanceOfTheWorkflowWithId(string instanceId, string workflowId)
        {
            var instance = await this.GetWorkflowInstance(instanceId);

            Assert.AreEqual(workflowId, instance.WorkflowId);
        }

        [Then(@"there should be a workflow instance with the id ""(.*)"" in the workflow instance store within (.*) seconds")]
        public async Task ThenAfterSecondsAtMostThereShouldBeAWorkflowInstanceWithTheIdInTheWorkflowInstanceStore(string instanceId, int maximumWaitTime)
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(TimeSpan.FromSeconds(maximumWaitTime));
            await Retriable.RetryAsync(
                () => this.GetWorkflowInstance(instanceId),
                tokenSource.Token,
                new Linear(TimeSpan.FromSeconds(1), int.MaxValue),
                new AnyException(),
                false).ConfigureAwait(false);
        }

        [Then(@"the workflow instance with id ""(.*)"" should have a context dictionary that matches ""(.*)""")]
        public async Task ThenTheWorkflowInstanceWithIdShouldHaveAContextDictionaryThatMatches(string instanceId, string itemName)
        {
            var instance = await this.GetWorkflowInstance(instanceId);
            var expected = (IDictionary<string, string>)this.context[itemName];
            var actual = instance.Context;

            CollectionAssert.AreEquivalent(expected, actual);
        }

        private async Task<WorkflowInstance> GetWorkflowInstance(string id)
        {
            await this.EnsureWorkflowInstanceIsNotBeingModified(id).ConfigureAwait(false);

            var engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            var tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            var engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root);

            try
            {
                var instance = await engine.GetWorkflowInstanceAsync(id).ConfigureAwait(false);
                return instance;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Couldn't find an instance with id {id}: {ex}");
            }

            return null;
        }

        [Then(@"the workflow instance with id ""(.*)"" should be in the state with name ""(.*)"" within (.*) seconds")]
        public async Task ThenAfterSecondsAtMostTheWorkflowInstanceWithIdShouldBeInTheStateWithName(string instanceId, string expectedStateName, int maxWaitTime)
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(TimeSpan.FromSeconds(maxWaitTime));
            await Retriable.RetryAsync(
                () => this.ThenTheWorkflowInstanceWithIdShouldBeInTheStateWithName(instanceId, expectedStateName),
                tokenSource.Token,
                new Linear(TimeSpan.FromSeconds(1), int.MaxValue),
                new AnyException(),
                false).ConfigureAwait(false);
        }

        [Then(@"the workflow instance with id ""(.*)"" should be in the state with name ""(.*)""")]
        public async Task ThenTheWorkflowInstanceWithIdShouldBeInTheStateWithName(string instanceId, string expectedStateName)
        {
            var instance = await this.GetWorkflowInstance(instanceId).ConfigureAwait(false);

            var engineFactory = this.serviceProvider.GetRequiredService<IWorkflowEngineFactory>();
            var tenantProvider = this.serviceProvider.GetRequiredService<ITenantProvider>();
            var engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root);

            var workflow = await engine.GetWorkflowAsync(instance.WorkflowId);
            var state = workflow.GetState(instance.StateId);

            Assert.AreEqual(expectedStateName, state.DisplayName);
        }

        private async Task EnsureWorkflowInstanceIsNotBeingModified(string instanceId)
        {
            // The instance can be persisted but potentially also be being modified. To make sure it's not, attempt to take
            // out a lease on the instance.
            var leaseProvider = this.serviceProvider.GetRequiredService<ILeaseProvider>();
            await leaseProvider.ExecuteWithMutexAsync(
                ct => Console.WriteLine($"Acquired lease for instance {instanceId}"),
                instanceId,
                new Linear(TimeSpan.FromSeconds(1), 30));
        }
    }
}
