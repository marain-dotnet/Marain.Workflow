// <copyright file="ApplyCatalogItemPatchAction.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Actions
{
    using System;
    using System.Threading.Tasks;

    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Marain.Workflows.Specs.TestObjects.Triggers;
    using Microsoft.Azure.Cosmos;

    public class ApplyCatalogItemPatchAction : IWorkflowAction
    {
        public const string RegisteredContentType = "application/vnd.marain.datacatalog.applycatalogitempatchaction";

        private readonly DataCatalogItemRepositoryFactory repositoryFactory;

        public ApplyCatalogItemPatchAction(DataCatalogItemRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public string ContentType => RegisteredContentType;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public async Task ExecuteAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            if (trigger is EditCatalogItemTrigger patchTrigger)
            {
                await this.ExecuteAsync(instance, patchTrigger.PatchDetails);
            }
        }

        protected async Task ExecuteAsync(WorkflowInstance instance, CatalogItemPatch content)
        {
            var repository = this.repositoryFactory.GetRepository();
            var existingCatalogItem = await repository.ReadItemAsync<CatalogItem>(content.Id, new PartitionKey(content.Id)).ConfigureAwait(false);
            var newCatalogItem = content.ApplyTo(existingCatalogItem.Resource);
            await repository.UpsertItemAsync(newCatalogItem, new PartitionKey(newCatalogItem.PartitionKey)).ConfigureAwait(false);
        }
    }
}

#pragma warning restore