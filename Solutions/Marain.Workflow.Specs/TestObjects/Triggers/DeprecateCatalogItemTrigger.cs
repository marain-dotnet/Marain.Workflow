﻿// <copyright file="DeprecateCatalogItemTrigger.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Marain.Workflows.Specs.TestObjects.Triggers
{
    using System;
    using System.Collections.Generic;

    public class DeprecateCatalogItemTrigger : ICatalogItemTrigger
    {
        public const string RegisteredContentType = "application/vnd.endjin.datacatalog.deprecatecatalogitemtrigger";

        public string CatalogItemId { get; set; }

        public string ContentType => RegisteredContentType;

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string PartitionKey => this.CatalogItemId;

        public IEnumerable<string> GetSubjects() => new[] { this.CatalogItemId };
    }
}

#pragma warning restore