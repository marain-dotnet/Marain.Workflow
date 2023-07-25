// <copyright file="TraceAction.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TechTalk.SpecFlow;

    public class TraceAction : IWorkflowAction
    {
        public const string ScenarioContextListName = "WorkflowTraces";

        public const string RegisteredContentType = "application/vnd.marain.workflows.specs.actions.trace";

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ContentType => RegisteredContentType;

        public string Message { get; set; }

        public Task ExecuteAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            var log = GetTraces();

            log.Add(this.Message);

            return Task.CompletedTask;
        }

        public static IList<string> GetTraces()
        {
            if (ScenarioContext.Current.ContainsKey(ScenarioContextListName))
            {
                return (IList<string>)ScenarioContext.Current[ScenarioContextListName];
            }

            var log = new List<string>();
            
            ScenarioContext.Current[ScenarioContextListName] = log;

            return log;
        }
    }
}

#pragma warning restore