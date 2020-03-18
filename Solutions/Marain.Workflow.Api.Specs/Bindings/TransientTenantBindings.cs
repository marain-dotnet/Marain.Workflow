// <copyright file="TransientTenantBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.ContentManagement.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
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
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// The newly created tenant is added to the <see cref="FeatureContext"/>. Access it via the helper methods
        /// <see cref="GetTransientTenant(FeatureContext)"/> or <see cref="GetTransientTenantId(FeatureContext)"/>.
        /// </remarks>
        [BeforeFeature("@useTransientTenant", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task SetupTransientTenant(FeatureContext context)
        {
            // This needs to run after the ServiceProvider has been constructed
            IServiceProvider provider = ContainerBindings.GetServiceProvider(context);
            ITenantProvider tenantProvider = provider.GetRequiredService<ITenantProvider>();

            // In order to ensure the Cosmos aspects of the Tenancy setup are fully configured, we need to resolve
            // the ITenantCosmosContainerFactory, which triggers setting default config to the root tenant.
            // HACK: This is a hack until we can come up with a better way of handling deferred initialisation.
            provider.GetRequiredService<ITenantCosmosContainerFactory>();

            IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
            string parentTenantId = configuration["TransientTenantBindings:ParentTenantId"];

            if (string.IsNullOrEmpty(parentTenantId))
            {
                throw new Exception("In order to use Transient Tenant Bindings, your configuration must contain a value for setting 'TransientTenantBindings:ParentTenantId'.");
            }

            string transientTenantName = $"Marain.Workflow.Api.Specs test run for feature '{context.FeatureInfo.Title}' on {DateTimeOffset.UtcNow.ToString("f")}";

            Console.WriteLine($"Creating transient tenant as child of '{parentTenantId}' for feature '{context.FeatureInfo.Title}'");

            ITenant transientTenant = await tenantProvider.CreateChildTenantAsync(parentTenantId).ConfigureAwait(false);

            Console.WriteLine($"Adding name and default Cosmos configuration to transient tenant with Id '{transientTenant.Id}'");

            transientTenant.Properties.Set("name", transientTenantName);

            CosmosConfiguration cosmosConfiguration = tenantProvider.Root.GetDefaultCosmosConfiguration() ?? new CosmosConfiguration();
            cosmosConfiguration.DatabaseName = "endjinspecssharedthroughput";
            cosmosConfiguration.DisableTenantIdPrefix = true;
            transientTenant.SetDefaultCosmosConfiguration(cosmosConfiguration);

            Console.WriteLine($"Updating transient tenant with Id '{transientTenant.Id}'");

            await tenantProvider.UpdateTenantAsync(transientTenant).ConfigureAwait(false);

            context.Set(transientTenant);

            Console.WriteLine("Transient tenant setup complete");
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
                Console.WriteLine($"Tearing down transient tenant for feature '{context.FeatureInfo.Title}'");

                IServiceProvider provider = ContainerBindings.GetServiceProvider(context);
                ITenantProvider tenantProvider = provider.GetRequiredService<ITenantProvider>();

                if (context.TryGetValue<ITenant>(out ITenant tenant))
                {
                    return tenantProvider.DeleteTenantAsync(tenant.Id);
                }

                Console.WriteLine("No transient tenant has been created for this feature.");
                return Task.CompletedTask;
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
    }
}
