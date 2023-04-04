// <copyright file="SqlWorkflowStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Retry;
    using Corvus.Retry.Policies;
    using Corvus.Retry.Strategies;
    using Corvus.Storage;
    using Marain.Workflows;
    using Marain.Workflows.Storage.Internal;
    using Microsoft.Data.SqlClient;
    using Newtonsoft.Json;

    /// <summary>
    /// A CosmosDb implementation of the workflow store.
    /// </summary>
    public class SqlWorkflowStore : IWorkflowStore
    {
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly Func<ValueTask<SqlConnection>> connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowEngine"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The serializer settings provider for the store.</param>
        /// <param name="connectionFactory">A factory method to create a sqlconnection for the workflow store.</param>
        public SqlWorkflowStore(
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            Func<ValueTask<SqlConnection>> connectionFactory)
        {
            this.serializerSettingsProvider = serializerSettingsProvider;
            this.connectionFactory = connectionFactory;
        }

        /// <inheritdoc/>
        public async Task<EntityWithETag<Workflow>> GetWorkflowAsync(string workflowId, string partitionKey, string eTagExpected)
        {
            return await Retriable.RetryAsync(
                () => this.GetWorkflowCoreAsync(workflowId),
                CancellationToken.None,
                new Backoff(5, TimeSpan.FromSeconds(1)),
                DoNotRetryWhenFutile.Instance)
            .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<string> UpsertWorkflowAsync(Workflow workflow, string partitionKey, string eTagExpected)
        {
            return await Retriable.RetryAsync(
                () => this.UpsertWorkflowCoreAsync(workflow, eTagExpected),
                CancellationToken.None,
                new Backoff(5, TimeSpan.FromSeconds(1)),
                DoNotRetryWhenFutile.Instance)
            .ConfigureAwait(false);
        }

        private async Task<EntityWithETag<Workflow>> GetWorkflowCoreAsync(string workflowId)
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
                throw new WorkflowNotFoundException($"The workflow with id {workflowId} was not found.");
            }

            await reader.ReadAsync().ConfigureAwait(false);
            string serializedResult = reader.GetString(0);
            string returnedETag = reader.GetString(1);
            Workflow instance = JsonConvert.DeserializeObject<Workflow>(serializedResult, this.serializerSettingsProvider.Instance);

            return new EntityWithETag<Workflow>(instance, returnedETag);
        }

        private async Task<string> UpsertWorkflowCoreAsync(Workflow workflow, string eTagExpected)
        {
            string serializedWorkflow = JsonConvert.SerializeObject(workflow, this.serializerSettingsProvider.Instance);
            string newetag = EtagHelper.BuildEtag(nameof(SqlWorkflowStore), serializedWorkflow);

            using SqlConnection connection = await this.connectionFactory().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            command.Parameters.Add("@workflowId", SqlDbType.NVarChar, 50).Value = workflow.Id;
            command.Parameters.Add("@etag", SqlDbType.NVarChar, 50).Value = eTagExpected ?? (object)DBNull.Value;
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
                throw new WorkflowConflictException($"A workflow with id {workflow.Id} already exists.");
            }

            if ((int)returnValue.Value == 412)
            {
                throw new WorkflowPreconditionFailedException($"The workflow with id {workflow.Id} was already modified.");
            }

            return newetag;
        }

        private sealed class DoNotRetryWhenFutile : IRetryPolicy
        {
            public static readonly DoNotRetryWhenFutile Instance = new();

            private DoNotRetryWhenFutile()
            {
            }

            public bool CanRetry(Exception exception) => exception switch
            {
                WorkflowNotFoundException => false,
                _ => true,
            };
        }
    }
}