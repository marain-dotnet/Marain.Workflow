// <copyright file="MarainWorkflowServiceOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Client
{
    using System;

    /// <summary>
    /// Options for the Marain workflow service.
    /// </summary>
    public class MarainWorkflowServiceOptions
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