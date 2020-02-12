// <copyright file="IWorkflowCondition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The interface for classes that implement a condition that is evaluated as part
    /// of a <see cref="WorkflowInstance" /> processing an <see cref="IWorkflowTrigger" />.
    /// </summary>
    public interface IWorkflowCondition
    {
        /// <summary>
        /// Gets the content type that will be used when serializing/deserializing.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets or sets the Id of this condition.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        /// <param name="instance">
        /// The instance on which the action is being executed.
        /// </param>
        /// <param name="trigger">
        /// The trigger that has caused the action to execute.
        /// </param>
        /// <returns>
        /// A <see cref="Task" /> whose result will contain the outcome of
        /// evaluating this condition.
        /// </returns>
        Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger);

        /// <summary>
        /// Retrieves a list of interests for this condition. These will be collated by the
        /// current <see cref="WorkflowState" /> when its <see cref="WorkflowState.GetInterests" />
        /// method is called to update interests for the <see cref="WorkflowInstance" />.
        /// </summary>
        /// <param name="instance">
        /// The instance that this condition belongs to.
        /// </param>
        /// <returns>
        /// An IEnumerable{string} containing the interests for this condition.
        /// </returns>
        IEnumerable<string> GetInterests(WorkflowInstance instance);
    }
}