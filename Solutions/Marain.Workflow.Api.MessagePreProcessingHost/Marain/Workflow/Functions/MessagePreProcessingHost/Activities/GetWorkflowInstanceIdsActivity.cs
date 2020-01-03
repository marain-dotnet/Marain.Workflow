// <copyright file="GetWorkflowInstanceIdsActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.MessagePreProcessingHost.Activities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Marain.Composition;
    using Marain.Workflow.Functions.MessagePreProcessingHost.Shared;
    using Marain.Workflows;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The create workflow activity.
    /// </summary>
    public static class GetWorkflowInstanceIdsActivity
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
        [FunctionName(nameof(GetWorkflowInstanceIdsActivity))]
        public static async Task<string[]> RunAction(
            [ActivityTrigger] DurableActivityContext context,
            ExecutionContext executionContext,
            ILogger logger)
        {
            Initialization.Initialize(executionContext);

            ParameterDataWithTelemetry data = context.GetInput<ParameterDataWithTelemetry>();
            data.InitializeTelemetryOperationContext(executionContext, logger);

            TelemetryClient telemetryClient = ServiceRoot.ServiceProvider.GetService<TelemetryClient>();

            using (telemetryClient.StartOperation<RequestTelemetry>(
                executionContext.GetFunctionNameForBespokeStartOperation()))
            {
                WorkflowMessageEnvelope envelope = data.GetWorkflowMessageEnvelope();
                IWorkflowEngineFactory workflowEngineFactory = ServiceRoot.ServiceProvider.GetService<IWorkflowEngineFactory>();
                IWorkflowEngine workflowEngine = await workflowEngineFactory.GetWorkflowEngineAsync(data.Tenant).ConfigureAwait(false);
                IEnumerable<string> instanceIds = await workflowEngine.GetMatchingWorkflowInstanceIdsForSubjectsAsync(
                                      envelope.Trigger.GetSubjects(),
                                      data.GetWorkflowInstancesPageNumber(),
                                      500).ConfigureAwait(false);

                return instanceIds.ToArray();
            }
        }
    }
}