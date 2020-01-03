// <copyright file="ExternalConditionWorkflowFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System;
    using System.Net.Http;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;

    /// <summary>
    /// Creates a workflow for testing external conditions.
    /// </summary>
    public static class ExternalConditionWorkflowFactory
    {
        /// <summary>
        /// Creates the workflow.
        /// </summary>
        /// <param name="id">Id to use for this workflow.</param>
        /// <param name="externalServiceUrl">External URL to invoke.</param>
        /// <param name="serviceIdentityTokenSource">The token source to use when authenticating to external services.</param>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        /// <returns>The workflow definition.</returns>
        public static Workflow Create(
            string id,
            string externalServiceUrl,
            IServiceIdentityTokenSource serviceIdentityTokenSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            var workflow = new Workflow(
                id,
                "External Condition workflow",
                "Simple workflow using an external condition");

            WorkflowState waitingToRun = workflow.CreateState(displayName: "Waiting to run");
            WorkflowState done = workflow.CreateState(displayName: "Done");

            workflow.SetInitialState(waitingToRun);

            WorkflowTransition transition = waitingToRun.CreateTransition(done);
            var condition = new InvokeExternalServiceCondition(serviceIdentityTokenSource, serializerSettingsProvider)
            {
                Id = Guid.NewGuid().ToString(),
                HttpMethod = HttpMethod.Post,
                AuthenticateWithManagedServiceIdentity = true,
                MsiAuthenticationResource = "foobar",
                ExternalUrl = externalServiceUrl,
                ContextItemsToInclude = new[] { "include1", "include2" },
            };
            transition.Conditions.Add(condition);

            return workflow;
        }
    }
}
