// <copyright file="IWorkflowAction.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;

    /// <summary>
    /// The interface for classes that implement an action that's taken at a specific point
    /// in a workflow. This may be an action that is part of a transition, used to move a
    /// workflow between states. Or it could be an action taken when entering or leaving a
    /// state.
    /// </summary>
    /// <remarks>
    /// Actions should be small, self contained units of code that do one thing. Think
    /// Single Responsibility all the way.
    /// </remarks>
    public interface IWorkflowAction
    {
        /// <summary>
        /// Gets the content type that will be used when serializing/deserializing.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets or sets the Id of this action.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="instance">The instance on which the action is being executed.</param>
        /// <param name="trigger">The trigger that has caused the action to execute.</param>
        /// <returns>A <see cref="Task" /> that completes when the action has finished executing.</returns>
        Task<WorkflowActionResult> ExecuteAsync(WorkflowInstance instance, IWorkflowTrigger trigger);
    }
}