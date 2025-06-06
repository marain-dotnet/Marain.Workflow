// <copyright file="BindingSequence.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Bindings
{
    using Corvus.Testing.ReqnRoll;

    public static class BindingSequence
    {
        public const int TransientTenantSetup = ContainerBeforeFeatureOrder.ServiceProviderAvailable;

        public const int FunctionStartup = TransientTenantSetup + 1;
    }
}