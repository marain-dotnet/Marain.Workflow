// <copyright file="GetWorkflowInstancesService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System.Threading.Tasks;
    using Menes;

    /// <summary>
    /// Implementation of the getWorkflowInstances operation.
    /// </summary>
    public class GetWorkflowInstancesService : IOpenApiService
    {
        /// <summary>
        /// Operation Id for the getWorkflowStates operation.
        /// </summary>
        public const string GetWorkflowInstancesOperationId = "getWorkflowInstances";

        /// <summary>
        /// Retrieves a list of states for a workflow, optionally embedding a list of workflow instances in each state.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="workflowId">The Id of the workflow to retrieve states for.</param>
        /// <param name="stateId">The Id of the workflow state to return.</param>
        /// <param name="continuationToken">A continuation token from a previous request.</param>
        /// <param name="maxItems">The maximum number of items to return from the request.</param>
        /// <param name="filter">Filter params for customised queries.</param>
        /// <param name="sort">Sorting params for the customised queries.</param>
        /// <returns>An OpenApi response containing the requested data.</returns>
        [OperationId(GetWorkflowInstancesOperationId)]
        public Task<OpenApiResult> GetWorkflowInstancesAsync(
            IOpenApiContext context,
            string workflowId,
            string stateId,
            string continuationToken,
            int? maxItems,
            string filter,
            string sort)
        {
            return Task.FromResult(this.NotImplementedResult());
        }
    }
}
