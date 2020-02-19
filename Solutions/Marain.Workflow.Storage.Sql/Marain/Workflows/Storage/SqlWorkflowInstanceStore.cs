﻿// <copyright file="SqlWorkflowInstanceStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Retry;
    using Marain.Workflows;
    using Marain.Workflows.Storage.Internal;
    using Newtonsoft.Json;

    /// <summary>
    /// A CosmosDb implementation of the workflow instance store.
    /// </summary>
    public class SqlWorkflowInstanceStore : IWorkflowInstanceStore
    {
        private readonly Func<SqlConnection> connectionFactory;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlWorkflowInstanceStore"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The serializer settings provider for the store.</param>
        /// <param name="connectionFactory">A factory method to create a sqlconnection for the workflow store.</param>
        public SqlWorkflowInstanceStore(IJsonSerializerSettingsProvider serializerSettingsProvider, Func<SqlConnection> connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.serializerSettingsProvider = serializerSettingsProvider;
        }

        /// <inheritdoc/>
        public async Task<WorkflowInstance> GetWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null)
        {
            return await Retriable.RetryAsync(() =>
                this.GetWorkflowInstanceCoreAsync(workflowInstanceId))
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task UpsertWorkflowInstanceAsync(WorkflowInstance workflowInstance, string partitionKey = null)
        {
            return Retriable.RetryAsync(() =>
                this.UpsertWorkflowInstanceCoreAsync(workflowInstance));
        }

        /// <inheritdoc/>
        public Task DeleteWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null)
        {
            return Retriable.RetryAsync(() =>
                this.DeleteWorkflowInstanceCoreAsync(workflowInstanceId));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetMatchingWorkflowInstanceIdsForSubjectsAsync(
            IEnumerable<string> subjects,
            int pageSize,
            int pageNumber)
        {
            DataTable subjectTable = BuildInterests(subjects);
            using SqlConnection connection = this.connectionFactory();

            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "GetMatchingWorkflowInstanceCountForSubjects";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@subjects", subjectTable);
            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@pageIndex", pageNumber);

            var resultSet = new List<string>(pageSize);

            await connection.OpenAsync().ConfigureAwait(false);
            SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                resultSet.Add(reader.GetString(0));
            }

            return resultSet;
        }

        /// <inheritdoc/>
        public async Task<int> GetMatchingWorkflowInstanceCountForSubjectsAsync(IEnumerable<string> subjects)
        {
            DataTable subjectTable = BuildInterests(subjects);
            using SqlConnection connection = this.connectionFactory();

            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "GetMatchingWorkflowInstanceCountForSubjects";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@subjects", subjectTable);

            await connection.OpenAsync().ConfigureAwait(false);
            object result = await command.ExecuteScalarAsync().ConfigureAwait(false);
            return (int)result;
        }

        private static DataTable BuildInterests(IEnumerable<string> interests)
        {
            var interestTable = new DataTable();
            interestTable.Columns.Add("Interest", typeof(string));

            foreach (string interest in interests)
            {
                interestTable.Rows.Add(interest);
            }

            return interestTable;
        }

        private async Task<WorkflowInstance> GetWorkflowInstanceCoreAsync(string workflowInstanceId)
        {
            using SqlConnection connection = this.connectionFactory();

            using SqlCommand command = connection.CreateCommand();
            command.Parameters.AddWithValue("@workflowInstanceId", workflowInstanceId);
            command.CommandText = "GetWorkflowInstance";
            command.CommandType = CommandType.StoredProcedure;
            await connection.OpenAsync().ConfigureAwait(false);
            SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

            if (!reader.HasRows)
            {
                throw new WorkflowInstanceNotFoundException($"The workflow instance with id {workflowInstanceId} was not found");
            }

            string serializedResult = reader.GetString(0);
            string etag = reader.GetString(1);

            reader.Close();
            connection.Close();

            WorkflowInstance instance = JsonConvert.DeserializeObject<WorkflowInstance>(serializedResult, this.serializerSettingsProvider.Instance);
            instance.ETag = etag;
            return instance;
        }

        private async Task DeleteWorkflowInstanceCoreAsync(string workflowInstanceId)
        {
            using SqlConnection connection = this.connectionFactory();

            using SqlCommand command = connection.CreateCommand();
            command.Parameters.AddWithValue("@workflowInstanceId", workflowInstanceId);

            SqlParameter returnValue = command.Parameters.Add("@returnValue", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;

            command.CommandText = "DeleteWorkflowInstance";
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync().ConfigureAwait(false);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            if ((int)returnValue.Value == 404)
            {
                throw new WorkflowInstanceNotFoundException($"The workflow instance with id {workflowInstanceId} was not found.");
            }
        }

        private async Task UpsertWorkflowInstanceCoreAsync(WorkflowInstance workflowInstance)
        {
            string serializedInstance = JsonConvert.SerializeObject(workflowInstance, this.serializerSettingsProvider.Instance);
            string newetag = EtagHelper.BuildEtag(nameof(SqlWorkflowInstanceStore), serializedInstance);
            DataTable interests = BuildInterests(workflowInstance.Interests);

            using SqlConnection connection = this.connectionFactory();

            using SqlCommand command = connection.CreateCommand();
            command.Parameters.AddWithValue("@workflowInstanceId", workflowInstance.Id);
            command.Parameters.AddWithValue("@etag", workflowInstance.ETag);
            command.Parameters.AddWithValue("@newetag", newetag);
            command.Parameters.AddWithValue("@serializedInstance", serializedInstance);
            command.Parameters.AddWithValue("@interests", interests);

            SqlParameter returnValue = command.Parameters.Add("@returnValue", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;

            command.CommandText = "UpsertWorkflowInstance";
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync().ConfigureAwait(false);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            if ((int)returnValue.Value == 409)
            {
                throw new WorkflowInstanceConflictException($"The workflow instance with id {workflowInstance.Id} was already modified.");
            }
        }
    }
}
