// <copyright file="ExternalActionWorkflowFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System;
    using System.Collections.Generic;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ClientAuthentication;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Creates a workflow for testing external actions.
    /// </summary>
    public static class ExternalActionWorkflowFactory
    {
        /// <summary>
        /// Creates the workflow.
        /// </summary>
        /// <param name="id">Id to use for this workflow.</param>
        /// <param name="externalServiceUrl">External URL to invoke.</param>
        /// <param name="serviceIdentityTokenSource">The token source to use when authenticating to external services.</param>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        /// <param name="externalServiceActionLogger">The logger for <see cref="InvokeExternalServiceAction"/>s.</param>
        /// <returns>The workflow definition.</returns>
        public static Workflow Create(
            string id,
            string externalServiceUrl,
            IServiceIdentityAccessTokenSource serviceIdentityTokenSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ILogger<InvokeExternalServiceAction> externalServiceActionLogger)
        {
            Dictionary<string, WorkflowState> states = new();
            WorkflowState waitingToRun = states.AddState("WaitingToRun", displayName: "Waiting to run");
            WorkflowState done = states.AddState("Done", displayName: "Done");

            var workflow = new Workflow(
                id,
                states,
                waitingToRun.Id,
                "External Action workflow",
                "Simple workflow using an external action");

            WorkflowTransition transition = waitingToRun.CreateTransition(done);
            var action = new InvokeExternalServiceAction(
                serviceIdentityTokenSource,
                serializerSettingsProvider,
                externalServiceActionLogger)
            {
                Id = Guid.NewGuid().ToString(),
                AuthenticateWithManagedServiceIdentity = true,
                MsiAuthenticationResource = "foobar",
                ExternalUrl = externalServiceUrl,
                ContextItemsToInclude = new[] { "include1", "include2" },
            };
            transition.Actions.Add(action);

            return workflow;
        }
    }
}