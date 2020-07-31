// <copyright file="GetWorkflowStatesService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System.Threading.Tasks;
    using Menes;

    /// <summary>
    /// Implements the get workflow states endpoint for the query API.
    /// </summary>
    public class GetWorkflowStatesService : IOpenApiService
    {
        /// <summary>
        /// The operation Id for the endpoint.
        /// </summary>
        public const string GetWorkflowStatesOperationId = "getWorkflowStates";

        /// <summary>
        /// Retrieves a specific workflow.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="workflowId">The Id of the workflow to retrieve.</param>
        /// <param name="maxItems">The maximum number of items to return.</param>
        /// <param name="continuationToken">A continuation token from a previous request.</param>
        /// <returns>The workflow states, as an OpenApiResult.</returns>
        [OperationId(GetWorkflowStatesOperationId)]
        public Task<OpenApiResult> GetWorkflowStatesAsync(
            IOpenApiContext context,
            string workflowId,
            int? maxItems,
            string continuationToken)
        {
            return Task.FromResult(this.NotImplementedResult());
        }
    }
}
