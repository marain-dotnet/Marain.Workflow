﻿// <copyright file="StartLongRunningOperationActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.MessagePreProcessingHost.Activities
{
    using System;
    using System.Threading.Tasks;
    using Marain.Composition;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Workflow.Functions.MessagePreProcessingHost.Shared;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     The durable activity for recording that work has started on a long-running operation.
    /// </summary>
    public static class StartLongRunningOperationActivity
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
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [FunctionName(nameof(StartLongRunningOperationActivity))]
        public static Task RunAction(
            [ActivityTrigger] DurableActivityContext context,
            ExecutionContext executionContext)
        {
            Initialization.Initialize(executionContext);
            Guid operationId = context.GetInput<Guid>();

            IEndjinOperationsControl operationsControl = ServiceRoot.ServiceProvider.GetRequiredService<IEndjinOperationsControl>();
            return operationsControl.SetOperationRunningAsync(operationId);
        }
    }
}
