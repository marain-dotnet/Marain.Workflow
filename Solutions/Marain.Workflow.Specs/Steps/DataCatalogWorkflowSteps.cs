// <copyright file="DataCatalogWorkflowSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Actions;
    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

    [Binding]
    public class DataCatalogWorkflowSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly FeatureContext featureContext;

        public DataCatalogWorkflowSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
        }

        [Given("I have an object of type '(.*)' called '(.*)'")]
        public void GivenIHaveAnObjectOfTypeCalled(string contentType, string instanceName, Table table)
        {
            this.scenarioContext.Set(this.CreateObject(contentType, table), instanceName);
        }

        [Given("I have created and persisted the DataCatalogWorkflow with Id '(.*)'")]
        public async Task GivenIHaveCreatedAndPersistedANewInstanceOfTheDataCatalogWorkflowWithId(string workflowId)
        {
            IWorkflowMessageQueue workflowMessageQueue =
                ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowMessageQueue>();

            Workflow workflow = DataCatalogWorkflowFactory.Create(workflowId, workflowMessageQueue);

            ITenantedWorkflowStoreFactory storeFactory =
                ContainerBindings.GetServiceProvider(this.featureContext).GetService<ITenantedWorkflowStoreFactory>();

            ITenant tenant = this.featureContext.Get<ITenant>();

            IWorkflowStore store = await storeFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);

            await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(async () =>
            {
                try
                {
                    Workflow oldWorkflow = await store.GetWorkflowAsync(workflowId).ConfigureAwait(false);
                    workflow.ETag = oldWorkflow.ETag;
                }
                catch (WorkflowNotFoundException)
                {
                    // Don't care if there is no old workflow.
                }

                await store.UpsertWorkflowAsync(workflow).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        [Then("a new data catalog item with Id '(.*)' should have been added to the data catalog store")]
        public async Task ThenANewDataCatalogItemWithIdShouldHaveBeenAddedToTheDataCatalogStore(string catalogItemId)
        {
            Container repo =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<DataCatalogItemRepositoryFactory>()
                    .GetRepository();

            ItemResponse<CatalogItem> item = await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                () => repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId))).ConfigureAwait(false);

            Assert.IsNotNull(item);
        }

        [Then("a new data catalog item with Id '(.*)' should not have been added to the data catalog store")]
        public async Task ThenANewDataCatalogItemWithIdShouldNotHaveBeenAddedToTheDataCatalogStore(string catalogItemId)
        {
            Container repo =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<DataCatalogItemRepositoryFactory>()
                    .GetRepository();

            try
            {
                await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                    () => repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId))).ConfigureAwait(false);

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

        [Then("the data catalog item with Id '(.*)' should have a Description of '(.*)'")]
        public async Task ThenTheDataCatalogItemWithIdShouldHaveADescriptionOf(
            string catalogItemId,
            string expectedDescription)
        {
            Container repo =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<DataCatalogItemRepositoryFactory>()
                    .GetRepository();

            ItemResponse<CatalogItem> item =
                await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                    () => repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId))).ConfigureAwait(false);

            Assert.AreEqual(expectedDescription, item.Resource.Description);
        }

        [Then("the data catalog item with Id '(.*)' should have an Identifier of '(.*)'")]
        public async Task ThenTheDataCatalogItemWithIdShouldHaveAnIdentifierOf(
            string catalogItemId,
            string expectedIdentifier)
        {
            Container repo =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<DataCatalogItemRepositoryFactory>()
                    .GetRepository();

            ItemResponse<CatalogItem> item =
                await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                    () => repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId))).ConfigureAwait(false);

            Assert.AreEqual(expectedIdentifier, item.Resource.Identifier);
        }

        [Then("the data catalog item with Id '(.*)' should have a Type of '(.*)'")]
        public async Task ThenTheDataCatalogItemWithIdShouldHaveATypeOf(string catalogItemId, string expectedType)
        {
            Container repo =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<DataCatalogItemRepositoryFactory>()
                    .GetRepository();

            ItemResponse<CatalogItem> item =
                await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                    () => repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId))).ConfigureAwait(false);

            Assert.AreEqual(expectedType, item.Resource.Type);
        }

        [Then("the data catalog item with Id '(.*)' should have Notes of '(.*)'")]
        public async Task ThenTheDataCatalogItemWithIdShouldHaveNotesOf(string catalogItemId, string expectedNotes)
        {
            Container repo =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<DataCatalogItemRepositoryFactory>()
                        .GetRepository();

            ItemResponse<CatalogItem> item =
                await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                    () => repo.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId))).ConfigureAwait(false);

            Assert.AreEqual(expectedNotes, item.Resource.Notes);
        }

        [Then("the following trace messages should be the last messages recorded")]
        public void ThenTheFollowingTraceMessagesShouldBeTheLastMessagesRecorded(Table table)
        {
            var log = this.scenarioContext[TraceAction.ScenarioContextListName] as IList<string>;

            Assert.IsNotNull(log);
            Assert.GreaterOrEqual(log.Count, table.Rows.Count);

            string[] logTail = log.Skip(log.Count - table.Rows.Count).ToArray();
            logTail.ForEachAtIndex((s, i) => Assert.AreEqual(table.Rows[i]["Message"], s));
        }

        [Then("the following trace messages should have been recorded")]
        public void ThenTheFollowingTraceMessagesShouldHaveBeenRecorded(Table table)
        {
            var log = this.scenarioContext[TraceAction.ScenarioContextListName] as IList<string>;

            Assert.IsNotNull(log);
            Assert.AreEqual(table.Rows.Count, log.Count);

            log.ForEachAtIndex((s, i) => Assert.AreEqual(table.Rows[i]["Message"], s));
        }

        [Then("the workflow instance with Id '(.*)' should have (.*) change log entries")]
        public async Task ThenTheWorkflowInstanceWithIdShouldHaveChangeLogEntries(string instanceId, int count)
        {
            ITenantedWorkflowInstanceChangeLogFactory instanceChangeLogFactory =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<ITenantedWorkflowInstanceChangeLogFactory>();

            ITenant tenant = this.featureContext.Get<ITenant>();
            IWorkflowInstanceChangeLogReader instanceChangeLog = await instanceChangeLogFactory.GetWorkflowInstanceChangeLogReaderForTenantAsync(tenant).ConfigureAwait(false);

            int totalCount = 0;
            string continuationToken = null;

            // Let's keep going until we've run out of items to get.
            // We're asking for blocks of our expected result, so we will, in the usual case, get the result in a single block.
            // (Unless the API decides to return us a smaller batch for some internal reason, which is within its rights to do!)
            do
            {
                WorkflowInstanceLogPage log = await instanceChangeLog.GetLogEntriesAsync(instanceId, maxItems: count, continuationToken: continuationToken).ConfigureAwait(false);
                totalCount += log.Entries.Count;
                continuationToken = log.ContinuationToken;
            }
            while (continuationToken != null);

            Assert.AreEqual(count, totalCount);
        }

        [Then("the workflow instance with Id '(.*)' should be in the state called '(.*)'")]
        public async Task ThenTheWorkflowInstanceWithIdShouldBeInTheStateCalled(string instanceId, string stateName)
        {
            ITenantedWorkflowStoreFactory storeFactory =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<ITenantedWorkflowStoreFactory>();
            ITenantedWorkflowInstanceStoreFactory instanceStoreFactory =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<ITenantedWorkflowInstanceStoreFactory>();

            ITenant tenant = this.featureContext.Get<ITenant>();
            IWorkflowStore store = await storeFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);
            IWorkflowInstanceStore instanceStore = await instanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenant).ConfigureAwait(false);

            WorkflowInstance instance =
                await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                    () => instanceStore.GetWorkflowInstanceAsync(instanceId)).ConfigureAwait(false);

            Workflow workflow =
                await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                    () => store.GetWorkflowAsync(instance.WorkflowId)).ConfigureAwait(false);

            WorkflowState currentState = workflow.GetState(instance.StateId);

            Assert.AreEqual(stateName, currentState.DisplayName);
        }

        [Then("the workflow instance with Id '(.*)' should have status '(.*)'")]
        public async Task ThenTheWorkflowInstanceWithIdShouldHaveStatus(string instanceId, string expectedStatus)
        {
            ITenantedWorkflowInstanceStoreFactory instanceStoreFactory =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetService<ITenantedWorkflowInstanceStoreFactory>();

            ITenant tenant = this.featureContext.Get<ITenant>();

            IWorkflowInstanceStore instanceStore = await instanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenant).ConfigureAwait(false);

            WorkflowInstance instance =
                await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                    () => instanceStore.GetWorkflowInstanceAsync(instanceId)).ConfigureAwait(false);

            Assert.AreEqual(expectedStatus, instance.Status.ToString());
        }

        [Given("I have sent the workflow engine a trigger of type '(.*)'")]
        [When("I send the workflow engine a trigger of type '(.*)'")]
        public async Task WhenISendTheWorkflowEngineATriggerOfType(string contentType, Table table)
        {
            var trigger = (IWorkflowTrigger)this.CreateObject(contentType, table);

            IWorkflowMessageQueue queue =
                ContainerBindings.GetServiceProvider(this.featureContext)
                    .GetRequiredService<IWorkflowMessageQueue>();

            await queue.EnqueueTriggerAsync(trigger, default).ConfigureAwait(false);
        }

        private object CreateObject(string contentType, Table table)
        {
            Assert.AreEqual(1, table.RowCount);

            return table.CreateInstance(() => ContainerBindings.GetServiceProvider(this.featureContext).GetContent(contentType));
        }
    }
}
