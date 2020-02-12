﻿// <copyright file="CosmosWorkflowStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System.Net;
    using System.Threading.Tasks;
    using Corvus.Retry;
    using Marain.Workflows;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// A CosmosDb implementation of the workflow store.
    /// </summary>
    public class CosmosWorkflowStore : IWorkflowStore
    {
        private readonly Container workflowContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowEngine"/> class.
        /// </summary>
        /// <param name="workflowContainer">The repository in which to store workflows.</param>
        public CosmosWorkflowStore(
            Container workflowContainer)
        {
            this.workflowContainer = workflowContainer;
        }

        /// <inheritdoc/>
        public async Task<Workflow> GetWorkflowAsync(string workflowId, string partitionKey = null)
        {
            try
            {
                ItemResponse<Workflow> itemResponse = await Retriable.RetryAsync(() =>
                    this.workflowContainer.ReadItemAsync<Workflow>(
                        workflowId,
                        new PartitionKey(partitionKey ?? workflowId)))
                    .ConfigureAwait(false);

                return itemResponse.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new WorkflowNotFoundException($"The workflow with id {workflowId} was not found", ex);
            }
        }

        /// <inheritdoc/>
        public Task UpsertWorkflowAsync(Workflow workflow, string partitionKey = null)
        {
            return Retriable.RetryAsync(() =>
                this.workflowContainer.UpsertItemAsync(
                    workflow,
                    new PartitionKey(partitionKey ?? workflow.Id),
                    new ItemRequestOptions { IfMatchEtag = workflow.ETag }));
        }
    }
}
