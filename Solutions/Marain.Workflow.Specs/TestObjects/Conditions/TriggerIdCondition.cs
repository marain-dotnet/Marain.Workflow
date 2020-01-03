// <copyright file="TriggerIdCondition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable
namespace Marain.Workflows.Specs.TestObjects.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class TriggerIdCondition : IWorkflowCondition
    {
        public const string RegisteredContentType = "application/vnd.marain.workflows.specs.conditions.trigger-id";

        public string ContentType => RegisteredContentType;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string TriggerId { get; set; }

        public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            return Task.FromResult(trigger.Id == this.TriggerId);
        }

        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            return new string[0];
        }
    }
}
#pragma warning restore