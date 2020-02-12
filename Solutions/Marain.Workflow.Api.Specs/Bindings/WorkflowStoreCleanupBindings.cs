// <copyright file="WorkflowStoreCleanupBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.ContentManagement.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Workflows;
    using Marain.Workflows.Storage;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Cleans up the tenant-specific workflow stores that will likely have been created as a result of calls to the API. This
    /// is intended to be used in conjunction with <see cref="TransientTenantBindings"/>, which will create and tear down a
    /// tenant specifically for the current feature.
    /// </summary>
    [Binding]
    public static class WorkflowStoreCleanupBindings
    {
        /// <summary>
        /// Gets a reference to the tenant container used for this test, then deletes it.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// It's critically important that this runs to clean up Cosmos collections created as part of test execution. As such,
        /// this code runs for every feature but doesn't do anything if it can't obtain both the tenant and service provider
        /// from the <c>FeatureContext</c>.
        /// </remarks>
        [AfterFeature]
        public static async Task ClearDownTransientTenantContentStore(FeatureContext context)
        {
            ITenant currentTenant = context.GetTransientTenant();
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(context);

            if (currentTenant != null && serviceProvider != null)
            {
                await context.RunAndStoreExceptionsAsync(async () =>
                    {
                        ITenantedWorkflowStoreFactory workflowStoreFactory = serviceProvider.GetRequiredService<ITenantedWorkflowStoreFactory>();
                        var workflowStore = (CosmosWorkflowStore)await workflowStoreFactory.GetWorkflowStoreForTenantAsync(currentTenant).ConfigureAwait(false);
                        await workflowStore.Container.DeleteContainerAsync().ConfigureAwait(false);
                    }).ConfigureAwait(false);

                await context.RunAndStoreExceptionsAsync(async () =>
                {
                    ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory = serviceProvider.GetRequiredService<ITenantedWorkflowInstanceStoreFactory>();
                    var workflowInstanceStore = (CosmosWorkflowInstanceStore)await workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(currentTenant).ConfigureAwait(false);
                    await workflowInstanceStore.Container.DeleteContainerAsync().ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }
    }
}
