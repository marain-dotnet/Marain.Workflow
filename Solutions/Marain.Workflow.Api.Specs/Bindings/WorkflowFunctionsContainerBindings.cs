// <copyright file="WorkflowFunctionsContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.SpecFlow.Bindings
{
    using System;
    using System.IO;

    using Corvus.Leasing;
    using Microsoft.Azure.Cosmos;
    using Marain.Workflows;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using TechTalk.SpecFlow;
    using Corvus.SpecFlow.Extensions;

    /// <summary>
    /// Provides Specflow bindings for Endjin Composition
    /// </summary>
    [Binding]
    public static class WorkflowFunctionsContainerBindings
    {
        /// <summary>
        /// Setup the endjin container for a feature
        /// </summary>
        /// <param name="featureContext">The feature context for the current feature</param>
        /// <remarks>We expect features run in parallel to be executing in separate app domains</remarks>
        [BeforeFeature("@setupContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeature(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    ////IConfigurationRoot configurationRoot = serviceCollection.AddTestConfiguration(null);
                    ////serviceCollection.AddEndjinJsonConverters();
                    ////serviceCollection.AddLogging();
                    ////serviceCollection.AddRepositoryJsonConverters();
                    ////serviceCollection.SetRootTenantDefaultRepositoryConfiguration(configurationRoot);
                    ////serviceCollection.AddTenantKeyVaultOrConfigurationAccountKeyProvider();
                    ////serviceCollection.AddWorkflowEngineFactory();
                    ////serviceCollection.RegisterCoreWorkflowContentTypes();
                    ////serviceCollection.AddAzureLeasing(c => c.ConnectionStringKey = "LeasingStorageAccountConnectionString");
                });
        }
    }
}
