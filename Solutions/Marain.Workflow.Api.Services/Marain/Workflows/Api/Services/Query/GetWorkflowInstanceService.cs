// <copyright file="GetWorkflowInstanceService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System.Threading.Tasks;
    using Menes;

    /// <summary>
    /// Implementation of the getWorkflowInstance operation.
    /// </summary>
    public class GetWorkflowInstanceService : IOpenApiService
    {
        /// <summary>
        /// Operation Id for the getWorkflowStates operation.
        /// </summary>
        public const string GetWorkflowInstanceOperationId = "getWorkflowInstance";

        /// <summary>
        /// Retrieves a list of states for a workflow, optionally embedding a list of workflow instances in each state.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="workflowInstanceId">The Id of the workflow instance to return.</param>
        /// <returns>An OpenApi response containing the workflow state.</returns>
        [OperationId(GetWorkflowInstanceOperationId)]
        public Task<OpenApiResult> GetWorkflowInstanceAsync(
            IOpenApiContext context,
            string workflowInstanceId)
        {
            return Task.FromResult(this.NotImplementedResult());
        }
    }
}