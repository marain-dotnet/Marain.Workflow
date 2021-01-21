// <copyright file="WorkflowActionResultExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for the <see cref="WorkflowActionResult"/> class.
    /// </summary>
    public static class WorkflowActionResultExtensions
    {
        /// <summary>
        /// Merges the context updates from the supplied results.
        /// </summary>
        /// <param name="results">The results to merge.</param>
        /// <returns>The merged result.</returns>
        /// <remarks>
        /// If there are multiple results that attempt to update the same context item, behaviour is non-deterministic.
        /// </remarks>
        public static WorkflowActionResult Collapse(this IEnumerable<WorkflowActionResult> results)
        {
            // TODO: I think we should care about duplicates and throw an exception of we detect any.
            var itemsToAddOrUpdate = new Dictionary<string, string>();
            results.SelectMany(x => x.ContextItemsToAddOrUpdate).ForEach(x => itemsToAddOrUpdate[x.Key] = x.Value);

            IEnumerable<string> itemsToRemove = results.SelectMany(x => x.ContextItemsToRemove).Distinct();

            return new WorkflowActionResult(itemsToAddOrUpdate, itemsToRemove);
        }
    }
}
