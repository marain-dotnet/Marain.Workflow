// <copyright file="CreateWorkflowActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.MessagePreProcessingHost.Activities
{
    using System;
    using System.Threading.Tasks;

    using Marain.Composition;
    using Marain.Workflow.Functions.EngineHost;
    using Marain.Workflow.Functions.EngineHost.Models;
    using Marain.Workflow.Functions.MessagePreProcessingHost.Shared;
    using Marain.Workflows;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The create workflow activity.
    /// </summary>
    public static class CreateWorkflowActivity
    {
        /// <summary>
        ///     The run action.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="executionContext">
        ///     The execution Context.
        /// </param>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [FunctionName(nameof(CreateWorkflowActivity))]
        public static async Task RunAction(
            [ActivityTrigger] DurableActivityContext context,
            ExecutionContext executionContext,
            ILogger logger)
        {
            Initialization.Initialize(executionContext);

            ParameterDataWithTelemetry data = context.GetInput<ParameterDataWithTelemetry>();
            data.InitializeTelemetryOperationContext(executionContext, logger);

            TelemetryClient telemetryClient = ServiceRoot.ServiceProvider.GetService<TelemetryClient>();

            using (telemetryClient.StartOperation<RequestTelemetry>(executionContext.GetFunctionNameForBespokeStartOperation()))
            {
                WorkflowMessageEnvelope envelope = data.GetWorkflowMessageEnvelope();

                IConfigurationRoot configuration = ServiceRoot.ServiceProvider.GetService<IConfigurationRoot>();

                logger.LogInformation(
                    $"Making function call for StartWorkflowInstanceRequest {envelope.StartWorkflowInstanceRequest.RequestId}");

                IEndjinWorkflowEngine client = ServiceRoot.ServiceProvider.GetRequiredService<IEndjinWorkflowEngine>();

                var body = new StartWorkflow
                {
                    Context = envelope.StartWorkflowInstanceRequest.Context,
                    WorkflowId = envelope.StartWorkflowInstanceRequest.WorkflowId,
                    WorkflowInstanceId =
                                       envelope.StartWorkflowInstanceRequest.WorkflowInstanceId,
                    RequestId = envelope.StartWorkflowInstanceRequest.RequestId,
                };

                await client.StartWorkflowInstanceAsync(body).ConfigureAwait(false);
            }
        }
    }
}