// <copyright file="FailLongRunningOperationActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessagePreProcessingHost.Activities
{
    using System;
    using System.Threading.Tasks;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Operations.Client.OperationsControl.Models;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;

    /// <summary>
    /// The durable activity for recording that work on a long-running operation is complete.
    /// </summary>
    public class FailLongRunningOperationActivity
    {
        private readonly IMarainOperationsControl operationsControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteLongRunningOperationActivity"/> class.
        /// </summary>
        /// <param name="operationsControl">The operations control client.</param>
        public FailLongRunningOperationActivity(IMarainOperationsControl operationsControl)
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
        [FunctionName(nameof(FailLongRunningOperationActivity))]
        public async Task RunAction(
            [ActivityTrigger] IDurableActivityContext context,
            ExecutionContext executionContext)
        {
            (Guid operationId, string tenantId) = context.GetInput<(Guid, string)>();

            ProblemDetails operationResult = await this.operationsControl.SetOperationFailedAsync(tenantId, operationId).ConfigureAwait(false);

            if (operationResult != null)
            {
                throw new Exception($"Unexpected error when trying to set operation {operationId} for tenant {tenantId} to Succeeded");
            }
        }
    }
}
