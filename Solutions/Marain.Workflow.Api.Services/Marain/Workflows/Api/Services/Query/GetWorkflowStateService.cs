// <copyright file="GetWorkflowStateService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System.Threading.Tasks;
    using Menes;

    /// <summary>
    /// Implements the get workflow state endpoint for the query API.
    /// </summary>
    public class GetWorkflowStateService : IOpenApiService
    {
        /// <summary>
        /// The operation Id for the endpoint.
        /// </summary>
        public const string GetWorkflowStateOperationId = "getWorkflowState";

        /// <summary>
        /// Retrieves a specific workflow.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="workflowId">The Id of the workflow to retrieve.</param>
        /// <param name="stateId">The Id of the state to retrieve.</param>
        /// <returns>The workflow state, as an OpenApiResult.</returns>
        [OperationId(GetWorkflowStateOperationId)]
        public Task<OpenApiResult> GetWorkflowStateAsync(
            IOpenApiContext context,
            string workflowId,
            string stateId)
        {
            return Task.FromResult(this.NotImplementedResult());
        }
    }
}
