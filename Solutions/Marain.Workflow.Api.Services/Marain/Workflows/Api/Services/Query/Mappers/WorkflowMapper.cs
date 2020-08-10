// <copyright file="WorkflowMapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query.Mappers
{
    using System;
    using System.Threading.Tasks;
    using Marain.Workflows;
    using Menes;
    using Menes.Hal;
    using Menes.Links;

    /// <summary>
    /// Maps workflow definitions to response HAL documents.
    /// </summary>
    public class WorkflowMapper : IHalDocumentMapper<Workflow, IOpenApiContext>
    {
        private readonly IHalDocumentFactory halDocumentFactory;
        private readonly IOpenApiWebLinkResolver openApiWebLinkResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowMapper"/> class.
        /// </summary>
        /// <param name="halDocumentFactory">The service provider to construct <see cref="HalDocument"/> instances.</param>
        /// <param name="openApiWebLinkResolver">The link resolver.</param>
        public WorkflowMapper(
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
            links.MapByContentTypeAndRelationTypeAndOperationId<Workflow>(
                "self",
                GetWorkflowService.GetWorkflowOperationId);

            links.MapByContentTypeAndRelationTypeAndOperationId<Workflow>(
                "states",
                GetWorkflowStatesService.GetWorkflowStatesOperationId);
        }

        /// <inheritdoc/>
        public ValueTask<HalDocument> MapAsync(Workflow resource, IOpenApiContext context)
        {
            HalDocument resultDoc = this.halDocumentFactory.CreateHalDocumentFrom(new
            {
                resource.Id,
                resource.DisplayName,
                resource.Description,
                resource.InitialStateId,
                resource.ContentType,
            });

            resultDoc.ResolveAndAddByOwnerAndRelationType(
                this.openApiWebLinkResolver,
                resource,
                "self",
                ("tenantId", context.CurrentTenantId),
                ("workflowId", resource.Id));

            resultDoc.ResolveAndAddByOwnerAndRelationType(
                this.openApiWebLinkResolver,
                resource,
                "states",
                ("tenantId", context.CurrentTenantId),
                ("workflowId", resource.Id));

            return new ValueTask<HalDocument>(resultDoc);
        }
    }
}
