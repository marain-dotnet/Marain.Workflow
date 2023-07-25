// <copyright file="DataCatalogItemRepositoryFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Subjects
{
    using Microsoft.Azure.Cosmos;
    using Marain.Workflows.Specs.Bindings;

    using TechTalk.SpecFlow;

    public class DataCatalogItemRepositoryFactory
    {
        public Container GetRepository()
        {
            return (Container)FeatureContext.Current[WorkflowCosmosDbBindings.TestDocumentsRepository];
        }
    }
}

#pragma warning restore