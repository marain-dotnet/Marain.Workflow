// <copyright file="ParameterDataWithTelemetryExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.MessagePreProcessingHost.Shared
{
    using System.Globalization;

    using Marain.Serialization.Json;
    using Marain.Telemetry;
    using Marain.Workflows;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    /// <summary>
    /// The parameter data with telemetry extensions.
    /// </summary>
    public static class ParameterDataWithTelemetryExtensions
    {
        /// <summary>
        /// The initialize telemetry operation context.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="log">
        /// The logger.
        /// </param>
        public static void InitializeTelemetryOperationContext(
            this ParameterDataWithTelemetry data,
            ExecutionContext context,
            ILogger log)
        {
            log.LogDebug($"Initializing telemetry context");

            TelemetryOperationContext.Initialize(
                context.InvocationId.ToString(),
                context.FunctionName,
                "MessagePreProcessingHost",
                null,
                data.AdditionalTelemetryProperties,
                data.ParentOperationId);
        }

        /// <summary>
        /// The set workflow message envelope.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="envelope">
        /// The envelope.
        /// </param>
        public static void SetWorkflowMessageEnvelope(this ParameterDataWithTelemetry data, WorkflowMessageEnvelope envelope)
        {
            data.Payload["WorkflowMessageEnvelopeJson"] = JsonConvert.SerializeObject(envelope, SerializerSettings.CreateSerializationSettings());
        }

        /// <summary>
        /// The set workflow message envelope json.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="envelopeJson">
        /// The envelope json.
        /// </param>
        public static void SetWorkflowMessageEnvelopeJson(this ParameterDataWithTelemetry data, string envelopeJson)
        {
            data.Payload["WorkflowMessageEnvelopeJson"] = envelopeJson;
        }

        /// <summary>
        /// The get workflow message envelope.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="WorkflowMessageEnvelope"/>.
        /// </returns>
        public static WorkflowMessageEnvelope GetWorkflowMessageEnvelope(this ParameterDataWithTelemetry data)
        {
            string json = data.Payload["WorkflowMessageEnvelopeJson"];
            return JsonConvert.DeserializeObject<WorkflowMessageEnvelope>(json, SerializerSettings.CreateSerializationSettings());
        }

        /// <summary>
        /// The get workflow instance id.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetWorkflowInstancesPageNumber(this ParameterDataWithTelemetry data)
        {
            return int.Parse(data.Payload["GetWorkflowInstancesPageNumber"], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The set workflow instances page number.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="pageNumber">
        /// The page number.
        /// </param>
        public static void SetWorkflowInstancesPageNumber(this ParameterDataWithTelemetry data, int pageNumber)
        {
            data.Payload["GetWorkflowInstancesPageNumber"] = pageNumber.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The get workflow instance id.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetWorkflowInstanceId(this ParameterDataWithTelemetry data)
        {
            return data.Payload["WorkflowInstanceId"];
        }

        /// <summary>
        /// The set workflow instance id.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="workflowInstanceId">
        /// The workflow instance id.
        /// </param>
        public static void SetWorkflowInstanceId(this ParameterDataWithTelemetry data, string workflowInstanceId)
        {
            data.Payload["WorkflowInstanceId"] = workflowInstanceId;
        }
    }
}