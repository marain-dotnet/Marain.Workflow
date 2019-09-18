// <copyright file="DataCatalogWorkflowSteps.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Actions;
    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public class DataCatalogWorkflowSteps
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;

        public DataCatalogWorkflowSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

        [Given(@"I have an object of type ""(.*)"" called ""(.*)""")]
        public void GivenIHaveAnObjectOfTypeCalled(string contentType, string instanceName, Table table)
        {
            this.scenarioContext.Set(this.CreateObject(contentType, table), instanceName);
        }

        [Given(@"I have created and persisted the DataCatalogWorkflow with Id ""(.*)""")]
        public async Task GivenIHaveCreatedAndPersistedANewInstanceOfTheDataCatalogWorkflowWithId(string workflowId)
        {
            var workflow = DataCatalogWorkflowFactory.Create(featureContext, ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IWorkflowMessageQueue>(), workflowId);
            var tenantProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<ITenantProvider>();
            var engineFactory = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowEngineFactory>();
            var engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root);

            await engine.UpsertWorkflowAsync(workflow).ConfigureAwait(false);
        }

        [Then(@"a new data catalog item with Id ""(.*)"" should have been added to the data catalog store")]
        public async Task ThenANewDataCatalogItemWithIdShouldHaveBeenAddedToTheDataCatalogStore(string catalogItemId)
        {
            var repo = ContainerBindings.GetServiceProvider(this.featureContext).GetService<DataCatalogItemRepositoryFactory>().GetRepository();
            var item = await repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId)).ConfigureAwait(false);

            Assert.IsNotNull(item);
        }

        [Then(@"a new data catalog item with Id ""(.*)"" should not have been added to the data catalog store")]
        public async Task ThenANewDataCatalogItemWithIdShouldNotHaveBeenAddedToTheDataCatalogStore(string catalogItemId)
        {
            var repo = ContainerBindings.GetServiceProvider(this.featureContext).GetService<DataCatalogItemRepositoryFactory>().GetRepository();

            try
            {
                await repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId)).ConfigureAwait(false);
                Assert.Fail(
                    $"Did not expect a Catalog Item with Id {catalogItemId} to have been created, but one was found.");
            }
            catch (CosmosException ex)
            {
                Assert.AreEqual(
                    HttpStatusCode.NotFound,
                    ex.StatusCode,
                    $"Unexpected exception when trying to retrieve document with Id {catalogItemId}");
            }
        }

        [Then(@"the data catalog item with Id ""(.*)"" should have a Description of ""(.*)""")]
        public async Task ThenTheDataCatalogItemWithIdShouldHaveADescriptionOf(
            string catalogItemId,
            string expectedDescription)
        {
            var repo = ContainerBindings.GetServiceProvider(this.featureContext).GetService<DataCatalogItemRepositoryFactory>().GetRepository();
            var item = await repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId)).ConfigureAwait(false);

            Assert.AreEqual(expectedDescription, item.Resource.Description);
        }

        [Then(@"the data catalog item with Id ""(.*)"" should have an Identifier of ""(.*)""")]
        public async Task ThenTheDataCatalogItemWithIdShouldHaveAnIdentifierOf(
            string catalogItemId,
            string expectedIdentifier)
        {
            var repo = ContainerBindings.GetServiceProvider(this.featureContext).GetService<DataCatalogItemRepositoryFactory>().GetRepository();
            var item = await repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId)).ConfigureAwait(false);
            
            Assert.AreEqual(expectedIdentifier, item.Resource.Identifier);
        }

        [Then(@"the data catalog item with Id ""(.*)"" should have a Type of ""(.*)""")]
        public async Task ThenTheDataCatalogItemWithIdShouldHaveATypeOf(string catalogItemId, string expectedType)
        {
            var repo = ContainerBindings.GetServiceProvider(this.featureContext).GetService<DataCatalogItemRepositoryFactory>().GetRepository();
            var item = await repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId)).ConfigureAwait(false);

            Assert.AreEqual(expectedType, item.Resource.Type);
        }

        [Then(@"the data catalog item with Id ""(.*)"" should have Notes of ""(.*)""")]
        public async Task ThenTheDataCatalogItemWithIdShouldHaveNotesOf(string catalogItemId, string expectedNotes)
        {
            var repo = ContainerBindings.GetServiceProvider(this.featureContext).GetService<DataCatalogItemRepositoryFactory>().GetRepository();
            var item = await repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId)).ConfigureAwait(false);

            Assert.AreEqual(expectedNotes, item.Resource.Notes);
        }

        [Then(@"the following trace messages should be the last messages recorded")]
        public void ThenTheFollowingTraceMessagesShouldBeTheLastMessagesRecorded(Table table)
        {
            var log = this.scenarioContext[TraceAction.ScenarioContextListName] as List<string>;

            Assert.IsNotNull(log);
            Assert.GreaterOrEqual(log.Count, table.Rows.Count);

            var logTail = log.Skip(log.Count - table.Rows.Count).ToArray();
            logTail.ForEachAtIndex((s, i) => Assert.AreEqual(table.Rows[i]["Message"], s));
        }

        [Then(@"the following trace messages should have been recorded")]
        public void ThenTheFollowingTraceMessagesShouldHaveBeenRecorded(Table table)
        {
            var log = this.scenarioContext[TraceAction.ScenarioContextListName] as List<string>;

            Assert.IsNotNull(log);
            Assert.AreEqual(table.Rows.Count, log.Count);

            log.ForEachAtIndex((s, i) => Assert.AreEqual(table.Rows[i]["Message"], s));
        }

        [Then(@"the workflow instance with Id ""(.*)"" should be in the state called ""(.*)""")]
        public async Task ThenTheWorkflowInstanceWithIdShouldBeInTheStateCalled(string instanceId, string stateName)
        {
            var engineFactory = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowEngineFactory>();
            var tenantProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<ITenantProvider>();
            var engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root);

            var instance = await engine.GetWorkflowInstanceAsync(instanceId).ConfigureAwait(false);
            var workflow = await engine.GetWorkflowAsync(instance.WorkflowId).ConfigureAwait(false);
            var currentState = workflow.GetState(instance.StateId);

            Assert.AreEqual(stateName, currentState.DisplayName);
        }

        [Then(@"the workflow instance with Id ""(.*)"" should have status ""(.*)""")]
        public async Task ThenTheWorkflowInstanceWithIdShouldHaveStatus(string instanceId, string expectedStatus)
        {
            var engineFactory = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowEngineFactory>();
            var tenantProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<ITenantProvider>();
            var engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root);

            var instance = await engine.GetWorkflowInstanceAsync(instanceId).ConfigureAwait(false);

            Assert.AreEqual(expectedStatus, instance.Status.ToString());
        }

        [Given(@"I have sent the workflow engine a trigger of type ""(.*)""")]
        [When(@"I send the workflow engine a trigger of type ""(.*)""")]
        public async Task WhenISendTheWorkflowEngineATriggerOfType(string contentType, Table table)
        {
            var trigger = (IWorkflowTrigger)this.CreateObject(contentType, table);

            var queue = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowMessageQueue>();
            await queue.EnqueueTriggerAsync(trigger, default(Guid));
        }

        private object CreateObject(string contentType, Table table)
        {
            Assert.AreEqual(1, table.RowCount);

            var instance = table.CreateInstance(() => ContainerBindings.GetServiceProvider(this.featureContext).GetContent(contentType));

            return instance;
        }
    }
}

#pragma warning restore