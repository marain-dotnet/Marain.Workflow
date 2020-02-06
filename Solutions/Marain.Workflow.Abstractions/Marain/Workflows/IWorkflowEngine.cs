﻿// <copyright file="IWorkflowEngine.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The workflow engine. Manages the processing of triggers (classes inheriting <see cref="IWorkflowTrigger" />
    /// by <see cref="WorkflowInstance" />s.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The workflow engine should be supplied with <see cref="IWorkflowTrigger" /> messages from an external source
    /// such as a queue. It handles retrieval of the workflow instances
    /// that could potentially accept a trigger, and the requesting and releasing of shared leases for each instance
    /// whilst it is passed the trigger to process.
    /// </para>
    /// </remarks>
    public interface IWorkflowEngine
    {
        /// <summary>
        /// Applies the trigger to the specified <see cref="WorkflowInstance" />.
        /// </summary>
        /// <param name="trigger">The trigger to apply.</param>
        /// <param name="workflowInstanceId">The Id of the <see cref="WorkflowInstance" /> to apply the trigger to.</param>
        /// <param name="partitionKey">The partition key for the instance. If not supplied, the Id will be used.</param>
        /// <returns>The <see cref="Task" />.</returns>
        Task ProcessTriggerAsync(IWorkflowTrigger trigger, string workflowInstanceId, string partitionKey = null);

        /// <summary>
        /// Starts a new instance of the specified <see cref="Workflow"/>.
        /// </summary>
        /// <param name="request">The request containing the details of the new workflow instance.</param>
        /// <returns>A <see cref="Task"/> which completes with the created workflow instance.</returns>
        Task<WorkflowInstance> StartWorkflowInstanceAsync(StartWorkflowInstanceRequest request);

        /// <summary>
        /// Starts a new instance of the specified <see cref="Workflow"/>.
        /// </summary>
        /// <param name="workflowId">The workflow id of the workflow for which to create the instance.</param>
        /// <param name="workflowPartitionKey">The partition key for the workflow. If not supplied, the Id will be used.</param>
        /// <param name="instanceId">The id of the new instance.</param>
        /// <param name="instancePartitionKey">The partition key for the instance. If not supplied, the Id will be used.</param>
        /// <param name="context">The context for the instance.</param>
        /// <returns>
        /// A <see cref="Task"/> which completes with the created workflow instance.
        /// </returns>
        Task<WorkflowInstance> StartWorkflowInstanceAsync(
            string workflowId,
            string workflowPartitionKey = null,
            string instanceId = null,
            string instancePartitionKey = null,
            IDictionary<string, string> context = null);

        /// <summary>
        /// Gets a <see cref="Workflow"/>.
        /// </summary>
        /// <param name="workflowId">The workflow ID.</param>
        /// <param name="partitionKey">The partition key for the workflow. If not supplied, the Id will be used.</param>
        /// <returns>A <see cref="Task"/> which completes with the <see cref="Workflow"/>.</returns>
        Task<Workflow> GetWorkflowAsync(string workflowId, string partitionKey = null);

        /// <summary>
        /// Upserts a <see cref="Workflow"/>.
        /// </summary>
        /// <param name="workflow">The workflow to upsert.</param>
        /// <param name="partitionKey">The partition key for the workflow. If not supplied, the Id will be used.</param>
        /// <returns>A <see cref="Task"/> which completes when the workflow has been upserted.</returns>
        Task UpsertWorkflowAsync(Workflow workflow, string partitionKey = null);

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

        /// <summary>
        /// Gets the workflow instance IDs that correspond to a particular set of subjects.
        /// </summary>
        /// <param name="subjects">The list of subjects.</param>
        /// <param name="pageSize">The number of items to return.</param>
        /// <param name="pageNumber">The page of items to return.</param>
        /// <returns>A <see cref="Task"/> which completes with the specified page of workflow instance ids.</returns>
        Task<IEnumerable<string>> GetMatchingWorkflowInstanceIdsForSubjectsAsync(
            IEnumerable<string> subjects,
            int pageSize,
            int pageNumber);

        /// <summary>
        /// Gets the number of instances that match a particular set of subjects.
        /// </summary>
        /// <param name="subjects">The list of subjects.</param>
        /// <returns>A task which completes with the number of subjects that currently match the instance.</returns>
        Task<int> GetMatchingWorkflowInstanceCountForSubjectsAsync(IEnumerable<string> subjects);
    }
}