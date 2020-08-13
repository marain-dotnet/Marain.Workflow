// <copyright file="GetWorkflowInstanceService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Services.Tenancy;
    using Marain.Workflows.Api.Services.Query.Mappers;
    using Menes;
    using Menes.Hal;

    /// <summary>
    /// Implementation of the getWorkflowInstance operation.
    /// </summary>
    public class GetWorkflowInstanceService : IOpenApiService
    {
        /// <summary>
        /// Operation Id for the getWorkflowStates operation.
        /// </summary>
        public const string GetWorkflowInstanceOperationId = "getWorkflowInstance";

        private readonly IMarainServicesTenancy marainServicesTenancy;
        private readonly ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory;
        private readonly WorkflowInstanceMapper workflowInstanceMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkflowService"/> class.
        /// </summary>
        /// <param name="workflowInstanceStoreFactory">The workflow store factory.</param>
        /// <param name="marainServicesTenancy">The tenancy services.</param>
        /// <param name="workflowInstanceMapper">The workflow mapper.</param>
        public GetWorkflowInstanceService(
            ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory,
            IMarainServicesTenancy marainServicesTenancy,
            WorkflowInstanceMapper workflowInstanceMapper)
        {
            this.marainServicesTenancy = marainServicesTenancy
                ?? throw new ArgumentNullException(nameof(marainServicesTenancy));
            this.workflowInstanceStoreFactory = workflowInstanceStoreFactory
                ?? throw new ArgumentNullException(nameof(workflowInstanceStoreFactory));
            this.workflowInstanceMapper = workflowInstanceMapper
                ?? throw new ArgumentNullException(nameof(workflowInstanceMapper));
        }

        /// <summary>
        /// Retrieves a list of states for a workflow, optionally embedding a list of workflow instances in each state.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="workflowInstanceId">The Id of the workflow instance to return.</param>
        /// <returns>An OpenApi response containing the workflow state.</returns>
        [OperationId(GetWorkflowInstanceOperationId)]
        public async Task<OpenApiResult> GetWorkflowInstanceAsync(
            IOpenApiContext context,
            string workflowInstanceId)
        {
            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
            IWorkflowInstanceStore workflowStore = await this.workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenant).ConfigureAwait(false);
            WorkflowInstance workflowInstance = await workflowStore.GetWorkflowInstanceAsync(workflowInstanceId).ConfigureAwait(false);
            HalDocument response = await this.workflowInstanceMapper.MapAsync(workflowInstance, context).ConfigureAwait(false);
            OpenApiResult result = this.OkResult(response, "application/json");

            if (!string.IsNullOrEmpty(workflowInstance.ETag))
            {
                result.Results.Add("ETag", workflowInstance.ETag);
            }

            return result;
        }
    }
}