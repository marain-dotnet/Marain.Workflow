// <copyright file="ExternalServiceWorkflowRequest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the structure of the HTTP request body sent to external services by an
    /// <see cref="InvokeExternalServiceAction"/>, or by an <see cref="InvokeExternalServiceCondition"/>
    /// that has been configured to use POST.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="Workflow"/> can invoke external user-supplied HTTP services (often described
    /// as 'webhooks'), but those services must be designed specifically for that purpose. (We
    /// don't support invoking any arbitrary endpoint. If you want to call out to some third party
    /// service, you should implement an internal HTTP endpoint designed specifically to be invoked
    /// by a workflow, which then calls out to whatever external service you need to communicate
    /// with.)
    /// </para>
    /// <para>
    /// External services will always be invoked with an HTTP POST, and this class defines how the
    /// content of that request will be structured.
    /// </para>
    /// </remarks>
    public class ExternalServiceWorkflowRequest
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.externalservicerequest";

        /// <summary>
        /// Gets the content type for serialization.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the unique identifier of the <see cref="Workflow"/> that caused this request.
        /// </summary>
        public string WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the <see cref="WorkflowInstance"/> that caused
        /// this request.
        /// </summary>
        public string WorkflowInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="WorkflowState"/> that the <see cref="WorkflowInstance"/>
        /// is currently in.
        /// </summary>
        public string WorkflowInstanceCurrentStateId { get; set; }

        /// <summary>
        /// Gets or sets the status of the <see cref="WorkflowInstance"/>.
        /// </summary>
        public string WorkflowInstanceCurrentStatus { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the action or condition that caused this request.
        /// </summary>
        /// <remarks>
        /// This will correspond to either the <see cref="InvokeExternalServiceAction.Id"/> or
        /// <see cref="InvokeExternalServiceCondition.Id"/> depending on whether this request
        /// originated from an action or condition respectively.
        /// </remarks>
        public string InvokedById { get; set; }

        /// <summary>
        /// Gets or sets the trigger that resulted in this request being made.
        /// </summary>
        public IWorkflowTrigger Trigger { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of values from the workflow instance context.
        /// </summary>
        /// <remarks>
        /// This will only include values specified by the originating
        /// <see cref="InvokeExternalServiceAction.ContextItemsToInclude"/> or
        /// <see cref="InvokeExternalServiceCondition.ContextItemsToInclude"/>, and will be null if
        /// that list is null or empty.
        /// </remarks>
        public IDictionary<string, string> ContextProperties { get; set; }
    }
}