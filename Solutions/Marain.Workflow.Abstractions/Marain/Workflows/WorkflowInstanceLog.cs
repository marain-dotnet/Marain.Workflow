// <copyright file="WorkflowInstanceLog.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A page of entries from the workflow instance log.
    /// </summary>
    public class WorkflowInstanceLog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceLog"/> class.
        /// </summary>
        /// <param name="continuationToken">The continuation token for the next page of the log.</param>
        /// <param name="entries">The entries in this page of the log.</param>
        public WorkflowInstanceLog(string continuationToken, IEnumerable<WorkflowInstanceLogEntry> entries)
        {
            if (string.IsNullOrEmpty(continuationToken))
            {
                throw new System.ArgumentException("message", nameof(continuationToken));
            }

            this.ContinuationToken = continuationToken;
            this.Entries = (entries ?? throw new System.ArgumentNullException(nameof(entries))).ToList();
        }

        /// <summary>
        /// Gets the continuation token for the next page of results in the log.
        /// </summary>
        public string ContinuationToken { get; }

        /// <summary>
        /// Gets the entries in the log. They will be ordered by the oldest entry first.
        /// </summary>
        public IReadOnlyList<WorkflowInstanceLogEntry> Entries { get; }
    }
}
