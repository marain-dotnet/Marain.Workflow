// <copyright file="WorkflowInstanceSnapshot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Internal
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// Object holding the current state of a <see cref="WorkflowInstance"/>.
    /// </summary>
    public class WorkflowInstanceSnapshot
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.snapshots.workflowinstance.v1";

        /// <summary>
        /// Gets the registered content type used when this object is serialized/deserialized.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the unique Id of this workflow instance.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the version number of the snapshot.
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Workflow" /> that this is an instance of.
        /// </summary>
        public string WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the current <see cref="WorkflowState" /> for this instance.
        /// </summary>
        public string StateId { get; set; }

        /// <summary>
        /// Gets or sets the status of this instance. A <see cref="WorkflowInstance" /> can only process
        /// new triggers if it's in the <see cref="WorkflowStatus.Waiting" /> status.
        /// </summary>
        public WorkflowStatus Status { get; set; }

        /// <summary>
        /// Gets or sets data related to the active transition. This should only be set if the <see cref="Status"/> of
        /// the instance is <see cref="WorkflowStatus.ProcessingTransition"/>.
        /// </summary>
        public WorkflowInstanceActiveTransitionState ActiveTransitionState { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of useful related pieces of data to be stored with
        /// the workflow instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Workflow instances will almost always have related data that are stored independently
        /// of the instance itself. However, to make processing triggers more efficient, you
        /// can choose to add specific pieces of that data.
        /// </para>
        /// </remarks>
        public IImmutableDictionary<string, string> Context { get; set; } = ImmutableDictionary<string, string>.Empty;

        /// <summary>
        /// Gets or sets the list of interests for this workflow instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This list is automatically updated whenever the instance's current <see cref="StateId" />
        /// changes. The list will be populated by calling <see cref="WorkflowState.GetInterests" />
        /// on the current state and  will always include the value of the <see cref="Id" /> property.
        /// </para>
        /// <para>
        /// This list is intended for use by the <see cref="IWorkflowInstanceStore.GetMatchingWorkflowInstanceIdsForSubjectsAsync(IEnumerable{string}, int, int)" />
        /// method to search for <see cref="WorkflowInstance" />s whose interests match at least one of the
        /// current trigger's subjects (see <see cref="IWorkflowTrigger.GetSubjects" />).
        /// </para>
        /// </remarks>
        public IImmutableList<string> Interests { get; set; } = ImmutableList<string>.Empty;
    }
}
