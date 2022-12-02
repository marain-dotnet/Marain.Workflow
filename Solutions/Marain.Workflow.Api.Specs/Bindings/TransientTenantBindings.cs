// <copyright file="TransientTenantBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;

    using Corvus.Storage.Azure.BlobStorage;
    using Corvus.Storage.Azure.Cosmos;
    using Corvus.Tenancy;
    using Corvus.Testing.AzureFunctions;
    using Corvus.Testing.AzureFunctions.SpecFlow;
    using Corvus.Testing.SpecFlow;
    using Marain.Services;
    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;

    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Bindings to manage creation and deletion of tenants for test features.
    /// </summary>
    [Binding]
    public static class TransientTenantBindings
    {
        public const string OperationsStoreContainerNameKey = "TransientTenantBindings:OperationsStoreContainerName";
        private const string OperationsV1Id = "3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858";

        /// <summary>
        /// Creates a new <see cref="ITenant"/> for the current feature, adding test <see cref="CosmosContainerConfiguration"/>
        /// and <see cref="BlobContainerConfiguration"/> entries to the tenant data.
        /// </summary>
        /// <param name="featureContext">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// The newly created tenant is added to the <see cref="FeatureContext"/>. Access it via the helper methods
        /// <see cref="GetTransientTenant(FeatureContext)"/> or <see cref="GetTransientTenantId(FeatureContext)"/>.
        /// </remarks>
        [BeforeFeature("@useTransientTenant", Order = BindingSequence.TransientTenantSetup)]
        public static async Task SetupTransientTenant(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
            var transientTenantManager = TransientTenantManager.GetInstance(featureContext);
            await transientTenantManager.EnsureInitialised().ConfigureAwait(false);

            // Create a transient service tenant for testing purposes.
            ITenant transientServiceTenant = await transientTenantManager.CreateTransientServiceTenantFromEmbeddedResourceAsync(
                typeof(TransientTenantBindings).Assembly,
                "Marain.Workflows.Api.Specs.ServiceManifests.WorkflowServiceManifest.jsonc").ConfigureAwait(false);

            // Now update the service Id in our configuration and in the function configuration
            UpdateServiceConfigurationWithTransientTenantId(featureContext, transientServiceTenant);

            // Now we need to construct a transient client tenant for the test, and enroll it in the new
            // transient service.
            ITenant transientClientTenant = await transientTenantManager.CreateTransientClientTenantAsync().ConfigureAwait(false);

            EnrollmentConfigurationEntry enrollmentConfiguration = CreateWorkflowConfig(featureContext);
            var definitionsStoreConfig = (BlobContainerConfigurationItem)enrollmentConfiguration.ConfigurationItems[WorkflowAzureBlobTenancyPropertyKeys.Definitions];
            var instancesStoreConfig = (CosmosContainerConfigurationItem)enrollmentConfiguration.ConfigurationItems[WorkflowCosmosTenancyPropertyKeys.Instances];
            var operationsStoreConfig = (BlobContainerConfigurationItem)enrollmentConfiguration.Dependencies[OperationsV1Id].ConfigurationItems["Marain:Operations:BlobContainerConfiguration:Operations"];
            IBlobContainerSourceFromDynamicConfiguration blobContainerSource = serviceProvider.GetRequiredService<IBlobContainerSourceFromDynamicConfiguration>();
            BlobContainerClient definitionsContainer = await blobContainerSource.GetStorageContextAsync(definitionsStoreConfig.Configuration);
            ICosmosContainerSourceFromDynamicConfiguration cosmosContainerSource = serviceProvider.GetRequiredService<ICosmosContainerSourceFromDynamicConfiguration>();
            BlobContainerClient operationsContainer = await blobContainerSource.GetStorageContextAsync(operationsStoreConfig.Configuration);
            Container instancesContainer = await cosmosContainerSource.GetStorageContextAsync(instancesStoreConfig.Configuration);
            await definitionsContainer.CreateIfNotExistsAsync().ConfigureAwait(false);
            await instancesContainer.Database.CreateContainerIfNotExistsAsync(
                instancesContainer.Id,
                TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowInstanceStorePartitionKeyPath).ConfigureAwait(false);
            featureContext[OperationsStoreContainerNameKey] = operationsStoreConfig.Configuration;
            await operationsContainer.CreateIfNotExistsAsync().ConfigureAwait(false);

            await transientTenantManager.AddEnrollmentAsync(
                transientClientTenant.Id,
                transientServiceTenant.Id,
                enrollmentConfiguration).ConfigureAwait(false);

            // TODO: Temporary hack to work around the fact that the transient tenant manager no longer holds the latest
            // version of the tenants it's tracking; see https://github.com/marain-dotnet/Marain.TenantManagement/issues/28
            transientTenantManager.PrimaryTransientClient = await tenantProvider.GetTenantAsync(transientClientTenant.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the transient tenant created for the current feature from the supplied <see cref="FeatureContext"/>,
        /// or null if there is none.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>The <see cref="ITenant"/>.</returns>
        public static ITenant GetTransientTenant(this FeatureContext context)
        {
            context.TryGetValue(out ITenant result);
            return result;
        }

        /// <summary>
        /// Retrieves the Id of the transient tenant created for the current feature from the supplied feature context.
        /// <see cref="FeatureContext"/>.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>The Id of the <see cref="ITenant"/>.</returns>
        /// <exception cref="ArgumentNullException">There is no current tenant.</exception>
        public static string GetTransientTenantId(this FeatureContext context)
        {
            return context.GetTransientTenant().Id;
        }

        private static void UpdateServiceConfigurationWithTransientTenantId(
            FeatureContext featureContext,
            ITenant transientServiceTenant)
        {
            MarainServiceConfiguration configuration = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<MarainServiceConfiguration>();

            configuration.ServiceTenantId = transientServiceTenant.Id;
            configuration.ServiceDisplayName = transientServiceTenant.Name;

            FunctionConfiguration functionConfiguration = FunctionsBindings.GetFunctionConfiguration(featureContext);

            functionConfiguration.EnvironmentVariables.Add(
                "MarainServiceConfiguration:ServiceTenantId",
                configuration.ServiceTenantId);

            functionConfiguration.EnvironmentVariables.Add(
                "MarainServiceConfiguration:ServiceDisplayName",
                configuration.ServiceDisplayName);
        }

        private static EnrollmentConfigurationEntry CreateWorkflowConfig(FeatureContext featureContext)
        {
            // We need each test run to have a distinct container. We want these test-generated
            // containers to be easily recognized in storage accounts, so we don't just want to use
            // GUIDs.
            string testRunId = DateTime.Now.ToString("yyyy-MM-dd-hhmmssfff");

            IConfiguration configuration = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<IConfiguration>();

            // Load the config items we need:
            CosmosContainerConfiguration cosmosConfiguration =
                configuration.GetSection("TestCosmosConfiguration").Get<CosmosContainerConfiguration>()
                ?? new CosmosContainerConfiguration();

            cosmosConfiguration.Database = "endjinspecssharedthroughput";
            cosmosConfiguration.Container = $"specs-workflow-instances-{testRunId}";

            BlobContainerConfiguration definitionsStorageConfiguration =
                configuration.GetSection("TestBlobStorageConfiguration").Get<BlobContainerConfiguration>()
                ?? new BlobContainerConfiguration();
            definitionsStorageConfiguration.Container = $"specs-workflow-definitions-{testRunId}";

            BlobContainerConfiguration operationsStorageConfiguration =
                configuration.GetSection("TestBlobStorageConfiguration").Get<BlobContainerConfiguration>()
                ?? new BlobContainerConfiguration();
            operationsStorageConfiguration.Container = $"specs-workflow-operations-{testRunId}";

            return new EnrollmentConfigurationEntry(
                new Dictionary<string, ConfigurationItem>
                {
                    {
                        WorkflowAzureBlobTenancyPropertyKeys.Definitions,
                        new BlobContainerConfigurationItem
                        {
                            Configuration = definitionsStorageConfiguration,
                        }
                    },
                    {
                        WorkflowCosmosTenancyPropertyKeys.Instances,
                        new CosmosContainerConfigurationItem
                        {
                            Configuration = cosmosConfiguration,
                        }
                    },
                },
                new Dictionary<string, EnrollmentConfigurationEntry>
                {
                    {
                        OperationsV1Id,
                        new EnrollmentConfigurationEntry(
                            new Dictionary<string, ConfigurationItem>
                            {
                                {
                                    "Marain:Operations:BlobContainerConfiguration:Operations",
                                    new BlobContainerConfigurationItem
                                    {
                                        Configuration = definitionsStorageConfiguration,
                                    }
                                },
                            },
                            null)
                    },
                });
        }
    }
}