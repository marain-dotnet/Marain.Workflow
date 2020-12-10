// <copyright file="WorkflowEventTypes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEvents
{
    /// <summary>
    /// Constants for the different types of CloudEvent that can be published by the workflow engine.
    /// </summary>
    public static class WorkflowEventTypes
    {
        /// <summary>
        /// Used when a new workflow instance has been created.
        /// </summary>
        public const string InstanceCreated = "workflow.instance.created";

        /// <summary>
        /// Used when a workflow instance transition has successfully completed.
        /// </summary>
        public const string TransitionCompleted = "workflow.instance.transition-completed";

        /// <summary>
        /// Used when a workflow instance has moved into the faulted state.
        /// </summary>
        public const string InstanceFaulted = "workflow.instance.faulted";
    }
}
