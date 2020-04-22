// <copyright file="TransientTenantBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Services;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Bindings to manage creation and deletion of tenants for test features.
    /// </summary>
    [Binding]
    public static class TransientTenantBindings
    {
        /// <summary>
        /// Creates a new <see cref="ITenant"/> for the current feature, adding a test <see cref="CosmosConfiguration"/>
        /// to the tenant data.
        /// </summary>
        /// <param name="featureContext">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// The newly created tenant is added to the <see cref="FeatureContext"/>. Access it via the helper methods
        /// <see cref="GetTransientTenant(FeatureContext)"/> or <see cref="GetTransientTenantId(FeatureContext)"/>.
        /// </remarks>
        [BeforeFeature("@useTransientTenant", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task SetupTransientTenant(FeatureContext featureContext)
        {
            var transientTenantManager = TransientTenantManager.GetInstance(featureContext);
            await transientTenantManager.EnsureInitialised().ConfigureAwait(false);

            // Create a transient service tenant for testing purposes.
            ITenant transientServiceTenant = await transientTenantManager.CreateTransientServiceTenantFromEmbeddedResourceAsync(
                typeof(TransientTenantBindings).Assembly,
                $"Marain.Workflow.Api.SpecsServiceManifests.WorkflowServiceManifest.jsonc").ConfigureAwait(false);

            // Now update the service Id in our configuration and in the function configuration
            UpdateServiceConfigurationWithTransientTenantId(featureContext, transientServiceTenant);

            // Now we need to construct a transient client tenant for the test, and enroll it in the new
            // transient service.
            ITenant transientClientTenant = await transientTenantManager.CreateTransientClientTenantAsync().ConfigureAwait(false);

            await transientTenantManager.AddEnrollmentAsync(
                transientClientTenant.Id,
                transientServiceTenant.Id,
                GetWorkflowConfig(featureContext)).ConfigureAwait(false);
        }

        /// <summary>
        /// Tears down the transient tenant created for the current feature.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AfterFeature("@useTransientTenant")]
        public static Task TearDownTransientTenant(FeatureContext context)
        {
            return context.RunAndStoreExceptionsAsync(() =>
            {
                IServiceProvider provider = ContainerBindings.GetServiceProvider(context);
                ITenantProvider tenantProvider = provider.GetRequiredService<ITenantProvider>();

                ITenant tenant = context.Get<ITenant>();
                return tenantProvider.DeleteTenantAsync(tenant.Id);
            });
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

            featureContext.AddFunctionConfigurationEnvironmentVariable(
                "MarainServiceConfiguration__ServiceTenantId",
                configuration.ServiceTenantId);

            featureContext.AddFunctionConfigurationEnvironmentVariable(
                "MarainServiceConfiguration__ServiceDisplayName",
                configuration.ServiceDisplayName);
        }

        private static EnrollmentConfigurationItem[] GetWorkflowConfig(FeatureContext featureContext)
        {
            IConfiguration configuration = ContainerBindings
                .GetServiceProvider(featureContext)
                .GetRequiredService<IConfiguration>();

            // Load the config items we need:
            CosmosConfiguration cosmosConfiguration =
                configuration.GetSection("TestTenantCosmosConfiguration").Get<CosmosConfiguration>();

            BlobStorageConfiguration storageConfiguration =
                configuration.GetSection("TestTenantBlobStorageConfiguration").Get<BlobStorageConfiguration>();

            return new EnrollmentConfigurationItem[]
            {
                new EnrollmentCosmosConfigurationItem
                {
                    Key = "workflowStore",
                    Configuration = cosmosConfiguration ?? new CosmosConfiguration(),
                },
                new EnrollmentCosmosConfigurationItem
                {
                    Key = "workflowInstanceStore",
                    Configuration = cosmosConfiguration ?? new CosmosConfiguration(),
                },
                new EnrollmentBlobStorageConfigurationItem
                {
                    Key = "operationsStore",
                    Configuration = storageConfiguration ?? new BlobStorageConfiguration(),
                },
            };
        }
    }
}
