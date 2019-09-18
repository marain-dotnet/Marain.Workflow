// <copyright file="TestStorageExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Microsoft.Extensions.DependencyInjection
{
    using Marain.Workflows;
    using Marain.Workflows.Specs.TestObjects;

    public static class TestStorageExtensions
    {
        public static IServiceCollection AddInMemoryWorkflowTriggerQueue(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IWorkflowMessageQueue, InMemoryWorkflowMessageQueue>();

            return serviceCollection;
        }
    }
}

#pragma warning restore