// <copyright file="WorkflowStateMappingContext.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query.Mappers
{
    /// <summary>
    /// Mapping context for the <see cref="WorkflowStatesMapper"/>.
    /// </summary>
    public class WorkflowStateMappingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowStateMappingContext"/> class.
        /// </summary>
        /// <param name="currentTenantId">The <see cref="CurrentTenantId"/>.</param>
        /// <param name="workflowId">The <see cref="WorkflowId"/>.</param>
        public WorkflowStateMappingContext(
            string currentTenantId,
            string workflowId)
        {
            this.CurrentTenantId = currentTenantId;
            this.WorkflowId = workflowId;
        }

        /// <summary>
        /// Gets the Id of the requesting tenant.
        /// </summary>
        public string CurrentTenantId { get; }

        /// <summary>
        /// Gets the Id of the workflow to which the state belongs.
        /// </summary>
        public string WorkflowId { get; }
    }
}
