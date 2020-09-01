// <copyright file="GetWorkflowsService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System.Threading.Tasks;
    using Menes;

    /// <summary>
    /// Implements the get workflows endpoint for the query API.
    /// </summary>
    public class GetWorkflowsService : IOpenApiService
    {
        /// <summary>
        /// The operation Id for the endpoint.
        /// </summary>
        public const string GetWorkflowsOperationId = "getWorkflows";

        /// <summary>
        /// Retrieves workflows.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="maxItems">The maximum number of items to return.</param>
        /// <param name="continuationToken">A continuation token from a previous request.</param>
        /// <returns>The workflows, as an OpenApiResult.</returns>
        [OperationId(GetWorkflowsOperationId)]
        public Task<OpenApiResult> GetWorkflowsAsync(
            IOpenApiContext context,
            int? maxItems,
            string continuationToken)
        {
            return Task.FromResult(this.NotImplementedResult());
        }
    }
}
