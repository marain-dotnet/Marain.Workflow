// <copyright file="TestTypesServiceCollectionExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
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
        public static IServiceCollection RegisterTestContentTypes(this IServiceCollection services)
        {
            services.AddContent(factory =>
            {
                factory.RegisterTransientContent<TraceAction>();
                factory.RegisterTransientContent<ApplyCatalogItemPatchAction>();
                factory.RegisterTransientContent<CreateCatalogItemAction>();
                factory.RegisterTransientContent<SendCreateCatalogItemTriggerAction>();
                factory.RegisterTransientContent<TriggerIdCondition>();
                factory.RegisterTransientContent<BooleanCondition>();
                factory.RegisterTransientContent<CatalogItemCompleteCondition>();
                factory.RegisterTransientContent<CatalogItemIdCondition>();
                factory.RegisterTransientContent<CatalogItemWillBeCompleteCondition>();
                factory.RegisterTransientContent<ContextItemsPresentCondition>();
                factory.RegisterTransientContent<CatalogItem>();
                factory.RegisterTransientContent<CatalogItemPatch>();
                factory.RegisterTransientContent<CreateCatalogItemTrigger>();
                factory.RegisterTransientContent<DeleteCatalogItemTrigger>();
                factory.RegisterTransientContent<DeprecateCatalogItemTrigger>();
                factory.RegisterTransientContent<EditCatalogItemTrigger>();
                factory.RegisterTransientContent<PublishCatalogItemTrigger>();
                factory.RegisterTransientContent<CauseExternalInvocationTrigger>();
            });

            return services;
        }
    }
}
#pragma warning restore