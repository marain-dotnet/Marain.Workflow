// <copyright file="CosmosWorkflowStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System.Net;
    using System.Threading.Tasks;
    using Corvus.Retry;
    using Corvus.Storage;
    using Marain.Workflows;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// A CosmosDb implementation of the workflow store.
    /// </summary>
    public class CosmosWorkflowStore : IWorkflowStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosWorkflowInstanceStore"/> class.
        /// </summary>
        /// <param name="workflowContainer">The repository in which to store workflows.</param>
        public CosmosWorkflowStore(
            Container workflowContainer)
        {
            this.Container = workflowContainer;
        }

        /// <summary>
        /// Gets the underlying Cosmos <see cref="Container"/> for this workflow instance store.
        /// </summary>
        public Container Container { get; }

        /// <inheritdoc/>
        public async Task<EntityWithETag<Workflow>> GetWorkflowAsync(string workflowId, string partitionKey, string eTagExpected)
        {
            try
            {
                ItemResponse<Workflow> itemResponse = await Retriable.RetryAsync(() =>
                    this.Container.ReadItemAsync<Workflow>(
                        workflowId,
                        new PartitionKey(partitionKey ?? workflowId)))
                    .ConfigureAwait(false);

                Workflow workflow = itemResponse.Resource;
                string returnedETag = itemResponse.ETag;

                return new EntityWithETag<Workflow>(workflow, returnedETag);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new WorkflowNotFoundException($"The workflow with id {workflowId} was not found", ex);
            }
        }

        /// <inheritdoc/>
        public Task<string> UpsertWorkflowAsync(Workflow workflow, string partitionKey = null, string eTag = null)
        {
            return Retriable.RetryAsync(() =>
            {
                if (string.IsNullOrEmpty(eTag))
                {
                    // This seems to be saying: "if you don't provide an eTag, the workflow you provided will be pushed
                    // up to Cosmos. Is this the behaviour we want?
                    return this.CreateWorkflowAsync(workflow, partitionKey);
                }

                return this.UpdateWorkflowAsync(workflow, partitionKey);
            });
        }

        private async Task<string> CreateWorkflowAsync(Workflow workflow, string partitionKey = null)
        {
            try
            {
                ItemResponse<Workflow> response = await this.Container.CreateItemAsync(
                    workflow,
                    new PartitionKey(partitionKey ?? workflow.Id)).ConfigureAwait(false);

                return response.ETag;
            }
            catch (CosmosException cex) when (cex.StatusCode == HttpStatusCode.Conflict)
            {
                // Can't save a new instance because it already exists
                throw new WorkflowConflictException();
            }
        }

        private async Task<string> UpdateWorkflowAsync(Workflow workflow, string partitionKey = null, string eTag = null)
        {
            try
            {
                ItemResponse<Workflow> response = await this.Container.UpsertItemAsync(
                    workflow,
                    new PartitionKey(partitionKey ?? workflow.Id),
                    new ItemRequestOptions { IfMatchEtag = eTag }).ConfigureAwait(false);

                return response.ETag;
            }
            catch (CosmosException cex) when (cex.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                // Return 409 here as well as with the create to stay consistent with other implementations.
                throw new WorkflowPreconditionFailedException();
            }
        }
    }
}