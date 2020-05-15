// <copyright file="RetryOnCosmosRequestRateExceededPolicy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
    using System;
    using System.Net;
    using Corvus.Retry.Policies;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// <see cref="IRetryPolicy"/> which retries on a Cosmos 429 response.
    /// </summary>
    public class RetryOnCosmosRequestRateExceededPolicy : IRetryPolicy
    {
        /// <summary>
        /// Gets the shared instance of the <see cref="RetryOnCosmosRequestRateExceededPolicy"/>.
        /// </summary>
        public static RetryOnCosmosRequestRateExceededPolicy Instance { get; } = new RetryOnCosmosRequestRateExceededPolicy();

        /// <inheritdoc/>
        public bool CanRetry(Exception exception) => exception is CosmosException cex && cex.StatusCode == HttpStatusCode.TooManyRequests;
    }
}
