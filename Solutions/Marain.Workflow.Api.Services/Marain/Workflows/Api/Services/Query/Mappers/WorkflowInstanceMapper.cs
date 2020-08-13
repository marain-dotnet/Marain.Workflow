// <copyright file="WorkflowInstanceMapper.cs" company="Endjin Limited">
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
    /// Maps Workflow Instances to HAL documents.
    /// </summary>
    public class WorkflowInstanceMapper : IHalDocumentMapper<WorkflowInstance, IOpenApiContext>
    {
        private readonly IHalDocumentFactory halDocumentFactory;
        private readonly IOpenApiWebLinkResolver openApiWebLinkResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceMapper"/> class.
        /// </summary>
        /// <param name="halDocumentFactory">The service provider to construct <see cref="HalDocument"/> instances.</param>
        /// <param name="openApiWebLinkResolver">The link resolver.</param>
        public WorkflowInstanceMapper(
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
            links.MapByContentTypeAndRelationTypeAndOperationId<WorkflowInstance>(
                "self",
                GetWorkflowInstanceService.GetWorkflowInstanceOperationId);
        }

        /// <inheritdoc/>
        public ValueTask<HalDocument> MapAsync(WorkflowInstance resource, IOpenApiContext context)
        {
            HalDocument resultDoc = this.halDocumentFactory.CreateHalDocumentFrom(new
            {
                resource.ContentType,
                resource.Id,
                resource.WorkflowId,
                resource.StateId,
                resource.Status,
                resource.Interests,
                resource.Context,
            });

            resultDoc.ResolveAndAddByOwnerAndRelationType(
                this.openApiWebLinkResolver,
                resource,
                "self",
                ("tenantId", context.CurrentTenantId),
                ("workflowId", resource.WorkflowId),
                ("workflowInstanceId", resource.Id));

            return new ValueTask<HalDocument>(resultDoc);
        }
    }
}
