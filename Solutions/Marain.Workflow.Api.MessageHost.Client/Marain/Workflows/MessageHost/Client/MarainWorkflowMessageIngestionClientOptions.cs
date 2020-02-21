// <copyright file="WorkflowEngineClientServiceCollectionExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

using System;

namespace Marain.Workflows.MessageHost.Client
{
    public class MarainWorkflowMessageIngestionClientOptions
    {
        /// <summary>
        /// Gets or sets the URI for the workflow message ingestion host.
        /// </summary>
        public Uri BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the resource ID of the AAD app used for the easyAuth for
        /// the message ingestion host.
        /// </summary>
        public string ResourceIdForAuthentication { get; set; }
    }
}