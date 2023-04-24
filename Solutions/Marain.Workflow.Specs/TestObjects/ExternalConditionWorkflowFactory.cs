// <copyright file="ExternalConditionWorkflowFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ClientAuthentication;
    using Microsoft.SqlServer.Management.Sdk.Sfc;

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
            IServiceIdentityAccessTokenSource serviceIdentityTokenSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            Dictionary<string, WorkflowState> states = new();
            WorkflowState waitingToRun = states.AddState("WaitingToRun", displayName: "Waiting to run");
            WorkflowState done = states.AddState("Done", displayName: "Done");

            var workflow = new Workflow(
                id,
                states,
                waitingToRun.Id,
                "External Condition workflow",
                "Simple workflow using an external condition");

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