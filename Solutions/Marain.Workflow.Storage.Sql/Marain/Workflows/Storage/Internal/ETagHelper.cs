// <copyright file="ETagHelper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage.Internal
{
    /// <summary>
    /// Helper methods for working with etags.
    /// </summary>
    public static class EtagHelper
    {
        /// <summary>
        /// Constructs a single etag value by combining the list of supplied values together. The resulting
        /// etag will be sensitive to the order of items in the list.
        /// </summary>
        /// <param name="discriminator">An arbitrary discriminator value that can be used to differentiate
        /// between different representations of items with the same underlying hash codes.</param>
        /// <param name="etags">The list of individual etags to combine.</param>
        /// <returns>A single combined etag.</returns>
        public static string BuildEtag(string discriminator, params string[] etags)
        {
            int hashCode = 160482331 * discriminator.GetHashCode();

            foreach (string current in etags)
            {
                hashCode = (hashCode * 179424319) + current.GetHashCode();
            }

            return string.Concat("\"", ((uint)hashCode).ToString(), "\"");
        }
    }
}
