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
        /// <param name="maxItems">The <see cref="MaxItems"/>.</param>
        /// <param name="continuationToken">The <see cref="ContinuationToken"/>.</param>
        public WorkflowStatesMappingContext(
            string currentTenantId,
            int maxItems,
            string continuationToken)
        {
            this.CurrentTenantId = currentTenantId;
            this.MaxItems = maxItems;
            this.ContinuationToken = continuationToken;
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
        /// Gets a continuation token returned from a previous request, used to get the next page of results.
        /// </summary>
        public string ContinuationToken { get; }
    }
}
