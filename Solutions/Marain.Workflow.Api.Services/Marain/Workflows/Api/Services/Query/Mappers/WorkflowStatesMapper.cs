// <copyright file="WorkflowStatesMapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Menes;
    using Menes.Hal;
    using Menes.Links;

    /// <summary>
    /// Maps a workflow to a response list of states.
    /// </summary>
    public class WorkflowStatesMapper : IHalDocumentMapper<WorkflowState[], WorkflowStatesMappingContext>
    {
        private readonly IHalDocumentFactory halDocumentFactory;
        private readonly IOpenApiWebLinkResolver openApiWebLinkResolver;
        private readonly WorkflowStateMapper workflowStateMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowMapper"/> class.
        /// </summary>
        /// <param name="halDocumentFactory">The service provider to construct <see cref="HalDocument"/> instances.</param>
        /// <param name="openApiWebLinkResolver">The link resolver.</param>
        /// <param name="workflowStateMapper">The mapper for <see cref="WorkflowState"/>s.</param>
        public WorkflowStatesMapper(
            IHalDocumentFactory halDocumentFactory,
            IOpenApiWebLinkResolver openApiWebLinkResolver,
            WorkflowStateMapper workflowStateMapper)
        {
            this.halDocumentFactory = halDocumentFactory
                ?? throw new ArgumentNullException(nameof(halDocumentFactory));

            this.openApiWebLinkResolver = openApiWebLinkResolver
                ?? throw new ArgumentNullException(nameof(openApiWebLinkResolver));

            this.workflowStateMapper = workflowStateMapper
                ?? throw new ArgumentNullException(nameof(workflowStateMapper));
        }

        /// <inheritdoc/>
        public void ConfigureLinkMap(IOpenApiLinkOperationMap links)
        {
            links.MapByContentTypeAndRelationTypeAndOperationId<WorkflowState[]>(
                "self",
                GetWorkflowStatesService.GetWorkflowStatesOperationId);

            links.MapByContentTypeAndRelationTypeAndOperationId<WorkflowState[]>(
                "next",
                GetWorkflowStatesService.GetWorkflowStatesOperationId);
        }

        /// <inheritdoc/>
        public async ValueTask<HalDocument> MapAsync(WorkflowState[] resource, WorkflowStatesMappingContext context)
        {
            var workflowStateMappingContext = new WorkflowStateMappingContext(context.CurrentTenantId, context.WorkflowId);
            IEnumerable<Task<HalDocument>> stateTasks = resource.Select(state => this.workflowStateMapper.MapAsync(state, workflowStateMappingContext).AsTask());

            HalDocument[] stateDocuments = await Task.WhenAll(stateTasks).ConfigureAwait(false);

            HalDocument resultDoc = this.halDocumentFactory.CreateHalDocument();

            resultDoc.AddEmbeddedResources("items", stateDocuments);

            foreach (HalDocument state in stateDocuments)
            {
                resultDoc.AddLink("items", state.GetLinksForRelation("self").First());
            }

            resultDoc.ResolveAndAddByOwnerAndRelationType(
                this.openApiWebLinkResolver,
                resource,
                "self",
                ("tenantId", context.CurrentTenantId),
                ("workflowId", context.WorkflowId),
                ("maxItems", context.MaxItems),
                ("continuationToken", context.RequestContinuationToken));

            if (!string.IsNullOrEmpty(context.NextContinuationToken))
            {
                resultDoc.ResolveAndAddByOwnerAndRelationType(
                    this.openApiWebLinkResolver,
                    resource,
                    "next",
                    ("tenantId", context.CurrentTenantId),
                    ("workflowId", context.WorkflowId),
                    ("maxItems", context.MaxItems),
                    ("continuationToken", context.NextContinuationToken));
            }

            return resultDoc;
        }
    }
}
