// <copyright file="WorkflowInstanceStoreSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Json;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public class WorkflowInstanceStoreSteps : Steps
    {
        [Given("I have stored the workflow instance called '(.*)'")]
        [When("I store the workflow instance called '(.*)'")]
        public async Task WhenIStoreTheWorkflowInstanceWithId(string instanceId)
        {
            ITenant tenant = this.ScenarioContext.Get<ITenant>();
            IWorkflowInstanceStore store = await this.GetWorkflowInstanceStore(tenant).ConfigureAwait(false);
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceId);

            try
            {
                await store.UpsertWorkflowInstanceAsync(instance).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.ScenarioContext.Set(ex);
            }
        }

        [Given("I apply the '(.*)' transition (.*) times to the workflow instance called '(.*)', saving on every iteration")]
        public async Task GivenIApplyTheTransitionTimesToTheWorkflowInstanceCalledSavingOnEveryIteration(string transitionName, int count, string instanceName)
        {
            ITenant tenant = this.ScenarioContext.Get<ITenant>();
            IWorkflowInstanceStore store = await this.GetWorkflowInstanceStore(tenant).ConfigureAwait(false);
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);
            Workflow workflow = this.ScenarioContext.Get<Workflow>(instance.WorkflowId);
            WorkflowTransition transition = workflow.GetState(instance.StateId).Transitions.First(x => x.Id == transitionName);
            WorkflowState targetState = workflow.GetState(transition.TargetStateId);

            var stopwatch = new Stopwatch();

            for (int i = 0; i < count; i++)
            {
                instance.SetTransitionStarted(workflow, transition, new EntityIdTrigger());
                instance.SetStateExited(WorkflowActionResult.Empty);
                instance.SetTransitionExecuted(WorkflowActionResult.Empty);
                instance.SetStateEntered(targetState, WorkflowActionResult.Empty);
                stopwatch.Start();
                await store.UpsertWorkflowInstanceAsync(instance).ConfigureAwait(false);
                stopwatch.Stop();
                Console.WriteLine($"Executed an update-save iteration in {stopwatch.ElapsedMilliseconds}ms");
                stopwatch.Reset();
            }
        }

        [Given("I apply the '(.*)' transition (.*) times to the workflow instance called '(.*)', saving at the end")]
        public async Task GivenIApplyTheTransitionTimesToTheWorkflowInstanceCalledSavingAtTheEnd(string transitionName, int count, string instanceName)
        {
            ITenant tenant = this.ScenarioContext.Get<ITenant>();
            IWorkflowInstanceStore store = await this.GetWorkflowInstanceStore(tenant).ConfigureAwait(false);
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);
            Workflow workflow = this.ScenarioContext.Get<Workflow>(instance.WorkflowId);
            WorkflowTransition transition = workflow.GetState(instance.StateId).Transitions.First(x => x.Id == transitionName);
            WorkflowState targetState = workflow.GetState(transition.TargetStateId);

            for (int i = 0; i < count; i++)
            {
                instance.SetTransitionStarted(workflow, transition, new EntityIdTrigger());
                instance.SetStateExited(WorkflowActionResult.Empty);
                instance.SetTransitionExecuted(WorkflowActionResult.Empty);
                instance.SetStateEntered(targetState, WorkflowActionResult.Empty);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await store.UpsertWorkflowInstanceAsync(instance).ConfigureAwait(false);
            stopwatch.Stop();
            Console.WriteLine($"Executed an update-save iteration in {stopwatch.ElapsedMilliseconds}ms");
        }

        [Given("I have loaded the workflow instance with Id '(.*)' and called it '(.*)'")]
        [When("I load the workflow instance with Id '(.*)' and call it '(.*)'")]
        public async Task WhenILoadTheWorkflowInstanceWithId(string instanceId, string instanceName)
        {
            ITenant tenant = this.ScenarioContext.Get<ITenant>();
            IWorkflowInstanceStore store = await this.GetWorkflowInstanceStore(tenant).ConfigureAwait(false);
            WorkflowInstance instance = await store.GetWorkflowInstanceAsync(instanceId).ConfigureAwait(false);
            this.ScenarioContext.Set(instance, instanceName);
        }

        private Task<IWorkflowInstanceStore> GetWorkflowInstanceStore(ITenant tenant)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.ScenarioContext);

            ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory = serviceProvider
                .GetRequiredService<ITenantedWorkflowInstanceStoreFactory>();

            return workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenant);
        }
    }
}
