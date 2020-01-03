﻿// <copyright file="WorkflowSerializationServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Corvus.ContentHandling;
    using Marain.Workflows;

    /// <summary>
    ///     Extension method for adding workflow objects tp the service
    ///     collection so they can be serialized and deserialized by their
    ///     content type.
    /// </summary>
    public static class WorkflowSerializationServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the standard set of core workflow types, including standard
        ///     actions and conditions, to the service collection.
        /// </summary>
        /// <param name="factory">
        ///     The <see cref="IServiceCollection" /> to add the types to.
        /// </param>
        /// <returns>
        ///     The <see cref="IServiceCollection" />.
        /// </returns>
        public static ContentFactory RegisterCoreWorkflowContentTypes(this ContentFactory factory)
        {
            factory.RegisterTransientContent<LogAction>();
            factory.RegisterTransientContent<EntityIdTrigger>();
            factory.RegisterTransientContent<HostedWorkflowTrigger>();
            factory.RegisterTransientContent<HostedWorkflowTriggerNameCondition>();
            factory.RegisterTransientContent<EntityIdCondition>();
            factory.RegisterTransientContent<TriggerContentTypeCondition>();
            factory.RegisterTransientContent<WorkflowMessageEnvelope>();
            factory.RegisterTransientContent<StartWorkflowInstanceRequest>();
            factory.RegisterTransientContent<InvokeExternalServiceAction>();
            factory.RegisterTransientContent<InvokeExternalServiceCondition>();

            return factory;
        }
    }
}