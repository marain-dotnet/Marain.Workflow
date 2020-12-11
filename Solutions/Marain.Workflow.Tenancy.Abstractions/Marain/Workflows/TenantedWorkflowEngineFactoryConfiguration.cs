// <copyright file="TenantedWorkflowEngineFactoryConfiguration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    /// <summary>
    /// Configuration data for the <see cref="TenantedWorkflowEngineFactory"/>.
    /// </summary>
    public class TenantedWorkflowEngineFactoryConfiguration
    {
        /// <summary>
        /// Gets or sets the base source name for CloudEvents (https://github.com/cloudevents/spec/blob/v1.0/spec.md#source).
        /// </summary>
        /// <remarks>
        /// This will be concatenated with the Id of the tenant to produce the full source. By default, this configuration
        /// item will be set to the Azure Subscription Id and Resource Group Name in which the workflow engine is being
        /// hosted.
        /// </remarks>
        public string CloudEventBaseSourceName { get; set; }
    }
}