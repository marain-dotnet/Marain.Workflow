// <copyright file="GetWorkflowService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System.Threading.Tasks;
    using Menes;

    /// <summary>
    /// Implements the get workflows endpoint for the query API.
    /// </summary>
    public class GetWorkflowService : IOpenApiService
    {
        /// <summary>
        /// The operation Id for the endpoint.
        /// </summary>
        public const string GetWorkflowOperationId = "getWorkflow";

        /// <summary>
        /// Retrieves a specific workflow.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="workflowId">The Id of the workflow to retrieve.</param>
        /// <returns>The workflow, as an OpenApiResult.</returns>
        [OperationId(GetWorkflowOperationId)]
        public Task<OpenApiResult> GetWorkflowAsync(
            IOpenApiContext context,
            string workflowId)
        {
            return Task.FromResult(this.NotImplementedResult());
        }
    }
}
