// <copyright file="ExternalServiceWorkflowResponse.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the structure of the optional HTTP response from external services which have been invoked through
    /// an <see cref="InvokeExternalServiceAction"/>. The response allows modification of the
    /// <see cref="WorkflowInstance.Context"/> properties.
    /// </summary>
    public class ExternalServiceWorkflowResponse
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.externalserviceresponse";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalServiceWorkflowResponse"/> class.
        /// </summary>
        public ExternalServiceWorkflowResponse()
        {
            this.ContextValuesToSetOrAdd = new Dictionary<string, string>();
            this.ContextValuesToRemove = new List<string>();
        }

        /// <summary>
        /// Gets the content type for serialization.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets a list of context values to update.
        /// </summary>
        public IDictionary<string, string> ContextValuesToSetOrAdd { get; set; }

        /// <summary>
        /// Gets or sets a list of context values that should be removed.
        /// </summary>
        public IList<string> ContextValuesToRemove { get; set; }
    }
}