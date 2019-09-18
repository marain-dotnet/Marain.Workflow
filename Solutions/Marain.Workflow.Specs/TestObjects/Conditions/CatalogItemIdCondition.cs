// <copyright file="CatalogItemIdCondition.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Marain.Workflows.Specs.TestObjects.Triggers;

    public class CatalogItemIdCondition : IWorkflowCondition
    {
        public const string RegisteredContentType = "application/vnd.marain.datacatalog.catalogitemidcondition";

        public string ContentType => RegisteredContentType;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            if (trigger is ICatalogItemTrigger itemTrigger)
            {
                instance.Context["CatalogItemId"] = itemTrigger.CatalogItemId;
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