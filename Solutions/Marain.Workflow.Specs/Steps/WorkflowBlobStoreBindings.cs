// <copyright file="WorkflowBlobStoreBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Extensions.Json;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Workflows.Specs.TestObjects;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class WorkflowBlobStoreBindings
    {
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;

        public WorkflowBlobStoreBindings(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

        [Given("I have a workflow definition with Id '(.*)' called '(.*)'")]
        public void GivenIHaveAWorkflowDefinitionWithIdCalled(string workflowId, string workflowName)
        {
            Workflow workflow = DataCatalogWorkflowFactory.Create(workflowId, null);
            this.scenarioContext.Set(workflow, workflowName);
        }

        [Given("I have stored the workflow called '(.*)' in the Azure storage workflow store")]
        [When("I store the workflow called '(.*)' in the Azure storage workflow store")]
        public async Task WhenIStoreTheWorkflowCalledInTheAzureStorageWorkflowStore(string workflowName)
        {
            IWorkflowStore workflowStore = await this.GetWorkflowStoreForCurrentTenantAsync().ConfigureAwait(false);

            Workflow workflow = this.scenarioContext.Get<Workflow>(workflowName);

            try
            {
                await workflowStore.UpsertWorkflowAsync(workflow).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex);
            }
        }

        [Then("a new blob with Id '(.*)' is created in the container for the current tenant")]
        public async Task ThenANewBlobWithIdIsCreatedInTheContainerForTheCurrentTenant(string blobId)
        {
            CloudBlockBlob blob = await this.GetCloudBlockBlobForCurrentTenantAsync(blobId).ConfigureAwait(false);
            bool exists = await blob.ExistsAsync().ConfigureAwait(false);

            Assert.IsTrue(exists);

            this.scenarioContext.Set(blob);
        }

        [Then("the blob with Id '(.*)' contains the workflow called '(.*)' serialized and encoded using UTF8")]
        public async Task ThenTheBlobWithIdContainsTheWorkflowCalledSerializedAndEncodedUsingUTF(string blobId, string workflowName)
        {
            IJsonSerializerSettingsProvider serializerSettingsProvider = ContainerBindings.GetServiceProvider(this.featureContext)
                .GetRequiredService<IJsonSerializerSettingsProvider>();

            CloudBlockBlob blob = await this.GetCloudBlockBlobForCurrentTenantAsync(blobId).ConfigureAwait(false);
            string blobContents = await blob.DownloadTextAsync(Encoding.UTF8, null, null, null).ConfigureAwait(false);
            Workflow actualWorkflow = JsonConvert.DeserializeObject<Workflow>(blobContents, serializerSettingsProvider.Instance);

            Workflow expectedWorkflow = this.scenarioContext.Get<Workflow>(workflowName);

            // We don't need to check that every property has deserialized correctly - we're not testing JSON.NET.
            // It's enough to know that we've successfully deserialized the JSON from the blob and that it's the
            // expected workflow.
            Assert.AreEqual(expectedWorkflow.Id, actualWorkflow.Id);
        }

        [Then("the workflow called '(.*)' has its etag updated to match the etag of the blob with Id '(.*)'")]
        public async Task WhenTheWorkflowCalledHasItsEtagUpdatedToMatchTheEtagOfTheBlobWithId(string workflowName, string blobId)
        {
            Workflow workflow = this.scenarioContext.Get<Workflow>(workflowName);
            CloudBlockBlob blob = await this.GetCloudBlockBlobForCurrentTenantAsync(blobId).ConfigureAwait(false);
            await blob.FetchAttributesAsync().ConfigureAwait(false);

            Assert.AreEqual(blob.Properties.ETag, workflow.ETag);
        }

        [Then("the request is successful")]
        public void ThenTheRequestIsSuccessful()
        {
            Assert.IsFalse(
                this.scenarioContext.TryGetValue<Exception>(out Exception thrownException),
                $"An unexpected exception was thrown whilst making the request:\n{thrownException}");
        }

        [Then("an '(.*)' is thrown")]
        [Then("a '(.*)' is thrown")]
        public void ThenAIsThrown(string exceptionTypeName)
        {
            Assert.IsTrue(this.scenarioContext.TryGetValue<Exception>(out Exception thrownException), "No exception was thrown");
            Assert.AreEqual(
                exceptionTypeName,
                thrownException.GetType().Name,
                $"An unexpected exception was thrown whilst making the request:\n{thrownException}");
        }

        [When("I request the workflow with Id '(.*)' from the Azure storage workflow store and call it '(.*)'")]
        public async Task WhenIRequestTheWorkflowWithIdFromTheAzureStorageWorkflowStoreAndCallIt(string workflowId, string workflowName)
        {
            IWorkflowStore workflowStore = await this.GetWorkflowStoreForCurrentTenantAsync().ConfigureAwait(false);

            try
            {
                Workflow actualWorkflow = await workflowStore.GetWorkflowAsync(workflowId).ConfigureAwait(false);
                this.scenarioContext.Set(actualWorkflow, workflowName);
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex);
            }
        }

        [Then("the workflow called '(.*)' has an etag matching the workflow called '(.*)'")]
        public void ThenTheWorkflowCalledHasAnEtagMatchingTheWorkflowCalled(string workflow1Name, string workflow2Name)
        {
            Workflow workflow1 = this.scenarioContext.Get<Workflow>(workflow1Name);
            Workflow workflow2 = this.scenarioContext.Get<Workflow>(workflow2Name);

            Assert.AreEqual(workflow2.ETag, workflow1.ETag);
        }

        [Given("I change the description of the workflow definition called '(.*)' to '(.*)'")]
        public void GivenIChangeTheDescriptionOfTheWorkflowDefinitionCalledTo(string workflowName, string newDescription)
        {
            Workflow workflow = this.scenarioContext.Get<Workflow>(workflowName);
            workflow.Description = newDescription;
        }

        [Given("I set the etag of the workflow definition called '(.*)' to '(.*)'")]
        public void GivenISetTheEtagOfTheWorkflowDefinitionCalledTo(string workflowName, string newEtag)
        {
            Workflow workflow = this.scenarioContext.Get<Workflow>(workflowName);
            workflow.ETag = newEtag;
        }

        private Task<IWorkflowStore> GetWorkflowStoreForCurrentTenantAsync()
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.featureContext);

            ITenantedWorkflowStoreFactory workflowStoreFactory = serviceProvider.GetRequiredService<ITenantedWorkflowStoreFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            return workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenantProvider.Root);
        }

        private async Task<CloudBlockBlob> GetCloudBlockBlobForCurrentTenantAsync(string blobId)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.featureContext);
            ITenantCloudBlobContainerFactory containerFactory = serviceProvider.GetRequiredService<ITenantCloudBlobContainerFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            CloudBlobContainer container = await containerFactory.GetBlobContainerForTenantAsync(
                tenantProvider.Root,
                TenantedBlobWorkflowStoreServiceCollectionExtensions.WorkflowStoreContainerDefinition).ConfigureAwait(false);

            return container.GetBlockBlobReference(blobId);
        }
    }
}
