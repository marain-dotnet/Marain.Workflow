﻿// <copyright file="CompleteLongRunningOperationActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessageProcessingHost.Activities
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
    public class CompleteLongRunningOperationActivity
    {
        private readonly IMarainOperationsControl operationsControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteLongRunningOperationActivity"/> class.
        /// </summary>
        /// <param name="operationsControl">The operations control client.</param>
        public CompleteLongRunningOperationActivity(IMarainOperationsControl operationsControl)
        {
            this.operationsControl = operationsControl;
        }

        /// <summary>
        /// The run action.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        [FunctionName(nameof(CompleteLongRunningOperationActivity))]
        public async Task RunAction(
            [ActivityTrigger] IDurableActivityContext context)
        {
            (Guid operationId, string tenantId) = context.GetInput<(Guid, string)>();

            ProblemDetails operationResult = await this.operationsControl.SetOperationSucceededAsync(tenantId, operationId);

            if (operationResult != null)
            {
                var exception = new Exception($"Unexpected arror when attempting to start long running operation '{operationId}' for tenant '{tenantId}': {operationResult.Status} - {operationResult.Title}");
                exception.Data.Add("ProblemDetails", operationResult);
                throw exception;
            }
        }
    }
}