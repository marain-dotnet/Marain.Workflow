// <copyright file="CatalogItemPatch.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Subjects
{
    public class CatalogItemPatch
    {
        public const string RegisteredContentType = "application/vnd.endjin.datacatalog.catalogitempatchdetails";

        public string ContentType => RegisteredContentType;

        public string Description { get; set; }

        public string Id { get; set; }

        public string Notes { get; set; }
    }
}

#pragma warning restore