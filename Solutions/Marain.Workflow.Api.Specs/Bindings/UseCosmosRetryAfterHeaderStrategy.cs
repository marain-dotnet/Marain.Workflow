// <copyright file="UseCosmosRetryAfterHeaderStrategy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
    using System;
    using Corvus.Retry.Strategies;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// <see cref="IRetryStrategy"/> which attempts to use the Retry-After value from Cosmos exceptions to set the interval.
    /// </summary>
    public class UseCosmosRetryAfterHeaderStrategy : RetryStrategy
    {
        private readonly TimeSpan defaultPeriodicity;
        private readonly int maxTries;
        private int tryCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="Linear"/> class.
        /// </summary>
        /// <param name="defaultPeriodicity">The default delay between retries, used if the Retry-After values is not
        /// present.</param>
        /// <param name="maxTries">The maximum number of retries.</param>
        public UseCosmosRetryAfterHeaderStrategy(TimeSpan defaultPeriodicity, int maxTries)
        {
            if (maxTries <= 0)
            {
                throw new ArgumentException("Max tries must be > 0", nameof(maxTries));
            }

            this.defaultPeriodicity = defaultPeriodicity;
            this.maxTries = maxTries;
        }

        /// <inheritdoc/>
        public override bool CanRetry
        {
            get
            {
                return this.tryCount < this.maxTries;
            }
        }

        /// <inheritdoc/>
        public override TimeSpan PrepareToRetry(Exception lastException)
        {
            if (lastException is null)
            {
                throw new ArgumentNullException(nameof(lastException));
            }

            this.AddException(lastException);

            this.tryCount++;

            return lastException is CosmosException cex && cex.RetryAfter.HasValue
                ? cex.RetryAfter.Value
                : this.defaultPeriodicity;
        }
    }
}
