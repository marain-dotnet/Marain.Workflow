// <copyright file="WorkflowMessageEnvelope.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using Corvus.Extensions.Json;
    using Corvus.Json;
    using Newtonsoft.Json;

    /// <summary>
    /// Wrapper for workflow messages being sent via a queuing system, e.g. an Event Hub.
    /// </summary>
    public class WorkflowMessageEnvelope
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.hosted.messageenvelope";

        /// <summary>
        /// Creates a new instance of the <see cref="WorkflowMessageEnvelope"/> class.
        /// </summary>
        /// <param name="properties">The <see cref="Properties"/>.</param>
        public WorkflowMessageEnvelope(IPropertyBag properties)
        {
            this.Properties = properties;
        }

        /// <summary>
        /// Gets the content type that will be used when serializing/deserializing.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the tenant associated with the request.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the message envelope contains a start
        /// workflow request.
        /// </summary>
        public bool IsStartWorkflowRequest => this.StartWorkflowInstanceRequest != null;

        /// <summary>
        /// Gets a value indicating whether the message envelope contains a trigger.
        /// </summary>
        public bool IsTrigger => this.Trigger != null;

        /// <summary>
        /// Gets a partition key that can be used with partitionable queuing systems.
        /// </summary>
        [JsonIgnore]
        public string PartitionKey =>
            this.IsTrigger ? this.Trigger.PartitionKey : this.StartWorkflowInstanceRequest.PartitionKey;

        /// <summary>
        /// Gets or sets the start workflow request to be sent.
        /// </summary>
        public StartWorkflowInstanceRequest StartWorkflowInstanceRequest { get; set; }

        /// <summary>
        /// Gets or sets the trigger to be sent.
        /// </summary>
        public IWorkflowTrigger Trigger { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the Operation tracking the progress of this work.
        /// </summary>
        public Guid OperationId { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="IPropertyBag"/> containing any additional data.
        /// </summary>
        public IPropertyBag Properties { get; set; }
    }
}