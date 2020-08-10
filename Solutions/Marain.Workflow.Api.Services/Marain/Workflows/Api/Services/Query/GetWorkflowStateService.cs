// <copyright file="GetWorkflowStateService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Services.Tenancy;
    using Marain.Workflows.Api.Services.Query.Mappers;
    using Menes;
    using Menes.Hal;

    /// <summary>
    /// Implements the get workflow state endpoint for the query API.
    /// </summary>
    public class GetWorkflowStateService : IOpenApiService
    {
        /// <summary>
        /// The operation Id for the endpoint.
        /// </summary>
        public const string GetWorkflowStateOperationId = "getWorkflowState";

        private readonly IMarainServicesTenancy marainServicesTenancy;
        private readonly ITenantedWorkflowStoreFactory workflowStoreFactory;
        private readonly WorkflowStateMapper workflowStateMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkflowStateService"/> class.
        /// </summary>
        /// <param name="workflowStoreFactory">The workflow store factory.</param>
        /// <param name="marainServicesTenancy">The tenancy services.</param>
        /// <param name="workflowStateMapper">The mapper for a workflow state.</param>
        public GetWorkflowStateService(
            ITenantedWorkflowStoreFactory workflowStoreFactory,
            IMarainServicesTenancy marainServicesTenancy,
            WorkflowStateMapper workflowStateMapper)
        {
            this.marainServicesTenancy = marainServicesTenancy
                ?? throw new ArgumentNullException(nameof(marainServicesTenancy));
            this.workflowStoreFactory = workflowStoreFactory
                ?? throw new ArgumentNullException(nameof(workflowStoreFactory));
            this.workflowStateMapper = workflowStateMapper
                ?? throw new ArgumentNullException(nameof(workflowStateMapper));
        }

        /// <summary>
        /// Retrieves a specific workflow.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="workflowId">The Id of the workflow to retrieve.</param>
        /// <param name="stateId">The Id of the state to retrieve.</param>
        /// <returns>The workflow state, as an OpenApiResult.</returns>
        [OperationId(GetWorkflowStateOperationId)]
        public async Task<OpenApiResult> GetWorkflowStateAsync(
            IOpenApiContext context,
            string workflowId,
            string stateId)
        {
            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
            IWorkflowStore workflowStore = await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);
            Workflow workflow = await workflowStore.GetWorkflowAsync(workflowId).ConfigureAwait(false);
            WorkflowState state = workflow.States?.Values.FirstOrDefault(s => s.Id == stateId);

            if (state == null)
            {
                return this.NotFoundResult();
            }

            HalDocument response = await this.workflowStateMapper.MapAsync(
                state,
                new WorkflowStateMappingContext(context.CurrentTenantId, workflowId)).ConfigureAwait(false);

            return this.OkResult(response, "application/json");
        }
    }
}
