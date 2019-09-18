// <copyright file="CatalogItemCompleteCondition.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Conditions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Marain.Workflows.Specs.TestObjects.Triggers;
    using Microsoft.Azure.Cosmos;

    public class CatalogItemCompleteCondition : IWorkflowCondition
    {
        public const string RegisteredContentType = "application/vnd.marain.datacatalog.catalogitemcompletecondition";

        private readonly DataCatalogItemRepositoryFactory repositoryFactory;

        public CatalogItemCompleteCondition(DataCatalogItemRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public string ContentType => RegisteredContentType;

        public string Id { get; set; }

        public async Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            if (trigger is ICatalogItemTrigger patchTrigger)
            {
                return await this.EvaluateAsync(instance, patchTrigger.CatalogItemId);
            }

            return false;
        }

        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            return new string[0];
        }

        private async Task<bool> EvaluateAsync(WorkflowInstance instance, string catalogItemId)
        {
            var repository = this.repositoryFactory.GetRepository();
            var item = await repository.ReadItemAsync<CatalogItem>(catalogItemId, new PartitionKey(catalogItemId)).ConfigureAwait(false);
            return item.Resource.IsComplete();
        }
    }
}

#pragma warning restore