// <copyright file="WorkflowStatus.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    /// <summary>
    ///     List of valid statuses for an <see cref="WorkflowInstance" />.
    /// </summary>
    /// <remarks>
    ///     Note that "Status" is completely separate from "State". States are the different
    ///     parts that a workflow instance moves through during its lifetime, with state
    ///     changes triggered by <see cref="IWorkflowTrigger" /> and actioned by
    ///     <see cref="WorkflowTransition" />s. A <see cref="WorkflowInstance" />'s Status
    ///     is an overall indicator of whether the instance is available for processing
    ///     triggers or not.
    /// </remarks>
    public enum WorkflowStatus
    {
        /// <summary>
        ///     The <see cref="WorkflowStatus" /> has been created and persisted, but
        ///     initialization has not yet completed. This implies that <see cref="WorkflowEngine.CreateWorkflowInstanceAsync(string, string, string, System.Collections.Generic.Dictionary{string, string})"/> is still in progress,
        ///     or has aborted without completing.
        /// </summary>
        Initializing,

        /// <summary>
        ///     The instance is available to process further triggers.
        /// </summary>
        Waiting,

        /// <summary>
        ///     The instance is processing a trigger.
        /// </summary>
        ProcessingTransition,

        /// <summary>
        ///     The <see cref="WorkflowInstance" /> has faulted (normally as a result of an
        ///     <see cref="IWorkflowAction" /> or <see cref="IWorkflowCondition" /> throwing
        ///     an exception. It will require manual recovery before it is able to process
        ///     any further triggers.
        /// </summary>
        Faulted,

        /// <summary>
        ///     The <see cref="WorkflowInstance" /> has entered a state from which there are
        ///     no transitions. This means it will not be able to accept any further triggers.
        /// </summary>
        /// <remarks>
        ///     Depending on the <see cref="Workflow" /> in question, it is possible that a
        ///     <see cref="WorkflowInstance" /> will never reach the completed state.
        /// </remarks>
        Complete,
    }
}