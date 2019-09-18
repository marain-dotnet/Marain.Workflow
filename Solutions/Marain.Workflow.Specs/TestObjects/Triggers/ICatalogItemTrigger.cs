// <copyright file="ICatalogItemTrigger.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
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