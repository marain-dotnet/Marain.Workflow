// <copyright file="ParameterDataWithTelemetry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.MessagePreProcessingHost.Shared
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Marain.Tenancy;
    using Microsoft.Azure.EventHubs;

    /// <summary>
    ///     A simple DTO to enable passing parameters and telemetry between functions.
    /// </summary>
    public class ParameterDataWithTelemetry
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterDataWithTelemetry" /> class.
        /// </summary>
        public ParameterDataWithTelemetry()
        {
            this.AdditionalTelemetryProperties = new Dictionary<string, string>();
            this.Payload = new Dictionary<string, string>();
        }

        /// <summary>
        ///     Gets or sets the additional telemetry properties.
        /// </summary>
        public Dictionary<string, string> AdditionalTelemetryProperties { get; set; }

        /// <summary>
        ///     Gets or sets the component name.
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        ///     Gets or sets the parent operation id.
        /// </summary>
        public string ParentOperationId { get; set; }

        /// <summary>
        /// Gets or sets the tenant associated with the request.
        /// </summary>
        public Tenant Tenant { get; set; } = Tenant.Root;

        /// <summary>
        ///     Gets or sets the payload.
        /// </summary>
        public Dictionary<string, string> Payload { get; set; }

        /// <summary>
        ///     The from event data.
        /// </summary>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        ///     The <see cref="ParameterDataWithTelemetry" />.
        /// </returns>
        public static ParameterDataWithTelemetry FromEventData(EventData input)
        {
            string messageBody = Encoding.UTF8.GetString(input.Body.Array);

            string operationId = input.Properties.ContainsKey("AppInsightsOperationId")
                                  ? (string)input.Properties["AppInsightsOperationId"]
                                  : null;

            var additionalTelemetryProperties = input.Properties.Where(x => x.Key != "AppInsightsOperationId")
                .ToDictionary(x => x.Key, x => x.Value?.ToString());

            var result = new ParameterDataWithTelemetry
            {
                ComponentName = "MessagePreProcessingHost",
                ParentOperationId = operationId,
                AdditionalTelemetryProperties = additionalTelemetryProperties,
            };

            result.SetWorkflowMessageEnvelopeJson(messageBody);

            return result;
        }
    }
}