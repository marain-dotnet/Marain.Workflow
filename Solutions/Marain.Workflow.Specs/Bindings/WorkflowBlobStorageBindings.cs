// <copyright file="WorkflowBlobStorageBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Corvus.Storage.Azure.BlobStorage.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;

    using Marain.Workflows.Storage;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using TechTalk.SpecFlow;

    /// <summary>
    /// Specflow bindings to support BlobStorage DB.
    /// </summary>
    [Binding]
    public static class WorkflowBlobStorageBindings
    {
        /// <summary>
        /// The key for the document repository instance in the feature context.
        /// </summary>
        public const string TestDocumentsRepository = "TestDocumentsRepository";

        /// <summary>
        /// Set up a BlobStorage DB Repository for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <remarks>
        /// Note that this sets up a resource in Azure and will incur cost. Ensure the corresponding tear down operation
        /// is always run, or verify manually after a test run.
        /// </remarks>
        [BeforeFeature("@setupTenantedBlobStorageContainers", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static void SetupBlobStorageRepository(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            IBlobContainerSourceWithTenantLegacyTransition factory = serviceProvider.GetRequiredService<IBlobContainerSourceWithTenantLegacyTransition>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            ITenant rootTenant = tenantProvider.Root;

            LegacyV2BlobStorageConfiguration storageConfig =
                configuration.GetSection("TestStorageConfiguration").Get<LegacyV2BlobStorageConfiguration>()
                    ?? new LegacyV2BlobStorageConfiguration();

            // Generate a container name just for this test, otherwise you can end up with collisions
            // on subsequent tests runs while containers are still being deleted.
            storageConfig.Container = Guid.NewGuid().ToString();

            tenantProvider.Root.UpdateProperties(data => data.Append(new KeyValuePair<string, object>(
                "StorageConfiguration__workflowdefinitions",
                storageConfig)));
        }

        /// <summary>
        /// Tear down the cosmos DB repository for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <returns>A <see cref="Task" /> which completes once the operation has completed.</returns>
        [AfterFeature("@setupTenantedBlobStorageContainers", Order = 100000)]
        public static async Task TeardownBlobStorage(FeatureContext featureContext)
        {
            // Pretty nasty hack to get rid of the underlying containers for the stores.
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            ITenantedWorkflowStoreFactory workflowStoreFactory = serviceProvider.GetRequiredService<ITenantedWorkflowStoreFactory>();
            var workflowStore = (BlobStorageWorkflowStore)await workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenantProvider.Root).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(
                () => workflowStore.Container.DeleteAsync()).ConfigureAwait(false);
        }
    }
}