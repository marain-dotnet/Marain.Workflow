// <copyright file="WorkflowSqlBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Sql.Tenancy;
    using Corvus.Tenancy;
    using Marain.Workflows.Specs.Steps;
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
            _ = serviceProvider.GetRequiredService<ITenantSqlConnectionFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            SqlConfiguration config = tenantProvider.Root.GetDefaultSqlConfiguration();
            config.DisableTenantIdPrefix = true;

            tenantProvider.Root.SetDefaultSqlConfiguration(config);
        }

        /// <summary>
        /// Tear down the cosmos DB repository for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        [AfterFeature("@setupTenantedSqlDatabase", Order = 100000)]
        public static void TeardownDatabases(FeatureContext featureContext)
        {
        }
    }
}