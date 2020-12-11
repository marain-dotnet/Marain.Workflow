// <copyright file="WorkflowInstanceTransitionCloudEventData.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEvents
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the standard data we include when publishing a CloudEvent related to a workflow instance from the
    /// workflow engine.
    /// </summary>
    public class WorkflowInstanceTransitionCloudEventData
    {
        /// <summary>
        /// The registered content type.
        /// </summary>
        public const string RegisteredContentType = "application/marain.workflows.workflowinstance.transitioncloudeventdata";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceTransitionCloudEventData"/> class.
        /// </summary>
        /// <param name="workflowInstanceId">The <see cref="WorkflowInstanceId"/>.</param>
        /// <param name="trigger">The (optional) <see cref="Trigger"/>.</param>
        public WorkflowInstanceTransitionCloudEventData(string workflowInstanceId, IWorkflowTrigger trigger = null)
        {
            this.WorkflowInstanceId = workflowInstanceId;
            this.Trigger = trigger;
        }

        /// <summary>
        /// Gets the <see cref="RegisteredContentType"/>.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets the workflow instance Id to which the event relates.
        /// </summary>
        public string WorkflowInstanceId { get; }

        /// <summary>
        /// Gets the workflow trigger that caused the event to be published.
        /// </summary>
        public IWorkflowTrigger Trigger { get; }

        /// <summary>
        /// Gets or sets the Id of the workflow definition for the workflow instance.
        /// </summary>
        public string WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the state that the instance was in prior to the activity which produced the event.
        /// </summary>
        public string PreviousState { get; set; }

        /// <summary>
        /// Gets or sets the status that the instance was in prior to the activity which produced the event.
        /// </summary>
        public WorkflowStatus PreviousStatus { get; set; }

        /// <summary>
        /// Gets or sets the Id of the transition that took place, or null if no transition occurred.
        /// </summary>
        public string TransitionId { get; set; }

        /// <summary>
        /// Gets or sets the state that the instance was in after the activity which produced the event.
        /// </summary>
        public string NewState { get; set; }

        /// <summary>
        /// Gets or sets the status of the instance after the activity which produced the event.
        /// </summary>
        public WorkflowStatus NewStatus { get; set; }

        /// <summary>
        /// Gets or sets the workflow instance context items prior to the activity which produced the event.
        /// </summary>
        public IDictionary<string, string> PreviousContext { get; set; }

        /// <summary>
        /// Gets or sets the workflow instance context items after the activity which produced the event.
        /// </summary>
        public IDictionary<string, string> NewContext { get; set; }
    }
}
