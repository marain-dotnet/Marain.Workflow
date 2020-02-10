// <copyright file="WorkflowEngineClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Net.Http;
    using Marain.Workflow.Api.EngineHost.Client;

    /// <summary>
    /// Extension methods for configuring DI for the the Workflow Engine client services.
    /// </summary>
    public static class WorkflowEngineClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the workflow engine client to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">Config for the workflow engine client.</param>
        /// <returns>The service collection (for chaining).</returns>
        public static IServiceCollection AddTenantedWorkflowEngineClient(
            this IServiceCollection services,
            WorkflowEngineClientConfiguration configuration)
        {
            var client = new WorkflowEngineClient(new HttpClient())
            {
                BaseUrl = configuration.BaseUrl,
            };

            services.AddSingleton<IWorkflowEngineClient>(client);

            return services;
        }
    }
}
