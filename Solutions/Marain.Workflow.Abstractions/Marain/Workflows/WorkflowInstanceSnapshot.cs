// <copyright file="WorkflowInstanceSnapshot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Immutable;
    using Marain.Workflows.Internal;

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
        /// Initializes a new instance of the <see cref="WorkflowInstanceSnapshot"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Id" />.</param>
        /// <param name="version">The <see cref="Version" />.</param>
        /// <param name="workflowId">The <see cref="WorkflowId" />.</param>
        /// <param name="stateId">The <see cref="StateId" />.</param>
        /// <param name="status">The <see cref="Status" />.</param>
        /// <param name="context">The <see cref="Context" />.</param>
        /// <param name="activeTransitionState">The <see cref="ActiveTransitionState" />.</param>
        public WorkflowInstanceSnapshot(
            string id,
            int version,
            string workflowId,
            string stateId,
            WorkflowStatus status,
            IImmutableDictionary<string, string> context,
            WorkflowInstanceActiveTransitionState activeTransitionState)
        {
            this.Id = id;
            this.Version = version;
            this.WorkflowId = workflowId;
            this.StateId = stateId;
            this.Status = status;
            this.Context = context;
            this.ActiveTransitionState = activeTransitionState;
        }

        /// <summary>
        /// Gets the registered content type used when this object is serialized/deserialized.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets the unique Id of this workflow instance.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the version number of the snapshot.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Gets the Id of the <see cref="Workflow" /> that this is an instance of.
        /// </summary>
        public string WorkflowId { get; }

        /// <summary>
        /// Gets the Id of the current <see cref="WorkflowState" /> for this instance.
        /// </summary>
        public string StateId { get; }

        /// <summary>
        /// Gets the status of this instance. A <see cref="WorkflowInstance" /> can only process
        /// new triggers if it's in the <see cref="WorkflowStatus.Waiting" /> status.
        /// </summary>
        public WorkflowStatus Status { get; }

        /// <summary>
        /// Gets data related to the active transition. This should only be set if the <see cref="Status"/> of
        /// the instance is <see cref="WorkflowStatus.ProcessingTransition"/>.
        /// </summary>
        public WorkflowInstanceActiveTransitionState ActiveTransitionState { get; }

        /// <summary>
        /// Gets a dictionary of useful related pieces of data to be stored with
        /// the workflow instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Workflow instances will almost always have related data that are stored independently
        /// of the instance itself. However, to make processing triggers more efficient, you
        /// can choose to add specific pieces of that data.
        /// </para>
        /// </remarks>
        public IImmutableDictionary<string, string> Context { get; } = ImmutableDictionary<string, string>.Empty;
    }
}
