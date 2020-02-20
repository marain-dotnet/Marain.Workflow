// <copyright file="SqlWorkflowStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Retry;
    using Marain.Workflows;
    using Marain.Workflows.Storage.Internal;
    using Newtonsoft.Json;

    /// <summary>
    /// A CosmosDb implementation of the workflow store.
    /// </summary>
    public class SqlWorkflowStore : IWorkflowStore
    {
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly Func<Task<SqlConnection>> connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowEngine"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The serializer settings provider for the store.</param>
        /// <param name="connectionFactory">A factory method to create a sqlconnection for the workflow store.</param>
        public SqlWorkflowStore(
            IJsonSerializerSettingsProvider serializerSettingsProvider, Func<Task<SqlConnection>> connectionFactory)
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
            return Retriable.RetryAsync(() =>
                this.UpsertWorkflowCoreAsync(workflow));
        }

        private async Task<Workflow> GetWorkflowCoreAsync(string workflowId)
        {
            using SqlConnection connection = await this.connectionFactory().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            command.Parameters.Add("@workflowId", SqlDbType.NVarChar, 50).Value = workflowId;
            command.CommandText = "GetWorkflow";
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync().ConfigureAwait(false);
            using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
            {
                throw new WorkflowInstanceNotFoundException($"The workflow with id {workflowId} was not found.");
            }

            await reader.ReadAsync().ConfigureAwait(false);
            string serializedResult = reader.GetString(0);
            string etag = reader.GetString(1);
            Workflow instance = JsonConvert.DeserializeObject<Workflow>(serializedResult, this.serializerSettingsProvider.Instance);
            instance.ETag = etag;

            reader.Close();
            connection.Close();

            return instance;
        }

        private async Task UpsertWorkflowCoreAsync(Workflow workflow)
        {
            string serializedWorkflow = JsonConvert.SerializeObject(workflow, this.serializerSettingsProvider.Instance);
            string newetag = EtagHelper.BuildEtag(nameof(SqlWorkflowStore), serializedWorkflow);

            using SqlConnection connection = await this.connectionFactory().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            command.Parameters.Add("@workflowId", SqlDbType.NVarChar, 50).Value = workflow.Id;
            command.Parameters.Add("@etag", SqlDbType.NVarChar, 50).Value = workflow.ETag ?? (object)DBNull.Value;
            command.Parameters.Add("@newetag", SqlDbType.NVarChar, 50).Value = newetag;
            command.Parameters.Add("@serializedWorkflow", SqlDbType.NVarChar, -1).Value = serializedWorkflow;

            SqlParameter returnValue = command.Parameters.Add("@returnValue", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;

            command.CommandText = "UpsertWorkflow";
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync().ConfigureAwait(false);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            if ((int)returnValue.Value == 409)
            {
                throw new WorkflowInstanceConflictException($"The workflow with id {workflow.Id} was already modified.");
            }

            workflow.ETag = newetag;
        }
    }
}
