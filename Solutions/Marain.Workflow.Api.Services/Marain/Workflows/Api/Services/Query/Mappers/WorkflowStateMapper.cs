// <copyright file="WorkflowStateMapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query.Mappers
{
    using System;
    using System.Threading.Tasks;
    using Menes;
    using Menes.Hal;
    using Menes.Links;

    /// <summary>
    /// Maps <see cref="WorkflowState"/> instances to their HAL representation.
    /// </summary>
    public class WorkflowStateMapper : IHalDocumentMapper<WorkflowState, WorkflowStateMappingContext>
    {
        private readonly IHalDocumentFactory halDocumentFactory;
        private readonly IOpenApiWebLinkResolver openApiWebLinkResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowMapper"/> class.
        /// </summary>
        /// <param name="halDocumentFactory">The service provider to construct <see cref="HalDocument"/> instances.</param>
        /// <param name="openApiWebLinkResolver">The link resolver.</param>
        public WorkflowStateMapper(
            IHalDocumentFactory halDocumentFactory,
            IOpenApiWebLinkResolver openApiWebLinkResolver)
        {
            this.halDocumentFactory = halDocumentFactory
                ?? throw new ArgumentNullException(nameof(halDocumentFactory));

            this.openApiWebLinkResolver = openApiWebLinkResolver
                ?? throw new ArgumentNullException(nameof(openApiWebLinkResolver));
        }

        /// <inheritdoc/>
        public void ConfigureLinkMap(IOpenApiLinkOperationMap links)
        {
            links.MapByContentTypeAndRelationTypeAndOperationId<WorkflowState>(
                "self",
                GetWorkflowStateService.GetWorkflowStateOperationId);

            links.MapByContentTypeAndRelationTypeAndOperationId<WorkflowState>(
                "instances",
                GetWorkflowInstancesService.GetWorkflowInstancesOperationId);
        }

        /// <inheritdoc/>
        public ValueTask<HalDocument> MapAsync(WorkflowState resource, WorkflowStateMappingContext context)
        {
            HalDocument resultDoc = this.halDocumentFactory.CreateHalDocumentFrom(new
            {
                resource.Id,
                resource.ContentType,
                resource.DisplayName,
                resource.Description,
            });

            resultDoc.ResolveAndAddByOwnerAndRelationType(
                this.openApiWebLinkResolver,
                resource,
                "self",
                ("tenantId", context.CurrentTenantId),
                ("workflowId", context.WorkflowId),
                ("stateId", resource.Id));

            resultDoc.ResolveAndAddByOwnerAndRelationType(
                this.openApiWebLinkResolver,
                resource,
                "instances",
                ("tenantId", context.CurrentTenantId),
                ("workflowId", context.WorkflowId),
                ("stateId", resource.Id));

            return new ValueTask<HalDocument>(resultDoc);
        }
    }
}
