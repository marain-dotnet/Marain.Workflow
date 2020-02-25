// <copyright file="WorkflowSqlBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Sql.Tenancy;
    using Corvus.Tenancy;
    using Marain.Workflows.Specs.Steps;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Specflow bindings to support Cosmos DB.
    /// </summary>
    [Binding]
    public static class WorkflowSqlBindings
    {
        /// <summary>
        /// The key for the document repository instance in the feature context.
        /// </summary>
        public const string TestDocumentsRepository = "TestDocumentsRepository";

        /// <summary>
        /// Set up a SQL Database for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        [BeforeFeature("@setupTenantedSqlDatabase", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static void SetupDatabases(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantSqlConnectionFactory sqlConnectionFactory = serviceProvider.GetRequiredService<ITenantSqlConnectionFactory>();
            ITenantCosmosContainerFactory factory = serviceProvider.GetRequiredService<ITenantCosmosContainerFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            SqlConfiguration config = tenantProvider.Root.GetDefaultSqlConfiguration();

            string containerBase = Guid.NewGuid().ToString();

            config.ConnectionString = "Server=(localdb)\\mssqllocaldb;Trusted_Connection=True;MultipleActiveResultSets=true";
            config.Database = $"workflow-{containerBase}";
            config.ConnectionStringSecretName = null;
            config.KeyVaultName = null;
            config.IsLocalDatabase = true;
            config.DisableTenantIdPrefix = true;
            tenantProvider.Root.SetDefaultSqlConfiguration(config);

            CosmosConfiguration cosmosConfig = tenantProvider.Root.GetDefaultCosmosConfiguration();
            cosmosConfig.DatabaseName = "endjinspecssharedthroughput";
            cosmosConfig.DisableTenantIdPrefix = true;
            tenantProvider.Root.SetDefaultCosmosConfiguration(cosmosConfig);

            Container testDocumentsRepository = WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                () => factory.GetContainerForTenantAsync(
                    tenantProvider.Root,
                    new CosmosContainerDefinition("workflow", $"{containerBase}testdocuments", "/id"))).Result;

            featureContext.Set(testDocumentsRepository, TestDocumentsRepository);

            // And now, deploy the sql server for this instance.
#if DEBUG
            const string BUILD = "debug";
#else
            const string BUILD = "release";
#endif

            SqlHelpers.SetupDatabaseFromDacPac(config.ConnectionString, config.Database, @$"..\..\..\..\Marain.Workflow.Storage.Sql.Database\bin\{BUILD}\Marain.Workflow.Storage.Sql.Database.dacpac");
        }

        /// <summary>
        /// Tear down the cosmos DB repository for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AfterFeature("@setupTenantedSqlDatabase", Order = 100000)]
        public static async Task TeardownDatabases(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantSqlConnectionFactory sqlConnectionFactory = serviceProvider.GetRequiredService<ITenantSqlConnectionFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            SqlConfiguration config = tenantProvider.Root.GetDefaultSqlConfiguration();

            featureContext.RunAndStoreExceptions(() =>
                SqlHelpers.DeleteDatabase(config.ConnectionString, config.Database));

            await featureContext.RunAndStoreExceptionsAsync(
                () => featureContext.Get<Container>(TestDocumentsRepository).DeleteContainerAsync()).ConfigureAwait(false);
        }
    }
}