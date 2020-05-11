// <copyright file="ContinuationToken.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage.Internal
{
    /// <summary>
    /// A continuation token for the SQL store.
    /// </summary>
    internal struct ContinuationToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuationToken"/> struct.
        /// </summary>
        /// <param name="pageSize">The page size.</param>
        /// <param name="pageIndex">The next page to retrieve.</param>
        /// <param name="startingSequenceNumber">The starting sequence number for the log, or null to start from the first available entry.</param>
        public ContinuationToken(int pageSize, int pageIndex, ulong? startingSequenceNumber = null)
        {
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.StartingSequenceNumber = startingSequenceNumber;
        }

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the index of the page.
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Gets the starting sequence number for the log, or null to start from the first available entry.
        /// </summary>
        public ulong? StartingSequenceNumber { get; }
    }
}
