// <copyright file="WorkflowEngineConfiguration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A class for storing the configuration for setting up the workflow engine.
    /// </summary>
    public class MarainWorkflowEngineOptions
    {
        /// <summary>
        /// Gets or sets the URI for the workflow engine.
        /// </summary>
        public Uri BaseUri { get; set; }
    }
}
