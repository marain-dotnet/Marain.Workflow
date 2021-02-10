// <copyright file="EventStreamProcessorConfiguration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.EventStreamProcessor.Configuration
{
    /// <summary>
    /// Configuration for the event stream processor.
    /// </summary>
    public class EventStreamProcessorConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamProcessorConfiguration"/> class.
        /// </summary>
        public EventStreamProcessorConfiguration()
        {
            this.CheckTenantEnrollmentStatus = true;
            this.RecursiveTenantSearch = false;
            this.WorkflowDependentServiceTenantIds = new string[0];
        }

        /// <summary>
        /// Gets or sets a value indicating whether tenants should be checked to ensure they are enrolled for workflow
        /// prior to starting an event stream processor for a tenant.
        /// </summary>
        /// <remarks>
        /// If you know that all client tenants are enrolled for workflow, then set this to false. Otherwise it should
        /// be set to true.
        /// </remarks>
        public bool CheckTenantEnrollmentStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the search for enrolled tenants should be recursive.
        /// </summary>
        /// <remarks>
        /// If you have a client tenant hierarchy, set this to true to ensure that the entire hierarchy is search.
        /// Otherwise, set it to false.
        /// </remarks>
        public bool RecursiveTenantSearch { get; set; }

        /// <summary>
        /// Gets or sets an array of Ids of service tenants that have a direct dependency on workflow. Delegated tenants
        /// for these services will also have stream processors initialised.
        /// </summary>
        public string[] WorkflowDependentServiceTenantIds { get; set; }
    }
}
