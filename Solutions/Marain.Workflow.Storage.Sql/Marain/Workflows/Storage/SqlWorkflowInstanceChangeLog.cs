// <copyright file="SqlWorkflowInstanceChangeLog.cs" company="Endjin Limited">
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
