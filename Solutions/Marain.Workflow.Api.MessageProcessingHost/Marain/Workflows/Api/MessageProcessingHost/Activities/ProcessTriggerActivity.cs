// <copyright file="ProcessTriggerActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessageProcessingHost.Activities
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Marain.Workflows;
    using Marain.Workflows.Api.MessageProcessingHost.Shared;
    using Marain.Workflows.Client;
    using Marain.Workflows.Client.Models;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The create workflow activity.
    /// </summary>
    public class ProcessTriggerActivity
    {
        private readonly ITenantedWorkflowEngineFactory workflowEngineFactory;
        private readonly IMarainWorkflowEngine client;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkflowInstanceCountActivity"/> class.
        /// </summary>
        /// <param name="workflowEngineFactory">The factory class for the workflow engine.</param>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        /// <param name="client">The client for the workflow engine.</param>
        public ProcessTriggerActivity(
            ITenantedWorkflowEngineFactory workflowEngineFactory,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            IMarainWorkflowEngine client)
        {
            this.workflowEngineFactory = workflowEngineFactory;
            this.client = client;
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
        [FunctionName(nameof(ProcessTriggerActivity))]
        public async Task RunAction(
            [ActivityTrigger] IDurableActivityContext context,
            ILogger logger)
        {
            WorkflowMessageEnvelope envelope =
                context.GetInputWithCustomSerializationSettings<WorkflowMessageEnvelope>(this.serializerSettingsProvider.Instance);

            var workflowTrigger = (HostedWorkflowTrigger)envelope.Trigger;
            string instanceId = envelope.GetWorkflowInstanceId();

            logger.LogInformation(
                $"Making function call to process trigger {workflowTrigger.Id} for instance {instanceId}");

            var subjects = new ObservableCollection<string>();
            workflowTrigger.Subjects?.ForEach(subjects.Add);

            var body = new Trigger
            {
                Id = workflowTrigger.Id,
                Subjects = subjects,
                TriggerName = workflowTrigger.TriggerName,
                Parameters = workflowTrigger.Parameters,
            };

            await this.client.SendTriggerAsync(envelope.TenantId, instanceId, body);
        }
    }
}