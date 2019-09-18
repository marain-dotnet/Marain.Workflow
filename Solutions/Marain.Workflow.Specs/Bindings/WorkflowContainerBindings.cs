// <copyright file="WorkflowContainerBindings.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using Corvus.SpecFlow.Extensions;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Leasing;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using TechTalk.SpecFlow;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using Corvus.Tenancy;

    /// <summary>
    ///     Container related bindings to configure the service provider for features.
    /// </summary>
    [Binding]
    public static class WorkflowContainerBindings
    {
        /// <summary>
        ///     Initializes the container before each feature's tests are run.
        /// </summary>
        /// <param name="featureContext">
        ///     The feature context.
        /// </param>
        [BeforeFeature("@setupContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void InitializeContainer(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    serviceCollection.AddLogging(
                        b =>
                        {
                            b.AddConsole();
                            b.AddDebug();
                        });

                    var configData = new Dictionary<string, string>
                    {
                        //// { "STORAGEACCOUNTCONNECTIONSTRING", "UseDevelopmentStorage=true" },
                    };
                    IConfigurationRoot config = new ConfigurationBuilder()
                        .AddInMemoryCollection(configData)
                        .AddEnvironmentVariables()
                        .AddJsonFile("local.settings.json", true, true)
                        .Build();

                    serviceCollection.AddContentSerialization();
                    serviceCollection.AddSingleton(config);
                    serviceCollection.AddSingleton<ITenantProvider, FakeTenantProvider>();
                    serviceCollection.AddSharedThroughputCosmosDbTestServices("/partitionKey");
                    serviceCollection.AddCosmosClientExtensions();
                    serviceCollection.AddTenantCosmosContainerFactory(config);
                    serviceCollection.AddInMemoryWorkflowTriggerQueue();
                    serviceCollection.AddInMemoryLeasing();
                    serviceCollection.RegisterCoreWorkflowContentTypes();
                    serviceCollection.RegisterTestContentTypes();

                    serviceCollection.AddSingleton<IWorkflowEngineFactory>(s => new FeatureContextWorkflowEngineFactory(featureContext, s.GetRequiredService<ILeaseProvider>(), s.GetRequiredService<ILogger<IWorkflowEngine>>()));
                    serviceCollection.AddSingleton(new DataCatalogItemRepositoryFactory(featureContext));

                    serviceCollection.AddSingleton<IServiceIdentityTokenSource, FakeServiceIdentityTokenSource>();
                });
        }
    }
}