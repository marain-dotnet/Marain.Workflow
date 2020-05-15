// <copyright file="WorkflowSqlBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Sql.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Marain.Workflows.Internal;
    using Marain.Workflows.Specs.Steps;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Configuration;
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@setupTenantedSqlDatabase", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task SetupDatabases(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantSqlConnectionFactory sqlConnectionFactory = serviceProvider.GetRequiredService<ITenantSqlConnectionFactory>();
            ITenantCosmosContainerFactory factory = serviceProvider.GetRequiredService<ITenantCosmosContainerFactory>();
            ITenantStore tenantStore = serviceProvider.GetRequiredService<ITenantStore>();
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            SqlConfiguration sqlConfig =
                configuration.GetSection("TestSqlConfiguration").Get<SqlConfiguration>()
                ?? new SqlConfiguration();

            string containerBase = Guid.NewGuid().ToString();

            ITenant testTenant = await tenantStore.CreateChildTenantAsync(tenantStore.Root.Id, "Test tenant").ConfigureAwait(false);
            sqlConfig.ConnectionString = "Server=(localdb)\\mssqllocaldb;Trusted_Connection=True;MultipleActiveResultSets=true";
            sqlConfig.Database = $"workflow-{containerBase}";
            sqlConfig.ConnectionStringSecretName = null;
            sqlConfig.KeyVaultName = null;
            sqlConfig.IsLocalDatabase = true;
            sqlConfig.DisableTenantIdPrefix = true;

            await tenantStore.UpdateTenantPropertiesAsync(testTenant, data => data.AddSqlConfiguration(
                TenantedSqlWorkflowStoreServiceCollectionExtensions.WorkflowConnectionDefinition,
                sqlConfig)).ConfigureAwait(false);

            CosmosConfiguration cosmosConfig =
                configuration.GetSection("TestCosmosConfiguration").Get<CosmosConfiguration>()
                ?? new CosmosConfiguration();

            cosmosConfig.DatabaseName = "endjinspecssharedthroughput";
            cosmosConfig.DisableTenantIdPrefix = true;

            testTenant = await tenantStore.UpdateTenantPropertiesAsync(testTenant, data => data.AddCosmosConfiguration(
                TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowStoreContainerDefinition,
                cosmosConfig)).ConfigureAwait(false);

            testTenant = await tenantStore.UpdateTenantPropertiesAsync(testTenant, data => data.AddCosmosConfiguration(
                TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowInstanceStoreContainerDefinition,
                cosmosConfig)).ConfigureAwait(false);

            testTenant = await tenantStore.UpdateTenantPropertiesAsync(testTenant, data => data.AddCosmosConfiguration(
                TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowInstanceChangeLogContainerDefinition,
                cosmosConfig)).ConfigureAwait(false);

            var testDocumentRepositoryContainerDefinition = new CosmosContainerDefinition("workflow", "testdocuments", "/id");
            testTenant = await tenantStore.UpdateTenantPropertiesAsync(testTenant, data => data.AddCosmosConfiguration(
                testDocumentRepositoryContainerDefinition,
                cosmosConfig)).ConfigureAwait(false);

            Container testDocumentsRepository = await WorkflowRetryHelper.ExecuteWithRetryRulesAsync(
                () => factory.GetContainerForTenantAsync(
                    testTenant,
                    testDocumentRepositoryContainerDefinition)).ConfigureAwait(false);

            featureContext.Set(testTenant);
            featureContext.Set(testDocumentsRepository, TestDocumentsRepository);

            // And now, deploy the sql server for this instance.
#if DEBUG
            const string BUILD = "debug";
#else
            const string BUILD = "release";
#endif

            SqlHelpers.SetupDatabaseFromDacPac(sqlConfig.ConnectionString, sqlConfig.Database, @$"..\..\..\..\Marain.Workflow.Storage.Sql.Database\bin\{BUILD}\Marain.Workflow.Storage.Sql.Database.dacpac");
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

            ITenant tenant = featureContext.Get<ITenant>();
            SqlConfiguration config = tenant.GetSqlConfiguration(TenantedSqlWorkflowStoreServiceCollectionExtensions.WorkflowConnectionDefinition);

            featureContext.RunAndStoreExceptions(() =>
                SqlHelpers.DeleteDatabase(config.ConnectionString, config.Database));

            await featureContext.RunAndStoreExceptionsAsync(
                () => featureContext.Get<Container>(TestDocumentsRepository).DeleteContainerAsync()).ConfigureAwait(false);
        }
    }
}