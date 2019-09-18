// <copyright file="CreateCatalogItemTrigger.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;

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
        /// <returns>The workflow definition.</returns>
        public static Workflow Create(
            IServiceIdentityTokenSource tokenSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            string id,
            string externalServiceUrl)
        {
            var workflow = new Workflow(
                id,
                "External Action workflow",
                "Simple workflow using an external action");

            WorkflowState waitingToRun = workflow.CreateState(displayName: "Waiting to run");
            WorkflowState done = workflow.CreateState(displayName: "Done");

            workflow.SetInitialState(waitingToRun);

            WorkflowTransition transition = waitingToRun.CreateTransition(done);
            var action = new InvokeExternalServiceAction(tokenSource, serializerSettingsProvider)
            {
                Id = Guid.NewGuid().ToString(),
                AuthenticateWithManagedServiceIdentity = true,
                MsiAuthenticationResource = "foobar",
                ExternalUrl = externalServiceUrl,
                ContextItemsToInclude = new[] { "include1", "include2" }
            };
            transition.Actions.Add(action);

            return workflow;
        }
    }
}
