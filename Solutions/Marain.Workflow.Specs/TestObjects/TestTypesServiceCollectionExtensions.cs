// <copyright file="TestTypesServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable
namespace Microsoft.Extensions.DependencyInjection
{
    using Corvus.ContentHandling;
    using Marain.Workflows.Specs.TestObjects.Actions;
    using Marain.Workflows.Specs.TestObjects.Conditions;
    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Marain.Workflows.Specs.TestObjects.Triggers;

    public static class TestTypesServiceCollectionExtensions
    {
        public static ContentFactory RegisterTestContentTypes(this ContentFactory contentFactory)
        {
            contentFactory.RegisterTransientContent<TraceAction>();
            contentFactory.RegisterTransientContent<ApplyCatalogItemPatchAction>();
            contentFactory.RegisterTransientContent<CreateCatalogItemAction>();
            contentFactory.RegisterTransientContent<SendCreateCatalogItemTriggerAction>();

            contentFactory.RegisterTransientContent<TriggerIdCondition>();
            contentFactory.RegisterTransientContent<BooleanCondition>();
            contentFactory.RegisterTransientContent<CatalogItemCompleteCondition>();
            contentFactory.RegisterTransientContent<CatalogItemIdCondition>();
            contentFactory.RegisterTransientContent<CatalogItemWillBeCompleteCondition>();
            contentFactory.RegisterTransientContent<ContextItemsPresentCondition>();

            contentFactory.RegisterTransientContent<CatalogItem>();
            contentFactory.RegisterTransientContent<CatalogItemPatch>();

            contentFactory.RegisterTransientContent<CreateCatalogItemTrigger>();
            contentFactory.RegisterTransientContent<DeleteCatalogItemTrigger>();
            contentFactory.RegisterTransientContent<DeprecateCatalogItemTrigger>();
            contentFactory.RegisterTransientContent<EditCatalogItemTrigger>();
            contentFactory.RegisterTransientContent<PublishCatalogItemTrigger>();
            contentFactory.RegisterTransientContent<CauseExternalInvocationTrigger>();

            return contentFactory;
        }
    }
}
#pragma warning restore