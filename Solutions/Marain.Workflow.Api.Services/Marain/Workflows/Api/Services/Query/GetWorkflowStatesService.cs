// <copyright file="GetWorkflowStatesService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.Tenancy;
    using Marain.Services.Tenancy;
    using Marain.Workflows.Api.Services.Query.Mappers;
    using Menes;
    using Menes.Exceptions;
    using Menes.Hal;

    /// <summary>
    /// Implements the get workflow states endpoint for the query API.
    /// </summary>
    public class GetWorkflowStatesService : IOpenApiService
    {
        /// <summary>
        /// The operation Id for the endpoint.
        /// </summary>
        public const string GetWorkflowStatesOperationId = "getWorkflowStates";

        private static readonly Regex MatchContinuationToken = new Regex("skip=([0-9]*)");

        private readonly IMarainServicesTenancy marainServicesTenancy;
        private readonly ITenantedWorkflowStoreFactory workflowStoreFactory;
        private readonly WorkflowStatesMapper workflowStatesMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkflowService"/> class.
        /// </summary>
        /// <param name="workflowStoreFactory">The workflow store factory.</param>
        /// <param name="marainServicesTenancy">The tenancy services.</param>
        /// <param name="workflowStatesMapper">The mapper for workflow states.</param>
        public GetWorkflowStatesService(
            ITenantedWorkflowStoreFactory workflowStoreFactory,
            IMarainServicesTenancy marainServicesTenancy,
            WorkflowStatesMapper workflowStatesMapper)
        {
            this.marainServicesTenancy = marainServicesTenancy
                ?? throw new ArgumentNullException(nameof(marainServicesTenancy));
            this.workflowStoreFactory = workflowStoreFactory
                ?? throw new ArgumentNullException(nameof(workflowStoreFactory));
            this.workflowStatesMapper = workflowStatesMapper
                ?? throw new ArgumentNullException(nameof(workflowStatesMapper));
        }

        /// <summary>
        /// Retrieves a specific workflow.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="workflowId">The Id of the workflow to retrieve.</param>
        /// <param name="maxItems">The maximum number of items to return.</param>
        /// <param name="continuationToken">A continuation token from a previous request.</param>
        /// <returns>The workflow states, as an OpenApiResult.</returns>
        [OperationId(GetWorkflowStatesOperationId)]
        public async Task<OpenApiResult> GetWorkflowStatesAsync(
            IOpenApiContext context,
            string workflowId,
            int? maxItems,
            string continuationToken)
        {
            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);

            int skip = ParseContinuationToken(continuationToken);
            int actualMaxItems = maxItems ?? 20;

            IWorkflowStore workflowStore = await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);
            Workflow workflow = await workflowStore.GetWorkflowAsync(workflowId).ConfigureAwait(false);
            WorkflowState[] states = workflow.States.Values
                .Skip(skip)
                .Take(actualMaxItems)
                .ToArray();

            int nextSkip = skip + actualMaxItems;

            string nextContinuationToken = nextSkip < workflow.States.Count
                ? BuildContinuationToken(nextSkip)
                : null;

            var mappingContext = new WorkflowStatesMappingContext(
                context.CurrentTenantId,
                workflowId,
                actualMaxItems,
                continuationToken,
                nextContinuationToken);

            HalDocument result = await this.workflowStatesMapper.MapAsync(
                states,
                mappingContext).ConfigureAwait(false);

            return this.OkResult(result);
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
