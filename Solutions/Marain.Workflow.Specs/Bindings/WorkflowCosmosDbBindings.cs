// <copyright file="WorkflowCosmosDbBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Workflows.Specs.Steps;
    using Marain.Workflows.Storage;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Specflow bindings to support Cosmos DB.
    /// </summary>
    [Binding]
    public static class WorkflowCosmosDbBindings
    {
        /// <summary>
        /// The key for the document repository instance in the feature context.
        /// </summary>
        public const string TestDocumentsRepository = "TestDocumentsRepository";

        /// <summary>
        /// Set up a Cosmos DB Repository for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <remarks>
        /// Note that this sets up a resource in Azure and will incur cost. Ensure the corresponding tear down operation
        /// is always run, or verify manually after a test run.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@setupTenantedCosmosContainers", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task SetupCosmosDbRepository(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantCosmosContainerFactory factory = serviceProvider.GetRequiredService<ITenantCosmosContainerFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            ITenant testTenant = await tenantProvider.CreateChildTenantAsync(tenantProvider.Root.Id, "Test tenant").ConfigureAwait(false);

            CosmosConfiguration cosmosConfig =
                configuration.GetSection("TestCosmosConfiguration").Get<CosmosConfiguration>()
                    ?? new CosmosConfiguration();

            cosmosConfig.DatabaseName = "endjinspecssharedthroughput";
            cosmosConfig.DisableTenantIdPrefix = true;

            testTenant.SetCosmosConfiguration(
                TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowStoreContainerDefinition,
                cosmosConfig);

            testTenant.SetCosmosConfiguration(
                TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowInstanceStoreContainerDefinition,
                cosmosConfig);

            testTenant.SetCosmosConfiguration(
                TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowInstanceChangeLogContainerDefinition,
                cosmosConfig);

            var testDocumentRepositoryContainerDefinition = new CosmosContainerDefinition("workflow", "testdocuments", "/id");
            testTenant.SetCosmosConfiguration(
                testDocumentRepositoryContainerDefinition,
                cosmosConfig);

            Container testDocumentsRepository = await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                () => factory.GetContainerForTenantAsync(
                    testTenant,
                    testDocumentRepositoryContainerDefinition)).ConfigureAwait(false);

            featureContext.Set(testDocumentsRepository, TestDocumentsRepository);

            featureContext.Set(testTenant);
        }

        /// <summary>
        /// Tear down the cosmos DB repository for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <returns>A <see cref="Task" /> which completes once the operation has completed.</returns>
        [AfterFeature("@setupTenantedCosmosContainers", Order = 100000)]
        public static async Task TeardownCosmosDb(FeatureContext featureContext)
        {
            // Pretty nasty hack to get rid of the underlying containers for the stores.
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
            ITenant tenant = featureContext.Get<ITenant>();
            ITenantedWorkflowStoreFactory workflowStoreFactory = serviceProvider.GetRequiredService<ITenantedWorkflowStoreFactory>();
            var workflowStore = (CosmosWorkflowStore)await workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(
                () => workflowStore.Container.DeleteContainerAsync()).ConfigureAwait(false);

            ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory = serviceProvider.GetRequiredService<ITenantedWorkflowInstanceStoreFactory>();
            var workflowInstanceStore = (CosmosWorkflowInstanceStore)await workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenant).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(
                () => workflowInstanceStore.Container.DeleteContainerAsync()).ConfigureAwait(false);

            ITenantedWorkflowInstanceChangeLogFactory workflowInstanceChangeLogFactory = serviceProvider.GetRequiredService<ITenantedWorkflowInstanceChangeLogFactory>();
            var workflowInstanceChangeLog = (CosmosWorkflowInstanceChangeLog)await workflowInstanceChangeLogFactory.GetWorkflowInstanceChangeLogWriterForTenantAsync(tenant).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(
                () => workflowInstanceChangeLog.Container.DeleteContainerAsync()).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(
                () => featureContext.Get<Container>(TestDocumentsRepository).DeleteContainerAsync()).ConfigureAwait(false);
        }
    }
}