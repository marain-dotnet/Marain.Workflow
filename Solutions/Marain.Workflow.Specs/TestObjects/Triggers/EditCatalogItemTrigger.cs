// <copyright file="EditCatalogItemTrigger.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Triggers
{
    using System;
    using System.Collections.Generic;

    using Marain.Workflows.Specs.TestObjects.Subjects;

    public class EditCatalogItemTrigger : ICatalogItemTrigger
    {
        public const string RegisteredContentType = "application/vnd.endjin.datacatalog.editcatalogitemtrigger";

        public string CatalogItemId => this.PatchDetails.Id;

        public string ContentType => RegisteredContentType;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public CatalogItemPatch PatchDetails { get; set; }

        public string PartitionKey => this.PatchDetails.Id;

        public IEnumerable<string> GetSubjects() => new[] { this.PatchDetails.Id };
    }
}

#pragma warning restore