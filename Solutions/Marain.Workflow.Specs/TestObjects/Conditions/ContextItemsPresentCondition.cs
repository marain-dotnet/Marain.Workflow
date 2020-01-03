// <copyright file="ContextItemsPresentCondition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ContextItemsPresentCondition : IWorkflowCondition
    {
        public const string RegisteredContentType = "application/vnd.endjin.datacatalog.contextitemspresentcondition";

        public string ContentType => RegisteredContentType;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public IEnumerable<string> RequiredContextItems { get; set; }

        public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            foreach (var current in this.RequiredContextItems)
            {
                if (!instance.Context.ContainsKey(current))
                {
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }

        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            return Enumerable.Empty<string>();
        }
    }
}

#pragma warning restore