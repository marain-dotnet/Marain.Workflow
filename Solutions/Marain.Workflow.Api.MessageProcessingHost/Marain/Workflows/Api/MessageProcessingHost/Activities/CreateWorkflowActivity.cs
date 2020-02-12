// <copyright file="CreateWorkflowActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessageProcessingHost.Activities
{
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Marain.Workflow.Api.EngineHost.Client;
    using Marain.Workflows.Api.MessageProcessingHost.Shared;
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
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateWorkflowActivity"/> class.
        /// </summary>
        /// <param name="configuration">The current configuration.</param>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        /// <param name="engineClient">The current client.</param>
        public CreateWorkflowActivity(
            IConfiguration configuration,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            IWorkflowEngineClient engineClient)
        {
            // TODO: Replace with custom config class.
            // https://github.com/marain-dotnet/Marain.Workflow/issues/45
            this.configuration = configuration;
            this.engineClient = engineClient;
            this.serializerSettingsProvider = serializerSettingsProvider;
        }

        /// <summary>
        /// The run action.
        /// </summary>
        /// <param name="context">
        /// The context.
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
            ILogger logger)
        {
            WorkflowMessageEnvelope envelope =
                context.GetInputWithCustomSerializationSettings<WorkflowMessageEnvelope>(this.serializerSettingsProvider.Instance);

            logger.LogInformation(
                $"Making function call for StartWorkflowInstanceRequest {envelope.StartWorkflowInstanceRequest.RequestId}");

            var body = new StartWorkflowRequest
            {
                Context = envelope.StartWorkflowInstanceRequest.Context,
                WorkflowId = envelope.StartWorkflowInstanceRequest.WorkflowId,
                WorkflowInstanceId = envelope.StartWorkflowInstanceRequest.WorkflowInstanceId,
                RequestId = envelope.StartWorkflowInstanceRequest.RequestId,
            };

            await this.engineClient.StartWorkflowInstanceAsync(envelope.TenantId, body);
        }
    }
}