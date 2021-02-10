// <copyright file="WorkflowActionResult.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// Result from executing <see cref="IWorkflowAction.ExecuteAsync(WorkflowInstance, IWorkflowTrigger)"/>.
    /// </summary>
    public class WorkflowActionResult
    {
        /// <summary>
        /// A default instance of <see cref="WorkflowActionResult"/> containing no data.
        /// </summary>
        public static readonly WorkflowActionResult Empty = new WorkflowActionResult(new Dictionary<string, string>(), new string[0]);

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowActionResult"/> class.
        /// </summary>
        /// <param name="contextItemsToAddOrUpdate">The <see cref="ContextItemsToAddOrUpdate"/>.</param>
        /// <param name="contextItemsToRemove">The <see cref="ContextItemsToRemove"/>.</param>
        public WorkflowActionResult(
            IDictionary<string, string> contextItemsToAddOrUpdate,
            IEnumerable<string> contextItemsToRemove)
        {
            this.ContextItemsToAddOrUpdate = contextItemsToAddOrUpdate.ToImmutableDictionary();
            this.ContextItemsToRemove = contextItemsToRemove.ToImmutableList();
        }

        /// <summary>
        /// Gets the list of context item additions or updates resulting from the workflow action.
        /// </summary>
        public IImmutableDictionary<string, string> ContextItemsToAddOrUpdate { get; }

        /// <summary>
        /// Gets the list of context items to remove as a result of the workflow action.
        /// </summary>
        public IImmutableList<string> ContextItemsToRemove { get; }
    }
}