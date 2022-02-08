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
        [BeforeFeature("@setupTenantedCosmosContainers", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static void SetupCosmosDbRepository(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ICosmosContainerSourceWithTenantLegacyTransition factory = serviceProvider.GetRequiredService<ICosmosContainerSourceWithTenantLegacyTransition>();
            ICosmosOptionsFactory optionsFactory = serviceProvider.GetRequiredService<ICosmosOptionsFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            ITenant rootTenant = tenantProvider.Root;

            LegacyV2CosmosContainerConfiguration cosmosConfig =
                configuration.GetSection("TestCosmosConfiguration").Get<LegacyV2CosmosContainerConfiguration>()
                    ?? new LegacyV2CosmosContainerConfiguration();

            cosmosConfig.DatabaseName = "endjinspecssharedthroughput";
            cosmosConfig.DisableTenantIdPrefix = true;

            tenantProvider.Root.UpdateProperties(data => data.Append(new KeyValuePair<string, object>(
                $"StorageConfiguration__{TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowStoreLogicalDatabaseName}__{TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowDefinitionStoreLogicalContainerName}",
                cosmosConfig)));

            tenantProvider.Root.UpdateProperties(data => data.Append(new KeyValuePair<string, object>(
                $"StorageConfiguration__{TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowStoreLogicalDatabaseName}__{TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowInstanceStoreLogicalContainerName}",
                cosmosConfig)));

            tenantProvider.Root.UpdateProperties(data => data.Append(new KeyValuePair<string, object>(
                "StorageConfiguration__workflow__testdocuments",
                cosmosConfig)));

            Newtonsoft.Json.JsonSerializerSettings jsonSettings = serviceProvider.GetRequiredService<IJsonSerializerSettingsProvider>().Instance;
            Container testDocumentsRepository = WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                async () => await factory.GetContainerForTenantAsync(
                    rootTenant,
                    "StorageConfiguration__workflow__testdocuments",
                    "NotUsed",
                    "workflow",
                    "testdocuments",
                    "/id",
                    cosmosClientOptions: optionsFactory.CreateCosmosClientOptions())).Result;

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