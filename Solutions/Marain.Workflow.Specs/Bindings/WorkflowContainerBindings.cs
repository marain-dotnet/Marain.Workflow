// <copyright file="WorkflowContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Leasing;
    using Corvus.SpecFlow.Extensions;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Container related bindings to configure the service provider for features.
    /// </summary>
    [Binding]
    public static class WorkflowContainerBindings
    {
        /// <summary>
        /// Initializes the container before each feature's tests are run.
        /// </summary>
        /// <param name="featureContext">
        /// The feature context.
        /// </param>
        [BeforeFeature("@perFeatureContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void InitializeContainer(FeatureContext featureContext)
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

                    services.AddLogging();

                    services.AddJsonSerializerSettings();

                    services.AddTenantCloudBlobContainerFactory(root);
                    services.AddTenantProviderBlobStore();
                    services.AddTenantCosmosContainerFactory(root);

                    services.AddInMemoryWorkflowTriggerQueue();
                    services.AddInMemoryLeasing();

                    services.RegisterCoreWorkflowContentTypes();
                    services.AddContent(factory => factory.RegisterTestContentTypes());

                    services.AddSingleton<IWorkflowEngineFactory>(s => new FeatureContextWorkflowEngineFactory(
                        featureContext,
                        s.GetRequiredService<ILeaseProvider>(),
                        s.GetRequiredService<ILogger<IWorkflowEngine>>()));

                    services.AddSingleton<DataCatalogItemRepositoryFactory>();

                    services.AddSingleton<IServiceIdentityTokenSource, FakeServiceIdentityTokenSource>();
                });
        }
    }
}