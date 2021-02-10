// <copyright file="IWorkflowInstanceStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;

    /// <summary>
    /// Abstracts the underlying storage mechanism for workflow instances.
    /// </summary>
    public interface IWorkflowInstanceStore
    {
        /// <summary>
        /// Deletes a <see cref="WorkflowInstance"/>.
        /// </summary>
        /// <param name="workflowInstanceId">The ID of the <see cref="WorkflowInstance"/> to delete.</param>
        /// <param name="partitionKey">The partition key for the instance. If not supplied, the Id will be used.</param>
        /// <returns>A <see cref="Task"/> which completes when the workflow instance has been deleted.</returns>
        Task DeleteWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null);

        /// <summary>
        /// Upserts a <see cref="WorkflowInstance"/>.
        /// </summary>
        /// <param name="workflowInstance">The <see cref="WorkflowInstance"/> to insert/update.</param>
        /// <param name="partitionKey">The partition key for the instance. If not supplied, the Id will be used.</param>
        /// <returns>A <see cref="Task"/> which completes when the workflow instance has been stored.</returns>
        Task UpsertWorkflowInstanceAsync(WorkflowInstance workflowInstance, string partitionKey = null);

        /// <summary>
        /// Gets a <see cref="WorkflowInstance"/>.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance ID.</param>
        /// <param name="partitionKey">The partition key for the instance. If not supplied, the Id will be used.</param>
        /// <returns>A <see cref="Task"/> which completes with the <see cref="WorkflowInstance"/>.</returns>
        Task<WorkflowInstance> GetWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null);
    }
}
