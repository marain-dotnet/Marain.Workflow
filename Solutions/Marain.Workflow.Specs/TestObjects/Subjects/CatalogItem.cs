// <copyright file="CatalogItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Subjects
{
    public class CatalogItem
    {
        public const string RegisteredContentType = "application/vnd.endjin.datacatalog.catalogitem";

        public string ContentType => RegisteredContentType;

        public string Description { get; set; }

        public string Id { get; set; }

        public string Identifier { get; set; }

        public string Notes { get; set; }

        public string Type { get; set; }
    }
}

#pragma warning restore