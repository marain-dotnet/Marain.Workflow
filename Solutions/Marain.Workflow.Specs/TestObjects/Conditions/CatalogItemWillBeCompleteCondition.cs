// <copyright file="CatalogItemWillBeCompleteCondition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Marain.Workflows.Specs.TestObjects.Triggers;
    using Microsoft.Azure.Cosmos;

    public class CatalogItemWillBeCompleteCondition : IWorkflowCondition
    {
        public const string RegisteredContentType =
            "application/vnd.endjin.datacatalog.catalogitemwillcompletecondition";

        private readonly DataCatalogItemRepositoryFactory repositoryFactory;

        public CatalogItemWillBeCompleteCondition(DataCatalogItemRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public string ContentType => RegisteredContentType;

        public bool ExpectedResult { get; set; } = true;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public async Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            if (trigger is EditCatalogItemTrigger editTrigger)
            {
                return await this.EvaluateAsync(instance, editTrigger.PatchDetails).ConfigureAwait(false);
            }

            return false;
        }

        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            return new string[0];
        }

        private async Task<bool> EvaluateAsync(WorkflowInstance instance, CatalogItemPatch patch)
        {
            var repository = this.repositoryFactory.GetRepository();
            var item = await repository.ReadItemAsync<CatalogItem>(patch.Id, new PartitionKey(patch.Id)).ConfigureAwait(false);
            var newItem = patch.ApplyTo(item);
            return newItem.IsComplete() == this.ExpectedResult;
        }
    }
}

#pragma warning restore