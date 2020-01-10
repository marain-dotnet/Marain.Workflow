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
        [BeforeFeature("@perFeatureContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeature(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                services =>
                {
                    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                        .AddEnvironmentVariables()
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

                    IConfigurationRoot root = configurationBuilder.Build();

                    services.AddSingleton(root);

                    services.AddJsonSerializerSettings();
                    services.AddLogging();

                    services.AddTenantProviderBlobStore();
                    services.AddTenantCloudBlobContainerFactory(root);

                    services.AddTenantCosmosContainerFactory(root);

                    services.AddWorkflowEngineFactory();
                    services.RegisterCoreWorkflowContentTypes();
                    services.AddAzureLeasing(c => c.ConnectionStringKey = "LeasingStorageAccountConnectionString");
                });
        }
    }
}
