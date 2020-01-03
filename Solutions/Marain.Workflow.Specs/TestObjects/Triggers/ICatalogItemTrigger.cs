// <copyright file="ICatalogItemTrigger.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Triggers
{
    public interface ICatalogItemTrigger : IWorkflowTrigger
    {
        string CatalogItemId { get; }
    }
}

#pragma warning restore