// <copyright file="WorkflowSerializationServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Corvus.ContentHandling;
    using Marain.Workflows;
    using Marain.Workflows.DomainEvents;

    /// <summary>
    /// Extension method for adding workflow objects tp the service
    /// collection so they can be serialized and deserialized by their
    /// content type.
    /// </summary>
    public static class WorkflowSerializationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the standard set of core workflow types, including standard
        /// actions and conditions, to the service collection.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection" /> to add the types to.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection" />.
        /// </returns>
        public static IServiceCollection RegisterCoreWorkflowContentTypes(this IServiceCollection services)
        {
            services.AddContent(factory =>
            {
                factory.RegisterTransientContent<LogAction>();
                factory.RegisterTransientContent<EntityIdTrigger>();
                factory.RegisterTransientContent<HostedWorkflowTrigger>();
                factory.RegisterTransientContent<HostedWorkflowTriggerNameCondition>();
                factory.RegisterTransientContent<HostedWorkflowTriggerParameterCondition>();
                factory.RegisterTransientContent<EntityIdCondition>();
                factory.RegisterTransientContent<TriggerContentTypeCondition>();
                factory.RegisterTransientContent<WorkflowMessageEnvelope>();
                factory.RegisterTransientContent<StartWorkflowInstanceRequest>();
                factory.RegisterTransientContent<InvokeExternalServiceAction>();
                factory.RegisterTransientContent<InvokeExternalServiceCondition>();
                factory.RegisterTransientContent<WorkflowTransition>();
                factory.RegisterTransientContent<WorkflowState>();
                factory.RegisterTransientContent<Workflow>();
                factory.RegisterTransientContent<ExternalServiceWorkflowRequest>();
                factory.RegisterTransientContent<ExternalServiceWorkflowResponse>();

                // Register all domain events
                Type domainEventType = typeof(DomainEvent);
                IEnumerable<Type> domainEventTypes = typeof(WorkflowSerializationServiceCollectionExtensions).Assembly
                    .GetExportedTypes()
                    .Where(x => domainEventType.IsAssignableFrom(x) && !x.IsAbstract);

                foreach (Type t in domainEventTypes)
                {
                    factory.RegisterTransientContent(t);
                }
            });

            return services;
        }
    }
}