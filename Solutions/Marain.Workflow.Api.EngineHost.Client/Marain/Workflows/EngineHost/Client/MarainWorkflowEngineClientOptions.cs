// <copyright file="WorkflowEngineConfiguration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System;

namespace Marain.Workflows.EngineHost.Client
{
    /// <summary>
    /// A class for storing the configuration for setting up the workflow engine client.
    /// </summary>
    public class MarainWorkflowEngineClientOptions
    {
        /// <summary>
        /// Gets or sets the URI for the workflow engine.
        /// </summary>
        public Uri BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the resource ID of the AAD app used for the easyAuth for
        /// the engine host.
        /// </summary>
        public string ResourceIdForAuthentication { get; set; }
    }
}
