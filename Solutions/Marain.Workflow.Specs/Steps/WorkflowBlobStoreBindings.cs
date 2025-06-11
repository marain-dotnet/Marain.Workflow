// <copyright file="WorkflowBlobStoreBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;

    using Corvus.Extensions.Json;
    using Corvus.Storage.Azure.BlobStorage.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Testing.ReqnRoll;
    using Marain.Workflows.Specs.TestObjects;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Reqnroll;

    [Binding]
    public class WorkflowBlobStoreBindings
    {
        private readonly Dictionary<string, Workflow> workflows = new();
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
            this.workflows.Add(workflowName, DataCatalogWorkflowFactory.Create(workflowId, null));
        }

        [Given("I have stored the workflow called '(.*)' in the Azure storage workflow store")]
        [When("I store the workflow called '(.*)' in the Azure storage workflow store")]
        public async Task WhenIStoreTheWorkflowCalledInTheAzureStorageWorkflowStore(string workflowName)
        {
            IWorkflowStore workflowStore = await this.GetWorkflowStoreForCurrentTenantAsync().ConfigureAwait(false);

            Workflow workflow = this.workflows[workflowName];

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
            BlobClient blob = await this.GetBlobClientForCurrentTenantAsync(blobId).ConfigureAwait(false);
            bool exists = await blob.ExistsAsync().ConfigureAwait(false);

            Assert.IsTrue(exists);

            this.scenarioContext.Set(blob);
        }

        [Then("the blob with Id '(.*)' contains the workflow called '(.*)' serialized and encoded using UTF8")]
        public async Task ThenTheBlobWithIdContainsTheWorkflowCalledSerializedAndEncodedUsingUTF(string blobId, string workflowName)
        {
            IJsonSerializerSettingsProvider serializerSettingsProvider = ContainerBindings.GetServiceProvider(this.featureContext)
                .GetRequiredService<IJsonSerializerSettingsProvider>();

            BlobClient blob = await this.GetBlobClientForCurrentTenantAsync(blobId).ConfigureAwait(false);
            Response<BlobDownloadResult> getResponse = await blob.DownloadContentAsync().ConfigureAwait(false);
            string blobContents = getResponse.Value.Content.ToString();
            Workflow actualWorkflow = JsonConvert.DeserializeObject<Workflow>(blobContents, serializerSettingsProvider.Instance);

            Workflow expectedWorkflow = this.workflows[workflowName];

            // We don't need to check that every property has deserialized correctly - we're not testing JSON.NET.
            // It's enough to know that we've successfully deserialized the JSON from the blob and that it's the
            // expected workflow.
            Assert.AreEqual(expectedWorkflow.Id, actualWorkflow.Id);
        }

        [Then("the workflow called '(.*)' has its etag updated to match the etag of the blob with Id '(.*)'")]
        public async Task WhenTheWorkflowCalledHasItsEtagUpdatedToMatchTheEtagOfTheBlobWithId(string workflowName, string blobId)
        {
            Workflow workflow = this.workflows[workflowName];
            BlobClient blob = await this.GetBlobClientForCurrentTenantAsync(blobId).ConfigureAwait(false);
            Response<BlobProperties> propertiesResponse = await blob.GetPropertiesAsync().ConfigureAwait(false);

            // Note: we want the form that includes the double quotes (because that's what we were doing
            // back before the storage libraries made you choose explicitly), which is what the "H"
            // form gives us.
            Assert.AreEqual(propertiesResponse.Value.ETag.ToString("H"), workflow.ETag);
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
                this.workflows.Add(
                    workflowName,
                    await workflowStore.GetWorkflowAsync(workflowId).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex);
            }
        }

        [Then("the workflow called '(.*)' has an etag matching the workflow called '(.*)'")]
        public void ThenTheWorkflowCalledHasAnEtagMatchingTheWorkflowCalled(string workflow1Name, string workflow2Name)
        {
            Workflow workflow1 = this.workflows[workflow1Name];
            Workflow workflow2 = this.workflows[workflow2Name];

            Assert.AreEqual(workflow2.ETag, workflow1.ETag);
        }

        [Given("I change the description of the workflow definition called '(.*)' to '(.*)'")]
        public void GivenIChangeTheDescriptionOfTheWorkflowDefinitionCalledTo(string workflowName, string newDescription)
        {
            Workflow workflow = this.workflows[workflowName];
            workflow.Description = newDescription;
        }

        [Given("I set the etag of the workflow definition called '(.*)' to '(.*)'")]
        public void GivenISetTheEtagOfTheWorkflowDefinitionCalledTo(string workflowName, string newEtag)
        {
            Workflow workflow = this.workflows[workflowName];
            workflow.ETag = newEtag;
        }

        private Task<IWorkflowStore> GetWorkflowStoreForCurrentTenantAsync()
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.featureContext);

            ITenantedWorkflowStoreFactory workflowStoreFactory = serviceProvider.GetRequiredService<ITenantedWorkflowStoreFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            return workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenantProvider.Root);
        }

        private async Task<BlobClient> GetBlobClientForCurrentTenantAsync(string blobId)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.featureContext);
            IBlobContainerSourceWithTenantLegacyTransition containerFactory = serviceProvider.GetRequiredService<IBlobContainerSourceWithTenantLegacyTransition>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            BlobContainerClient container = await containerFactory.GetBlobContainerClientFromTenantAsync(
                tenantProvider.Root,
                "StorageConfiguration__workflowdefinitions",
                WorkflowAzureBlobTenancyPropertyKeys.Definitions).ConfigureAwait(false);

            return container.GetBlobClient(blobId);
        }
    }
}