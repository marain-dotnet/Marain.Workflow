// <copyright file="WorkflowCosmosDbBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Corvus.CosmosClient;
    using Corvus.Extensions.Json;
    using Corvus.Storage.Azure.Cosmos;
    using Corvus.Storage.Azure.Cosmos.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;

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
        /// <returns>A <see cref="Task"/> indicating when setup completes.</returns>
        [BeforeFeature("@setupTenantedCosmosContainers", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task SetupCosmosDbRepository(FeatureContext featureContext)
        {
            // We need each test run to have a distinct container. We want these test-generated
            // containers to be easily recognized in storage accounts, so we don't just want to use
            // GUIDs.
            string testRunId = DateTime.Now.ToString("yyyy-MM-dd-hhmmssfff");

            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ICosmosContainerSourceFromDynamicConfiguration cosmosContainerSource = serviceProvider.GetRequiredService<ICosmosContainerSourceFromDynamicConfiguration>();
            ICosmosContainerSourceWithTenantLegacyTransition factory = serviceProvider.GetRequiredService<ICosmosContainerSourceWithTenantLegacyTransition>();
            ICosmosOptionsFactory optionsFactory = serviceProvider.GetRequiredService<ICosmosOptionsFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            ITenant rootTenant = tenantProvider.Root;

            CosmosContainerConfiguration cosmosConfig =
                configuration.GetSection("TestCosmosConfiguration").Get<CosmosContainerConfiguration>()
                    ?? new CosmosContainerConfiguration();

            cosmosConfig.Database = "endjinspecssharedthroughput";
            CosmosContainerConfiguration definitionsStoreConfig = cosmosConfig with
            {
                Container = $"specs-workflow-definitions-{testRunId}",
            };
            CosmosContainerConfiguration instancesStoreConfig = cosmosConfig with
            {
                Container = $"specs-workflow-instances-{testRunId}",
            };
            CosmosContainerConfiguration appDataStoreConfig = cosmosConfig with
            {
                Container = $"specs-workflow-testdocuments-{testRunId}",
            };

            Container definitionsContainer = await cosmosContainerSource.GetStorageContextAsync(definitionsStoreConfig).ConfigureAwait(false);
            await definitionsContainer.Database
                .CreateContainerIfNotExistsAsync(
                    definitionsContainer.Id,
                    TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowDefinitionStorePartitionKeyPath)
                .ConfigureAwait(false);
            Container instancesContainer = await cosmosContainerSource.GetStorageContextAsync(instancesStoreConfig);
            await instancesContainer.Database
                .CreateContainerIfNotExistsAsync(
                    instancesContainer.Id,
                    TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowInstanceStorePartitionKeyPath)
                .ConfigureAwait(false);

            tenantProvider.Root.UpdateProperties(data => data.Concat(new Dictionary<string, object>
            {
                { WorkflowCosmosTenancyPropertyKeys.Definitions, definitionsStoreConfig },
                { WorkflowCosmosTenancyPropertyKeys.Instances, instancesStoreConfig },
                { "StorageConfiguration__workflow__testdocuments", appDataStoreConfig },
            }));

            Newtonsoft.Json.JsonSerializerSettings jsonSettings = serviceProvider.GetRequiredService<IJsonSerializerSettingsProvider>().Instance;
            Container testDocumentsRepository = await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                async () => await factory.GetContainerForTenantAsync(
                    rootTenant,
                    "NotUsed",
                    "StorageConfiguration__workflow__testdocuments",
                    "workflow",
                    "testdocuments",
                    "/id",
                    cosmosClientOptions: optionsFactory.CreateCosmosClientOptions())).ConfigureAwait(false);
            await testDocumentsRepository.Database
                .CreateContainerIfNotExistsAsync(
                    testDocumentsRepository.Id,
                    "/id")
                .ConfigureAwait(false);

            featureContext.Set(testDocumentsRepository, TestDocumentsRepository);
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

            ITenantedWorkflowStoreFactory workflowStoreFactory = serviceProvider.GetRequiredService<ITenantedWorkflowStoreFactory>();
            var workflowStore = (CosmosWorkflowStore)await workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenantProvider.Root).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(
                () => workflowStore.Container.DeleteContainerAsync()).ConfigureAwait(false);

            ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory = serviceProvider.GetRequiredService<ITenantedWorkflowInstanceStoreFactory>();
            var workflowInstanceStore = (CosmosWorkflowInstanceStore)await workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenantProvider.Root).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(
                () => workflowInstanceStore.Container.DeleteContainerAsync()).ConfigureAwait(false);

            await featureContext.RunAndStoreExceptionsAsync(
                () => featureContext.Get<Container>(TestDocumentsRepository).DeleteContainerAsync()).ConfigureAwait(false);
        }
    }
}