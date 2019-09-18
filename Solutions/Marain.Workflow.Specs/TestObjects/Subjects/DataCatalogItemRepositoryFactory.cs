// <copyright file="DataCatalogItemRepositoryFactory.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Subjects
{
    using Marain.Workflows.Specs.Bindings;
    using Microsoft.Azure.Cosmos;
    using TechTalk.SpecFlow;

    public class DataCatalogItemRepositoryFactory
    {
        private readonly FeatureContext featureContext;

        public DataCatalogItemRepositoryFactory(FeatureContext featureContext)
        {
            this.featureContext = featureContext;
        }

        public Container GetRepository()
        {
            return this.featureContext.Get<Container>(WorkflowCosmosDbBindings.TestDocumentsRepository);
        }
    }
}

#pragma warning restore