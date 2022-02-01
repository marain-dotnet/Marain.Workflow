// <copyright file="CosmosWorkflowInstanceStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.Retry;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// A CosmosDb implementation of the workflow instance store.
    /// </summary>
    public class CosmosWorkflowInstanceStore : IWorkflowInstanceStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosWorkflowInstanceStore"/> class.
        /// </summary>
        /// <param name="workflowInstanceContainer">The repository in which to store workflow instances.</param>
        public CosmosWorkflowInstanceStore(
            Container workflowInstanceContainer)
        {
            this.Container = workflowInstanceContainer;
        }

        /// <summary>
        /// Gets the underlying Cosmos <see cref="Container"/> for this workflow instance store.
        /// </summary>
        public Container Container { get; }

        /// <inheritdoc/>
        public async Task<WorkflowInstance> GetWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null)
        {
            try
            {
                return await Retriable.RetryAsync(() =>
                    this.Container.ReadItemAsync<WorkflowInstance>(
                        workflowInstanceId,
                        new PartitionKey(partitionKey ?? workflowInstanceId)))
                    .ConfigureAwait(false);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new WorkflowInstanceNotFoundException(
                    $"The workflow instance with id {workflowInstanceId} was not found",
                    ex);
            }
        }

        /// <inheritdoc/>
        public async Task UpsertWorkflowInstanceAsync(WorkflowInstance workflowInstance, string partitionKey = null)
        {
            try
            {
                await Retriable.RetryAsync(() =>
                    this.Container.UpsertItemAsync(
                        workflowInstance,
                        new PartitionKey(partitionKey ?? workflowInstance.Id),
                        new ItemRequestOptions { IfMatchEtag = workflowInstance.ETag }))
                    .ConfigureAwait(false);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new WorkflowInstanceConflictException($"The workflow instance with id {workflowInstance.Id} was already modified.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null)
        {
            try
            {
                await Retriable.RetryAsync(() =>
                    this.Container.DeleteItemAsync<WorkflowInstance>(
                        workflowInstanceId,
                        new PartitionKey(partitionKey ?? workflowInstanceId)))
                    .ConfigureAwait(false);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new WorkflowInstanceNotFoundException($"The workflow instance with id {workflowInstanceId} was not found", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetMatchingWorkflowInstanceIdsForSubjectsAsync(
            IEnumerable<string> subjects,
            int pageSize,
            int pageNumber)
        {
            QueryDefinition spec = BuildFindInstanceIdsSpec(subjects, pageSize, pageNumber);

            FeedIterator<dynamic> iterator = this.Container.GetItemQueryIterator<dynamic>(spec);

            var matchingIds = new List<string>();

            while (iterator.HasMoreResults)
            {
                FeedResponse<dynamic> results = await Retriable.RetryAsync(() => iterator.ReadNextAsync()).ConfigureAwait(false);
                matchingIds.AddRange(results.Select(x => (string)x.id));
            }

            return matchingIds;
        }

        /// <inheritdoc/>
        public async Task<int> GetMatchingWorkflowInstanceCountForSubjectsAsync(IEnumerable<string> subjects)
        {
            QueryDefinition spec = BuildFindInstanceIdsSpec(subjects, 1, 0, true);

            FeedIterator<int> iterator = this.Container.GetItemQueryIterator<int>(spec, null, new QueryRequestOptions { MaxItemCount = 1 });

            // There will always be a result so we don't need to check...
            FeedResponse<int> result = await Retriable.RetryAsync(() => iterator.ReadNextAsync()).ConfigureAwait(false);
            return result.First();
        }

        private static QueryDefinition BuildFindInstanceIdsSpec(IEnumerable<string> subjects, int pageSize, int pageNumber, bool countOnly = false)
        {
            string[] subjectsArray = subjects?.ToArray();

            string query = countOnly ? "SELECT VALUE COUNT(root.id) FROM root" : "SELECT root.id FROM root";
            string offsetLimitClause = $" OFFSET {pageSize * pageNumber} LIMIT {pageSize}";
            if (subjectsArray?.Length > 0)
            {
                (string where, List<(string, string)> parameters) = GetSubjectClause(subjectsArray);
                var result = new QueryDefinition($"{query} WHERE {where}" + offsetLimitClause);
                parameters.ForEach(x => result.WithParameter(x.Item1, x.Item2));

                return result;
            }

            return new QueryDefinition(query + offsetLimitClause);
        }

        /// <summary>
        /// Builds the subject clause to be used when subjects are supplied to <see cref="GetMatchingWorkflowInstanceIdsForSubjectsAsync" />.
        /// </summary>
        /// <param name="subjects">
        /// The list of subjects/.
        /// </param>
        /// <returns>
        /// A <see cref="string" /> containing the WHERE clause and a List containing
        /// the parameters it should be supplied with.
        /// </returns>
        private static (string WhereClause, List<(string Name, string Value)> ParameterList) GetSubjectClause(IEnumerable<string> subjects)
        {
            var result = new StringBuilder();
            var parameters = new List<(string, string)>();

            subjects.ForEachAtIndex(
                (s, i) =>
                {
                    if (i > 0)
                    {
                        result.Append(" OR ");
                    }

                    result.Append("ARRAY_CONTAINS(root.interests, @subject").Append(i).Append(")");
                    parameters.Add(($"@subject{i}", s));
                });

            return (result.ToString(), parameters);
        }
    }
}