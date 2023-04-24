// <copyright file="WorkflowStateDictionaryExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;

    /// <summary>
    /// Extensions methods for adding <see cref="WorkflowState"/> entries to a dictionary.
    /// </summary>
    public static class WorkflowStateDictionaryExtensions
    {
        /// <summary>
        /// Defines a new state and adds it to the dictionary.
        /// </summary>
        /// <param name="states">The dictionary of states.</param>
        /// <param name="id">The id for the state.</param>
        /// <param name="displayName">The display name for the state.</param>
        /// <param name="description">The description for the state.</param>
        /// <returns>The newly created state.</returns>
        public static WorkflowState AddState(
            this Dictionary<string, WorkflowState> states,
            string id,
            string displayName = null,
            string description = null)
        {
            var state = new WorkflowState
            {
                Id = id,
                DisplayName = displayName,
                Description = description,
            };

            states.Add(id, state);

            return state;
        }
    }
}