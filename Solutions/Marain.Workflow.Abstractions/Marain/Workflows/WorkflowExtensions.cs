// <copyright file="WorkflowExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="Workflow"/>.
    /// </summary>
    public static class WorkflowExtensions
    {
        /// <summary>
        /// Gets the <see cref="WorkflowState" /> that has been defined as the initial state of the workflow.
        /// </summary>
        /// <param name="workflow">The workflow.</param>
        /// <returns>
        /// The <see cref="WorkflowState" /> that will be the first state for new <see cref="WorkflowInstance" />s
        /// created from this Workflow.
        /// </returns>
        public static WorkflowState GetInitialState(this Workflow workflow)
        {
            return workflow.GetState(workflow.InitialStateId);
        }

        /// <summary>
        /// Gets the <see cref="WorkflowState" /> with the given Id.
        /// </summary>
        /// <param name="workflow">The workflow.</param>
        /// <param name="id">The id of the state to retrieve.</param>
        /// <returns>
        /// The <see cref="WorkflowState" /> with the given Id, or null if no matching state is present
        /// in the <see cref="Workflow.States" /> collection.
        /// </returns>
        public static WorkflowState GetState(this Workflow workflow, string id)
        {
            if (workflow.States.TryGetValue(id, out WorkflowState value))
            {
                return value;
            }

            return null;
        }
    }
}