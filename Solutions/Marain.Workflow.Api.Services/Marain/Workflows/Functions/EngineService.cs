// <copyright file="EngineService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Functions
{
    using System.Linq;
    using System.Threading.Tasks;

    using Marain.OpenApi;
    using Marain.Telemetry;
    using Marain.Tenancy;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Handles incoming triggers posted to the trigger service.
    /// </summary>
    [EmbeddedOpenApiDefinition("Marain.Workflows.Functions.EngineService.yaml")]
    public class EngineService : IOpenApiService
    {
        private const string StartWorkflowInstanceOperationId = "startWorkflowInstance";
        private const string SendTriggerOperationId = "sendTrigger";

        private readonly IWorkflowEngineFactory workflowEngineFactory;
        private readonly TelemetryClient telemetryClient;
        private readonly ILogger<EngineService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineService"/> class.
        /// </summary>
        /// <param name="workflowEngineFactory">The workflow engine factory.</param>
        /// <param name="logger">The logger.</param>
        /// /// <param name="telemetryClient">A <see cref="TelemetryClient"/> to log telemetry.</param>
        public EngineService(IWorkflowEngineFactory workflowEngineFactory, ILogger<EngineService> logger, TelemetryClient telemetryClient)
        {
            this.workflowEngineFactory = workflowEngineFactory;
            this.telemetryClient = telemetryClient;
            this.logger = logger;
        }

        /// <summary>
        ///     Handles an incoming trigger for an instance.
        /// </summary>
        /// <param name="workflowInstanceId">
        ///     The Id of the workflow instance to which this trigger will be applied.
        /// </param>
        /// <param name="body">
        ///     The trigger.
        /// </param>
        /// <returns>
        ///     The <see cref="OpenApiResult" />.
        /// </returns>
        [OperationId(SendTriggerOperationId)]
        public async Task<OpenApiResult> HandleTrigger(string workflowInstanceId, IWorkflowTrigger body)
        {
            using (this.telemetryClient.StartOperation<RequestTelemetry>(SendTriggerOperationId))
            {
                TelemetryOperationContext.Current.Properties["Endjin_WorkflowTriggerId"] = body.Id;

                // TODO: We should be passing the tenant in through e.g. a header
                IWorkflowEngine workflowEngine = await this.workflowEngineFactory.GetWorkflowEngineAsync(Tenant.Root).ConfigureAwait(false);
                await workflowEngine.ProcessTriggerAsync(body, workflowInstanceId).ConfigureAwait(false);
                return this.OkResult();
            }
        }

        /// <summary>
        ///     The handle trigger.
        /// </summary>
        /// <param name="body">
        ///     The request body.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [OperationId(StartWorkflowInstanceOperationId)]
        public async Task<OpenApiResult> StartWorkflowInstance(StartWorkflowInstanceRequest body)
        {
            using (this.telemetryClient.StartOperation<RequestTelemetry>(StartWorkflowInstanceOperationId))
            {
                TelemetryOperationContext.Current.Properties["Endjin_WorkflowRequestId"] = body.RequestId;
                TelemetryOperationContext.Current.Properties["Endjin_WorkflowId"] = body.WorkflowId;

                // TODO: We should be passing the tenant in through e.g. a header
                IWorkflowEngine workflowEngine = await this.workflowEngineFactory.GetWorkflowEngineAsync(Tenant.Root).ConfigureAwait(false);

                await workflowEngine.StartWorkflowInstanceAsync(body).ConfigureAwait(false);
                return this.CreatedResult();
            }
        }
    }
}