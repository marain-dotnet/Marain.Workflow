// <copyright file="CreateWorkflowActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessagePreProcessingHost.Activities
{
    using System.Threading.Tasks;
    using Marain.Workflow.Api.EngineHost.Client;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The create workflow activity.
    /// </summary>
    public class CreateWorkflowActivity
    {
        private readonly IConfiguration configuration;
        private readonly IWorkflowEngineClient engineClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateWorkflowActivity"/> class.
        /// </summary>
        /// <param name="configuration">The current configuration.</param>
        /// <param name="engineClient">The current client.</param>
        public CreateWorkflowActivity(
            IConfiguration configuration,
            IWorkflowEngineClient engineClient)
        {
            // TODO: Replace with custom config class.
            this.configuration = configuration;
            this.engineClient = engineClient;
        }

        /// <summary>
        /// The run action.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="executionContext">
        /// The execution Context.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        [FunctionName(nameof(CreateWorkflowActivity))]
        public async Task RunAction(
            [ActivityTrigger] IDurableActivityContext context,
            ExecutionContext executionContext,
            ILogger logger)
        {
            WorkflowMessageEnvelope envelope = context.GetInput<WorkflowMessageEnvelope>();

            logger.LogInformation(
                $"Making function call for StartWorkflowInstanceRequest {envelope.StartWorkflowInstanceRequest.RequestId}");

            var body = new StartWorkflowRequest
            {
                Context = envelope.StartWorkflowInstanceRequest.Context,
                WorkflowId = envelope.StartWorkflowInstanceRequest.WorkflowId,
                WorkflowInstanceId = envelope.StartWorkflowInstanceRequest.WorkflowInstanceId,
                RequestId = envelope.StartWorkflowInstanceRequest.RequestId,
            };

            await this.engineClient.StartWorkflowInstanceAsync(envelope.TenantId, body).ConfigureAwait(false);
        }
    }
}