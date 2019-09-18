// <copyright file="CatalogItemExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Subjects
{
    public static class CatalogItemExtensions
    {
        public static bool IsComplete(this CatalogItem item)
        {
            return !(string.IsNullOrEmpty(item.Description) || string.IsNullOrEmpty(item.Notes));
        }
    }
}

#pragma warning restore