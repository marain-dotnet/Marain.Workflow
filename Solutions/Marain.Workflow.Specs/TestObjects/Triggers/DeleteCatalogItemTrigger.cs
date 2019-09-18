
// <copyright file="DeleteCatalogItemTrigger.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Triggers
{
    using System;
    using System.Collections.Generic;

    public class DeleteCatalogItemTrigger : ICatalogItemTrigger
    {
        public const string RegisteredContentType = "application/vnd.marain.datacatalog.deletecatalogitemtrigger";

        public string CatalogItemId { get; set; }

        public string ContentType => RegisteredContentType;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string PartitionKey => this.CatalogItemId;

        public IEnumerable<string> GetSubjects() => new[] { this.CatalogItemId };
    }
}

#pragma warning restore