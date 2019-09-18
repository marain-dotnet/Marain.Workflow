// <copyright file="BooleanCondition.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BooleanCondition : IWorkflowCondition
    {
        public const string RegisteredContentType = "application/vnd.marain.workflows.specs.conditions.boolean";

        public string ContentType => RegisteredContentType;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public bool Value { get; set; }

        public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            return Task.FromResult(this.Value);
        }

        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            return new string[0];
        }
    }
}

#pragma warning restore