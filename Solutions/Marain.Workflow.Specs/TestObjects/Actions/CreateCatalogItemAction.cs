// <copyright file="CreateCatalogItemAction.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Actions
{
    using System;
    using System.Threading.Tasks;

    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Microsoft.Azure.Cosmos;

    public class CreateCatalogItemAction : IWorkflowAction
    {
        public const string RegisteredContentType = "application/vnd.marain.datacatalog.createcatalogitemaction";

        private readonly DataCatalogItemRepositoryFactory repositoryFactory;

        public CreateCatalogItemAction(DataCatalogItemRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public async Task ExecuteAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            var item = new CatalogItem
                           {
                               Id = instance.Id,
                               Identifier = instance.Context["Identifier"],
                               Type = instance.Context["Type"]
                           };

            instance.Context.Remove("Identifier");
            instance.Context.Remove("Type");

            var repository = this.repositoryFactory.GetRepository();
            await repository.UpsertItemAsync(item, new PartitionKey(item.PartitionKey)).ConfigureAwait(false);
        }

        public string ContentType => RegisteredContentType;
    }
}

#pragma warning restore