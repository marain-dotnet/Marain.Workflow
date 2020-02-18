// <copyright file="SqlWorkflowStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Retry;
    using Marain.Workflows;
    using Newtonsoft.Json;

    /// <summary>
    /// A CosmosDb implementation of the workflow store.
    /// </summary>
    public class SqlWorkflowStore : IWorkflowStore
    {
        private const string WorkflowTable = "Workflow";

        private const string SerializedWorkflowColumn = "SerializedWorkflow";
        private const string WorkflowETagColumn = "ETag";
        private const string WorkflowIdColumn = "WorkflowId";

        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly Func<SqlConnection> connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowEngine"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The serializer settings provider for the store.</param>
        /// <param name="connectionFactory">A factory method to create a sqlconnection for the workflow store.</param>
        public SqlWorkflowStore(
            IJsonSerializerSettingsProvider serializerSettingsProvider, Func<SqlConnection> connectionFactory)
        {
            this.serializerSettingsProvider = serializerSettingsProvider;
            this.connectionFactory = connectionFactory;
        }

        /// <inheritdoc/>
        public async Task<Workflow> GetWorkflowAsync(string workflowId, string partitionKey = null)
        {
            return await Retriable.RetryAsync(() =>
            this.GetWorkflowCoreAsync(workflowId))
            .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task UpsertWorkflowAsync(Workflow workflow, string partitionKey = null)
        {
        }

        private async Task<Workflow> GetWorkflowCoreAsync(string workflowId)
        {
            using SqlConnection connection = this.connectionFactory();

            using SqlCommand command = connection.CreateCommand();
            command.Parameters.AddWithValue(nameof(workflowId), workflowId);
            command.CommandText = $"SELECT TOP 1 [{WorkflowETagColumn}] as ETag, [{SerializedWorkflowColumn}] AS SerializedWorkflow FROM [{WorkflowTable}] WHERE [{WorkflowIdColumn}] = @{nameof(workflowId)}";
            await connection.OpenAsync().ConfigureAwait(false);
            SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
            {
                throw new WorkflowInstanceNotFoundException($"The workflow with id {workflowId} was not found.");
            }

            string serializedResult = reader.GetString(0);
            string etag = reader.GetString(1);

            reader.Close();
            connection.Close();

            Workflow instance = JsonConvert.DeserializeObject<Workflow>(serializedResult, this.serializerSettingsProvider.Instance);
            instance.ETag = etag;
            return instance;
        }

    }
}
