// <copyright file="SqlWorkflowInstanceChangeLog.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.Extensions.Json;
    using Corvus.Retry;
    using Marain.Workflows;
    using Marain.Workflows.Storage.Internal;
    using Newtonsoft.Json;

    /// <summary>
    /// A CosmosDb implementation of the workflow instance change log.
    /// </summary>
    public class SqlWorkflowInstanceChangeLog : IWorkflowInstanceChangeLog
    {
        private readonly Func<Task<SqlConnection>> connectionFactory;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlWorkflowInstanceChangeLog"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The serializer settings provider for the store.</param>
        /// <param name="connectionFactory">A factory method to create a sqlconnection for the workflow store.</param>
        public SqlWorkflowInstanceChangeLog(IJsonSerializerSettingsProvider serializerSettingsProvider, Func<Task<SqlConnection>> connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.serializerSettingsProvider = serializerSettingsProvider;
        }

        /// <inheritdoc/>
        public Task CreateLogEntryAsync(IWorkflowTrigger trigger, WorkflowInstance workflowInstance, string partitionKey = null)
        {
            return Retriable.RetryAsync(() =>
                this.CreateLogEntryCoreAsync(trigger, workflowInstance));
        }

        /// <inheritdoc/>
        public Task<WorkflowInstanceLog> GetLogEntriesAsync(string workflowInstanceId, int? startingTimestamp = null, int maxItems = 25, string continuationToken = null)
        {
            return Retriable.RetryAsync(() =>
                this.GetLogEntriesCoreAsync(workflowInstanceId, startingTimestamp, maxItems, continuationToken));
        }

        private async Task<WorkflowInstanceLog> GetLogEntriesCoreAsync(string workflowInstanceId, int? startingTimestamp = null, int maxItems = 25, string continuationToken = null)
        {
            int pageIndex = 0;
            int pageSize = maxItems;

            if (continuationToken != null)
            {
                string serializedToken = continuationToken.FromBase64();
                ContinuationToken token = JsonConvert.DeserializeObject<ContinuationToken>(serializedToken);
                pageIndex = token.PageIndex;
                pageSize = token.PageSize;
                startingTimestamp = token.StartingTimestamp;
            }

            using SqlConnection connection = await this.connectionFactory().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "GetWorkflowInstanceLog";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@workflowInstanceId", workflowInstanceId);
            var timestamp = new SqlParameter("@startingTimestamp", SqlDbType.BigInt)
            {
                IsNullable = true,
                Value = (object)startingTimestamp ?? DBNull.Value,
            };
            command.Parameters.Add(timestamp);
            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@pageIndex", pageIndex);

            var resultSet = new List<WorkflowInstanceLogEntry>();

            await connection.OpenAsync().ConfigureAwait(false);
            using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                resultSet.Add(this.GetLogEntry(reader));
            }

            return new WorkflowInstanceLog(resultSet.Count > 0 ? JsonConvert.SerializeObject(new ContinuationToken(pageSize, pageIndex + 1, startingTimestamp)).AsBase64() : null, resultSet);
        }

        private WorkflowInstanceLogEntry GetLogEntry(SqlDataReader reader)
        {
            string serializedTrigger = reader.GetString(1);
            string serializedInstance = reader.GetString(2);
            int timestamp = reader.GetInt32(3);

            return new WorkflowInstanceLogEntry(
                JsonConvert.DeserializeObject<IWorkflowTrigger>(serializedTrigger, this.serializerSettingsProvider.Instance),
                JsonConvert.DeserializeObject<WorkflowInstance>(serializedInstance, this.serializerSettingsProvider.Instance),
                timestamp);
        }

        private async Task CreateLogEntryCoreAsync(IWorkflowTrigger trigger, WorkflowInstance workflowInstance)
        {
            string serializedInstance = JsonConvert.SerializeObject(workflowInstance, this.serializerSettingsProvider.Instance);
            string serializedTrigger = JsonConvert.SerializeObject(trigger, this.serializerSettingsProvider.Instance);

            using SqlConnection connection = await this.connectionFactory().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            string logId = Guid.NewGuid().ToString();
            command.Parameters.Add("@logId", SqlDbType.NVarChar, 50).Value = logId;
            command.Parameters.Add("@workflowInstanceId", SqlDbType.NVarChar, 50).Value = workflowInstance.Id;
            command.Parameters.Add("@serializedInstance", SqlDbType.NVarChar, -1).Value = serializedInstance;
            command.Parameters.Add("@serializedTrigger", SqlDbType.NVarChar, -1).Value = serializedTrigger;

            SqlParameter returnValue = command.Parameters.Add("@returnValue", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;

            command.CommandText = "CreateWorkflowInstanceChangeLogEntry";
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync().ConfigureAwait(false);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            if ((int)returnValue.Value == 409)
            {
                throw new WorkflowInstanceConflictException($"The workflow instance change log entry with id '{logId}' for the instance with id '{workflowInstance.Id}' was already cre3ated.");
            }
        }
    }
}
