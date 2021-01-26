// <copyright file="IWorkflowInstanceInterestsIndexStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Abstracts the underlying storage mechanism for a lookup of workflow instances by their interests.
    /// </summary>
    public interface IWorkflowInstanceInterestsIndexStore
    {
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
