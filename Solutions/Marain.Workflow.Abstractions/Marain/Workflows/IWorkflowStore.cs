// <copyright file="IWorkflowStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;

    /// <summary>
    /// Abstracts the underlying storage mechanism for workflows.
    /// </summary>
    public interface IWorkflowStore
    {
        /// <summary>
        /// Gets a <see cref="Workflow"/>.
        /// </summary>
        /// <param name="workflowId">The workflow ID.</param>
        /// <param name="partitionKey">The partition key for the workflow. If not supplied, the Id will be used.</param>
        /// <returns>A <see cref="Task"/> which completes with the <see cref="Workflow"/>.</returns>
        Task<(Workflow Workflow, string ETag)> GetWorkflowAsync(string workflowId, string partitionKey = null);

        /// <summary>
        /// Upserts a <see cref="Workflow"/>.
        /// </summary>
        /// <param name="workflow">The workflow to upsert.</param>
        /// <param name="partitionKey">The partition key for the workflow. If not supplied, the Id will be used.</param>
        /// <returns>A <see cref="Task"/> which completes when the workflow has been upserted.</returns>
        Task<string> UpsertWorkflowAsync(Workflow workflow, string partitionKey = null, string eTag = null);
    }
}