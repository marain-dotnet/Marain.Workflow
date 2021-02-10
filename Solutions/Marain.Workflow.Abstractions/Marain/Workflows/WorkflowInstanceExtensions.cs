// <copyright file="WorkflowInstanceExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Linq;

    /// <summary>
    /// Extension methods for the <see cref="WorkflowInstance"/> class.
    /// </summary>
    public static class WorkflowInstanceExtensions
    {
        /// <summary>
        /// Sets the status of the workflow instance to faulted.
        /// </summary>
        /// <param name="workflowInstance">The workflow instance to set to faulted.</param>
        /// <param name="errorMessage">A message describing the high-level cause of the fault.</param>
        /// <param name="data">A list of additional data associated with the fault.</param>
        public static void SetFaulted(this WorkflowInstance workflowInstance, string errorMessage, params (string Key, string Value)[] data)
        {
            workflowInstance.SetFaulted(
                errorMessage,
                data.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
