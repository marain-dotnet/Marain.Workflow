// <copyright file="CatalogItem.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Subjects
{
    public class CatalogItem
    {
        public const string RegisteredContentType = "application/vnd.marain.datacatalog.catalogitem";

        public string ContentType => RegisteredContentType;

        public string Description { get; set; }

        public string Id { get; set; }

        public string PartitionKey => this.Id;

        public string Identifier { get; set; }

        public string Notes { get; set; }

        public string Type { get; set; }
    }
}

#pragma warning restore