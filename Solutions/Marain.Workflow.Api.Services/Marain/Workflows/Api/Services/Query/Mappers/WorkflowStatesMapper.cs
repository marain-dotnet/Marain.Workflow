// <copyright file="WorkflowStatesMapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.Leasing.Retry.Policies;
    using Menes;
    using Menes.Exceptions;
    using Menes.Hal;
    using Menes.Links;
    using Tavis.UriTemplates;

    /// <summary>
    /// Maps a workflow to a response list of states.
    /// </summary>
    public class WorkflowStatesMapper : IHalDocumentMapper<Workflow, WorkflowStatesMappingContext>
    {
        private static readonly Regex MatchContinuationToken = new Regex("skip=([0-9]*)");

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
        }

        /// <inheritdoc/>
        public async ValueTask<HalDocument> MapAsync(Workflow resource, WorkflowStatesMappingContext context)
        {
            int skip = ParseContinuationToken(context.ContinuationToken);

            var workflowStateMappingContext = new WorkflowStateMappingContext(context.CurrentTenantId, resource.Id);
            IEnumerable<Task<HalDocument>> stateTasks = resource.States.Values
                .Skip(skip)
                .Take(context.MaxItems)
                .Select(state => this.workflowStateMapper.MapAsync(state, workflowStateMappingContext).AsTask());

            HalDocument[] stateDocuments = await Task.WhenAll(stateTasks).ConfigureAwait(false);

            int nextSkip = skip + context.MaxItems;

            string nextContinuationToken = nextSkip < resource.States.Count
                ? BuildContinuationToken(nextSkip)
                : null;

            HalDocument resultDoc = this.halDocumentFactory.CreateHalDocument();
            resultDoc.AddEmbeddedResources("items", stateDocuments);

            foreach (HalDocument state in stateDocuments)
            {
                resultDoc.AddLink("items", state.GetLinksForRelation("self").First());
            }

            resultDoc.ResolveAndAddByOperationIdAndRelationType(
                this.openApiWebLinkResolver,
                GetWorkflowStatesService.GetWorkflowStatesOperationId,
                "self",
                ("tenantId", context.CurrentTenantId),
                ("workflowId", resource.Id),
                ("maxItems", context.MaxItems),
                ("continuationToken", context.ContinuationToken));

            if (!string.IsNullOrEmpty(nextContinuationToken))
            {
                resultDoc.ResolveAndAddByOperationIdAndRelationType(
                    this.openApiWebLinkResolver,
                    GetWorkflowStatesService.GetWorkflowStatesOperationId,
                    "next",
                    ("tenantId", context.CurrentTenantId),
                    ("workflowId", resource.Id),
                    ("maxItems", context.MaxItems),
                    ("continuationToken", nextContinuationToken));
            }

            return resultDoc;
        }

        private static string BuildContinuationToken(int skip)
        {
            return $"skip={skip}".Base64UrlEncode();
        }

        private static int ParseContinuationToken(string continuationToken)
        {
            if (string.IsNullOrEmpty(continuationToken))
            {
                return 0;
            }

            string decodedContinuationToken = continuationToken.Base64UrlDecode();

            Match match = MatchContinuationToken.Match(decodedContinuationToken);
            if (!match.Success)
            {
                throw new OpenApiBadRequestException(
                    "Bad continuation token",
                    "The continuation token could not be decoded",
                    "/marain/workflow/query/errors/continuation/unable-to-decode")
                    .AddProblemDetailsExtension("continuationToken", continuationToken);
            }

            if (match.Groups.Count != 2)
            {
                throw new OpenApiBadRequestException(
                    "Bad continuation token",
                    "The continuation token is corrupted",
                    "/marain/workflow/query/errors/continuation/unable-to-match")
                    .AddProblemDetailsExtension("continuationToken", continuationToken);
            }

            if (!int.TryParse(match.Groups[1].Value, out int skip))
            {
                throw new OpenApiBadRequestException(
                    "Bad continuation token",
                    "The continuation token is corrupted",
                    "/marain/workflow/query/errors/continuation/token-data-invalid")
                    .AddProblemDetailsExtension("continuationToken", continuationToken);
            }

            return skip;
        }
    }
}
