// <copyright file="CauseExternalInvocationTrigger.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects.Triggers
{
    using System.Collections.Generic;

    public class CauseExternalInvocationTrigger : IWorkflowTrigger
    {
        public const string RegisteredContentType = "application/vnd.endjin.datacatalog.causeexternalinvocationtrigger";

        public string Id { get; set; }

        public string ContentType => RegisteredContentType;

        public string PartitionKey => this.Id;

        public string OtherData { get; set; }

        public IEnumerable<string> GetSubjects() => new[] { this.Id };
    }
}