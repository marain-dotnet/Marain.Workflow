// <copyright file="CreateCatalogItemTrigger.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable SA1600, CS1591

namespace Marain.Workflows.Specs.TestObjects.Triggers
{
    using System.Collections.Generic;

    public class CauseExternalInvocationTrigger : IWorkflowTrigger
    {
        public const string RegisteredContentType = "application/vnd.marain.datacatalog.causeexternalinvocationtrigger";

        public string Id { get; set; }

        public string ContentType => RegisteredContentType;

        public string PartitionKey => this.Id;

        public IEnumerable<string> GetSubjects() => new[] { this.Id };

        public string OtherData { get; set; }
    }
}
