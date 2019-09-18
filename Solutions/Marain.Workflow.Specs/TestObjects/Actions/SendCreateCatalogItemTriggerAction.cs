// <copyright file="SendCreateCatalogItemTriggerAction.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Actions
{
    using System;
    using System.Threading.Tasks;

    using Marain.Workflows.Specs.TestObjects.Triggers;

    public class SendCreateCatalogItemTriggerAction : IWorkflowAction
    {
        private readonly IWorkflowMessageQueue queue;

        public const string RegisteredContentType = "application/vnd.marain.datacatalog.sendcreatecatalogitemtriggeraction";

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public SendCreateCatalogItemTriggerAction(IWorkflowMessageQueue queue)
        {
            this.queue = queue;
        }

        public Task ExecuteAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            var newTrigger = new CreateCatalogItemTrigger { CatalogItemId = instance.Id };

            return this.queue.EnqueueTriggerAsync(newTrigger, default(Guid));
        }

        public string ContentType => RegisteredContentType;
    }
}

#pragma warning restore