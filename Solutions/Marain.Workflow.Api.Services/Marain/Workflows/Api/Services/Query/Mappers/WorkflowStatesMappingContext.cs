// <copyright file="WorkflowStatesMappingContext.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query.Mappers
{
    /// <summary>
    /// Mapping context for the <see cref="WorkflowStatesMapper"/>.
    /// </summary>
    public class WorkflowStatesMappingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowStatesMappingContext"/> class.
        /// </summary>
        /// <param name="currentTenantId">The <see cref="CurrentTenantId"/>.</param>
        /// <param name="workflowId">The <see cref="WorkflowId"/>.</param>
        /// <param name="maxItems">The <see cref="MaxItems"/>.</param>
        /// <param name="requestContinuationToken">The <see cref="RequestContinuationToken"/>.</param>
        /// <param name="nextContinuationToken">The <see cref="NextContinuationToken"/>.</param>
        public WorkflowStatesMappingContext(
            string currentTenantId,
            string workflowId,
            int maxItems,
            string requestContinuationToken,
            string nextContinuationToken)
        {
            this.CurrentTenantId = currentTenantId;
            this.MaxItems = maxItems;
            this.RequestContinuationToken = requestContinuationToken;
            this.NextContinuationToken = nextContinuationToken;
            this.WorkflowId = workflowId;
        }

        /// <summary>
        /// Gets the Id of the requesting tenant.
        /// </summary>
        public string CurrentTenantId { get; }

        /// <summary>
        /// Gets the maximum number of items to return in the response.
        /// </summary>
        public int MaxItems { get; }

        /// <summary>
        /// Gets a continuation token returned from a previous request, used to get the current page of results.
        /// </summary>
        public string RequestContinuationToken { get; }

        /// <summary>
        /// Gets a continuation token that can be used with a subsequent request to get the next page of results.
        /// </summary>
        public string NextContinuationToken { get; }

        /// <summary>
        /// Gets the Id of the workflow to which the states being mapped belong.
        /// </summary>
        public string WorkflowId { get; }
    }
}
