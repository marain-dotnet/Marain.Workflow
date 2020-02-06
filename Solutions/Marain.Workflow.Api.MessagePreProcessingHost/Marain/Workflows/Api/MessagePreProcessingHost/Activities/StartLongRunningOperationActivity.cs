// <copyright file="StartLongRunningOperationActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessagePreProcessingHost.Activities
{
    using System;
    using System.Threading.Tasks;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Operations.Client.OperationsControl.Models;
    using Marain.Workflows.Api.MessagePreProcessingHost.Shared;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The durable activity for recording that work has started on a long-running operation.
    /// </summary>
    public class StartLongRunningOperationActivity
    {
        private readonly IMarainOperationsControl operationsControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteLongRunningOperationActivity"/> class.
        /// </summary>
        /// <param name="operationsControl">The operations control client.</param>
        public StartLongRunningOperationActivity(IMarainOperationsControl operationsControl)
        {
            this.operationsControl = operationsControl;
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
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        [FunctionName(nameof(StartLongRunningOperationActivity))]
        public async Task RunAction(
            [ActivityTrigger] IDurableActivityContext context,
            ExecutionContext executionContext)
        {
            (Guid operationId, string tenantId) = context.GetInput<(Guid, string)>();

            // TODO: Check the result for problems.
            ProblemDetails operationResult = await this.operationsControl.SetOperationRunningAsync(tenantId, operationId).ConfigureAwait(false);
        }
    }
}
