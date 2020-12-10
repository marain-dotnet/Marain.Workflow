// <copyright file="WorkflowInstanceCreationCloudEventData.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEvents
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the standard data we include when publishing a CloudEvent related to a workflow instance from the
    /// workflow engine.
    /// </summary>
    public class WorkflowInstanceCreationCloudEventData
    {
        /// <summary>
        /// The registered content type.
        /// </summary>
        public const string RegisteredContentType = "application/marain.workflows.workflowinstance.creationcloudeventdata";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceCreationCloudEventData"/> class.
        /// </summary>
        /// <param name="workflowInstanceId">The <see cref="WorkflowInstanceId"/>.</param>
        public WorkflowInstanceCreationCloudEventData(string workflowInstanceId)
        {
            this.WorkflowInstanceId = workflowInstanceId;
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
        /// Gets or sets the Id of the workflow definition for the workflow instance.
        /// </summary>
        public string WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the state that the instance was in after the activity which produced the event.
        /// </summary>
        public string NewState { get; set; }

        /// <summary>
        /// Gets or sets the status of the instance after the activity which produced the event.
        /// </summary>
        public WorkflowStatus NewStatus { get; set; }

        /// <summary>
        /// Gets or sets the context items provided with the request to create a new instance.
        /// </summary>
        public IDictionary<string, string> SuppliedContext { get; set; }

        /// <summary>
        /// Gets or sets the workflow instance context items after the activity which produced the event.
        /// </summary>
        public IDictionary<string, string> NewContext { get; set; }
    }
}
