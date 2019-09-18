// <copyright file="CatalogItemPatchExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Subjects
{
    using System;

    public static class CatalogItemPatchExtensions
    {
        public static CatalogItem ApplyTo(this CatalogItemPatch patch, CatalogItem original)
        {
            if (patch.Id != original.Id)
            {
                throw new InvalidOperationException();
            }

            return new CatalogItem
                       {
                           Id = original.Id,
                           Type = original.Type,
                           Identifier = original.Identifier,
                           Description = patch.Description,
                           Notes = patch.Notes
                       };
        }
    }
}

#pragma warning restore