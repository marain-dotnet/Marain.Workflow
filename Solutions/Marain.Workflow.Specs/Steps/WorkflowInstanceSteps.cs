// <copyright file="WorkflowInstanceSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Corvus.Testing.SpecFlow;
    using Marain.Workflows.Specs.TestObjects;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public class WorkflowInstanceSteps : Steps
    {
        [Given("I have a simple two-state workflow definition called '(.*)'")]
        public void GivenIHaveASimpleTwo_StateWorkflowDefinitionWithId(string workflowId)
        {
            Workflow workflow = SimpleWorkflowFactory.CreateTwoStateWorkflow(workflowId);
            this.ScenarioContext.Set(workflow, workflowId);
        }

        [Given("I have created a new workflow instance called '(.*)'")]
        [When("I create a new workflow instance and call it '(.*)'")]
        public void WhenICreateANewWorkflowInstance(string name, Table parameters)
        {
            string instanceId = parameters.Rows[0]["InstanceId"];
            string workflowId = parameters.Rows[0]["WorkflowId"];
            string contextName = parameters.Rows[0]["Context"].Trim('{', '}');
            Dictionary<string, string> context = string.IsNullOrEmpty(contextName)
                ? new Dictionary<string, string>()
                : this.ScenarioContext.Get<Dictionary<string, string>>(contextName);

            Workflow workflow = this.ScenarioContext.Get<Workflow>(workflowId);

            var instance = new WorkflowInstance(instanceId, workflow, context);

            this.ScenarioContext.Set(instance, name);
        }

        [Given("I have a data catalog workflow definition with Id '(.*)'")]
        public void GivenIHaveADataCatalogWorkflowDefinitionWithId(string workflowId)
        {
            Workflow workflow = DataCatalogWorkflowFactory.Create(workflowId, null);
            this.ScenarioContext.Set(workflow, workflowId);
        }

        [Given("I have a context dictionary called '(.*)':")]
        public void GivenIHaveAContextDictionaryCalled(string name, Table table)
        {
            IEnumerable<KeyValuePair<string, string>> data = table.CreateSet<KeyValuePair<string, string>>();
            var context = data.ToDictionary(x => x.Key, x => x.Value);
            this.ScenarioContext.Set(context, name);
        }

        [Given("I have persisted the workflow instance called '(.*)' to storage")]
        public void GivenIHavePersistedTheWorkflowInstanceWithIdToStorage(string instanceId)
        {
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceId);
            instance.ClearUncommittedEvents();
        }

        [Given("I have started the transition '(.*)' for the workflow instance called '(.*)' with a trigger of type '(.*)'")]
        [When("I start the transition '(.*)' for the workflow instance called '(.*)' with a trigger of type '(.*)'")]
        public void WhenIStartTheTransitionForTheWorkflowInstanceWithIdATriggerOfType(string transitionId, string instanceId, string triggerContentType, Table triggerProperties)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.ScenarioContext);
            IWorkflowTrigger trigger = serviceProvider.GetContent<IWorkflowTrigger>(triggerContentType);
            triggerProperties.FillInstance(trigger);

            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceId);
            Workflow workflow = this.ScenarioContext.Get<Workflow>(instance.WorkflowId);
            WorkflowTransition transition = workflow.States.SelectMany(x => x.Value.Transitions).First(x => x.Id == transitionId);

            try
            {
                instance.SetTransitionStarted(workflow, transition, trigger);
            }
            catch (Exception ex)
            {
                this.ScenarioContext.Set(ex);
            }
        }

        [Given("I have set the workflow instance called '(.*)' as having entered the state '(.*)' with the following context updates:")]
        [When("I set the workflow instance called '(.*)' as having entered the state '(.*)' with the following context updates:")]
        public void WhenISetTheWorkflowInstanceIdAsHavingEnteredTheStateWithTheFollowingContextUpdates(string instanceId, string stateId, Table contextUpdatesTable)
        {
            WorkflowActionResult actionResult = this.BuildWorkflowActionResultFrom(contextUpdatesTable);
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceId);
            Workflow workflow = this.ScenarioContext.Get<Workflow>(instance.WorkflowId);
            WorkflowState enteredState = workflow.GetState(stateId);

            try
            {
                instance.SetStateEntered(enteredState, actionResult);
            }
            catch (Exception ex)
            {
                this.ScenarioContext.Set(ex);
            }
        }

        [Given("I have set the workflow instance called '(.*)' as having exited the current state with the following context updates:")]
        [When("I set the workflow instance called '(.*)' as having exited the current state with the following context updates:")]
        public void WhenISetTheWorkflowInstanceIdAsHavingExitedTheStateWithTheFollowingContextUpdates(string instanceName, Table contextUpdatesTable)
        {
            WorkflowActionResult actionResult = this.BuildWorkflowActionResultFrom(contextUpdatesTable);
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);

            try
            {
                instance.SetStateExited(actionResult);
            }
            catch (Exception ex)
            {
                this.ScenarioContext.Set(ex);
            }
        }

        [Given("I have set the workflow instance called '(.*)' as having exited the current state")]
        public void GivenIHaveSetTheWorkflowInstanceIdAsHavingExitedTheCurrentState(string instanceName)
        {
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);

            try
            {
                instance.SetStateExited(WorkflowActionResult.Empty);
            }
            catch (Exception ex)
            {
                this.ScenarioContext.Set(ex);
            }
        }

        [Given("I have set the workflow instance called '(.*)' as having executed transition actions with the following context updates:")]
        [When("I set the workflow instance called '(.*)' as having executed transition actions with the following context updates:")]
        public void WhenISetTheWorkflowInstanceIdAsHavingExecutedTransitionActionsWithTheFollowingContextUpdates(string instanceName, Table contextUpdatesTable)
        {
            WorkflowActionResult actionResult = this.BuildWorkflowActionResultFrom(contextUpdatesTable);
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);

            try
            {
                instance.SetTransitionExecuted(actionResult);
            }
            catch (Exception ex)
            {
                this.ScenarioContext.Set(ex);
            }
        }

        [When("I set the workflow instance called '(.*)' as having executed transition actions")]
        public void WhenISetTheWorkflowInstanceIdAsHavingExecutedTransitionActions(string instanceId)
        {
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceId);

            try
            {
                instance.SetTransitionExecuted(WorkflowActionResult.Empty);
            }
            catch (Exception ex)
            {
                this.ScenarioContext.Set(ex);
            }
        }

        [Given("I have set the workflow instance called '(.*)' as having entered the state '(.*)'")]
        [When("I set the workflow instance called '(.*)' as having entered the state '(.*)'")]
        public void WhenISetTheWorkflowInstanceIdAsHavingEnteredTheState(string instanceName, string stateId)
        {
            this.WhenISetTheWorkflowInstanceIdAsHavingEnteredTheStateWithTheFollowingContextUpdates(instanceName, stateId, new Table("Operation", "Key", "Value"));
        }

        [When("I set the workflow instance called '(.*)' as faulted with the message '(.*)' and data")]
        public void WhenISetTheWorkflowInstanceAsFaultedWithTheMessageAndData(string instanceName, string message, Table table)
        {
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);
            var data = table.Rows.ToDictionary(x => x["Key"], x => x["Value"]);

            instance.SetFaulted(message, data);
        }

        [Given("I have set the workflow instance called '(.*)' as faulted with the message '(.*)'")]
        [When("I set the workflow instance called '(.*)' as faulted with the message '(.*)'")]
        public void WhenISetTheWorkflowInstanceWithIdAsFaultedWithTheMessage(string instanceName, string message)
        {
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);

            instance.SetFaulted(message, new Dictionary<string, string>());
        }

        [Then("the workflow instance called '(.*)' should have (.*) uncommitted events")]
        [Then("the workflow instance called '(.*)' should have (.*) uncommitted event")]
        public void ThenTheWorkflowInstanceWithIdShouldHaveUncommittedEvent(string instanceName, int expectedUncommittedEventCount)
        {
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);

            Assert.AreEqual(expectedUncommittedEventCount, instance.GetUncommittedEvents().Count);
        }

        [Then("the workflow instance called '(.*)' should have the following properties:")]
        public void ThenTheWorkflowInstanceCalledShouldHaveTheFollowingProperties(string instanceName, Table table)
        {
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);

            foreach (TableRow current in table.Rows)
            {
                string propertyName = current["Property"];
                string expectedValue = current["Value"];

                switch (propertyName)
                {
                    case "Id":
                        Assert.AreEqual(expectedValue, instance.Id);
                        break;

                    case "Status":
                        WorkflowStatus expectedStatus = Enum.Parse<WorkflowStatus>(expectedValue);
                        Assert.AreEqual(expectedStatus, instance.Status);
                        break;

                    case "StateId":
                        if (string.IsNullOrEmpty(expectedValue))
                        {
                            Assert.IsNull(instance.StateId);
                        }
                        else
                        {
                            Assert.AreEqual(expectedValue, instance.StateId);
                        }

                        break;

                    case "WorkflowId":
                        Assert.AreEqual(expectedValue, instance.WorkflowId);
                        break;

                    case "Context":
                        Dictionary<string, string> expectedContext = this.ScenarioContext.Get<Dictionary<string, string>>(expectedValue.Trim('{', '}'));
                        CollectionAssert.AreEquivalent(expectedContext, instance.Context);
                        break;

                    case "IsDirty":
                        bool expectedIsDirty = bool.Parse(expectedValue);
                        Assert.AreEqual(expectedIsDirty, instance.IsDirty);
                        break;
                }
            }
        }

        [Then("the workflow instance called '(.*)' should have the following context:")]
        public void ThenTheWorkflowInstanceWithIdShouldHaveTheFollowingContext(string instanceName, Table table)
        {
            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceName);

            IEnumerable<KeyValuePair<string, string>> data = table.CreateSet<KeyValuePair<string, string>>();
            var expectedContext = data.ToDictionary(x => x.Key, x => x.Value);

            CollectionAssert.AreEquivalent(expectedContext, instance.Context);
        }

        private WorkflowActionResult BuildWorkflowActionResultFrom(Table table)
        {
            var itemsToAddOrUpdate = table.Rows
                .Where(x => x["Operation"] == "AddOrUpdate")
                .ToDictionary(x => x["Key"], x => x["Value"]);

            IEnumerable<string> itemsToRemove = table.Rows
                .Where(x => x["Operation"] == "Remove")
                .Select(x => x["Key"]);

            return new WorkflowActionResult(itemsToAddOrUpdate, itemsToRemove);
        }
    }
}
