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
        public Task<WorkflowInstanceLog> GetLogEntriesAsync(string workflowInstanceId, ulong? startingSequenceNumber = null, int maxItems = 25, string continuationToken = null)
        {
            return Retriable.RetryAsync(() =>
                this.GetLogEntriesCoreAsync(workflowInstanceId, startingSequenceNumber, maxItems, continuationToken));
        }

        private async Task<WorkflowInstanceLog> GetLogEntriesCoreAsync(string workflowInstanceId, ulong? startingSequenceNumber, int maxItems = 25, string continuationToken = null)
        {
            int pageIndex = 0;
            int pageSize = maxItems;

            if (continuationToken != null)
            {
                string serializedToken = continuationToken.FromBase64();
                ContinuationToken token = JsonConvert.DeserializeObject<ContinuationToken>(serializedToken);
                pageIndex = token.PageIndex;
                pageSize = token.PageSize;
                startingSequenceNumber = token.StartingSequenceNumber;
            }

            using SqlConnection connection = await this.connectionFactory().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "GetWorkflowInstanceLog";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@startingSequenceNumber", this.ToSqlBinary(startingSequenceNumber));
            command.Parameters.AddWithValue("@workflowInstanceId", workflowInstanceId);
            command.Parameters.AddWithValue("@pageSize", pageSize);
            command.Parameters.AddWithValue("@pageIndex", pageIndex);

            var resultSet = new List<WorkflowInstanceLogEntry>();

            await connection.OpenAsync().ConfigureAwait(false);
            using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                resultSet.Add(this.GetLogEntry(reader));
            }

            var nextToken = new ContinuationToken(pageSize, pageIndex + 1, startingSequenceNumber);
            return new WorkflowInstanceLog(JsonConvert.SerializeObject(nextToken).AsBase64(), resultSet);
        }

        private SqlBinary ToSqlBinary(ulong? startingSequenceNumber)
        {
            if (startingSequenceNumber is null)
            {
                return SqlBinary.Null;
            }

            // Need to ensure we convert BigEndian regardless of the server platform.
            return new SqlBinary(
                BitConverter.IsLittleEndian ?
                    BitConverter.GetBytes(startingSequenceNumber.Value).Reverse().ToArray() :
                    BitConverter.GetBytes(startingSequenceNumber.Value));
        }

        private ulong FromSqlBinary(SqlBinary sequenceNumberBinary)
        {
            return
               BitConverter.IsLittleEndian ?
                   BitConverter.ToUInt64(sequenceNumberBinary.Value.Reverse().ToArray()) :
                   BitConverter.ToUInt64(sequenceNumberBinary.Value);
        }

        private WorkflowInstanceLogEntry GetLogEntry(SqlDataReader reader)
        {
            string serializedTrigger = reader.GetString(1);
            string serializedInstance = reader.GetString(2);
            SqlBinary sequenceNumberBinary = reader.GetSqlBinary(3);

            ulong sequenceNumber = this.FromSqlBinary(sequenceNumberBinary);

            return new WorkflowInstanceLogEntry(
                JsonConvert.DeserializeObject<IWorkflowTrigger>(serializedTrigger, this.serializerSettingsProvider.Instance),
                JsonConvert.DeserializeObject<WorkflowInstance>(serializedInstance, this.serializerSettingsProvider.Instance),
                sequenceNumber);
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
