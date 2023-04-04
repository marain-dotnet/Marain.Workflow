// <copyright file="IWorkflowStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Storage;

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
        /// <param name="eTagExpected">
        /// If null the method will always attempt to fetch the workflow. If not null this might
        /// perform a conditional fetch and if the existing stored entity matches this eTag this
        /// method can indicate that by throwing a <see cref="WorkflowPreconditionFailedException"/>.
        /// </param>
        /// <returns>A <see cref="Task"/> which produces a <see cref="Workflow"/> wrapped in an <see cref="EntityWithETag{T}"/>.</returns>
        /// <exception cref="WorkflowNotFoundException">Thrown if no workflow with the specified <paramref name="workflowId"/> is found. </exception>
        Task<EntityWithETag<Workflow>> GetWorkflowAsync(string workflowId, string partitionKey = null, string eTagExpected = null);

        /// <summary>
        /// Upserts a <see cref="Workflow"/>.
        /// </summary>
        /// <param name="workflow">The workflow to upsert.</param>
        /// <param name="partitionKey">The partition key for the workflow. If not supplied, the Id will be used.</param>
        /// <param name="eTagExpected">eTag.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> which completes when the workflow has been upserted and
        /// produces the eTag for the entity.
        /// </returns>
        Task<string> UpsertWorkflowAsync(Workflow workflow, string partitionKey = null, string eTagExpected = null);
    }
}