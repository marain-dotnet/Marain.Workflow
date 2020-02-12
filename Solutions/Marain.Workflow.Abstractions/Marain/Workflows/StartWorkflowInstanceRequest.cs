// <copyright file="StartWorkflowInstanceRequest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// Class containing the details required to start a new instance of a workflow.
    /// </summary>
    public class StartWorkflowInstanceRequest
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType =
            "application/vnd.marain.workflows.hosted.startworkflowinstancerequest";

        /// <summary>
        /// Gets the content type that will be used when serializing/deserializing.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the context. This will be passed to the
        /// <see cref="WorkflowEngine.CreateWorkflowInstanceAsync(string, string, string, string, IDictionary{string, string})" />
        /// method when creating the new instance.
        /// </summary>
        public IDictionary<string, string> Context { get; set; }

        /// <summary>
        /// Gets or sets the id of this request. This can be used for diagnostic purposes.
        /// </summary>
        /// <remarks>
        /// If not supplied, a generated value will be used.
        /// </remarks>
        public string RequestId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the Id of the workflow to create the new instance from.
        /// </summary>
        public string WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the Id that will be given to the new workflow instance.
        /// </summary>
        /// <remarks>
        /// If not supplied, a generated value will be used. If a value is supplied but that value
        /// is already in use, an error will be thrown.
        /// </remarks>
        public string WorkflowInstanceId { get; set; }

        /// <summary>
        /// Gets the partition key.
        /// </summary>
        [JsonIgnore]
        public string PartitionKey =>
            string.IsNullOrEmpty(this.WorkflowInstanceId) ? Guid.NewGuid().ToString() : this.WorkflowInstanceId;
    }
}