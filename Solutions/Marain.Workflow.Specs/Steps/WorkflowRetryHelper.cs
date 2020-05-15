// <copyright file="WorkflowRetryHelper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Retry;
    using Corvus.Retry.Strategies;

    /// <summary>
    /// Helper methods providing standard retry functionality for test steps.
    /// </summary>
    public static class WorkflowRetryHelper
    {
        /// <summary>
        /// Executes the supplied method with standard retry strategy and policy.
        /// </summary>
        /// <param name="asyncMethod">The method to execute.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task ExecuteWithStandardTestRetryRulesAsync(Func<Task> asyncMethod)
        {
            return Retriable.RetryAsync(
                asyncMethod,
                CancellationToken.None,
                new UseCosmosRetryAfterHeaderStrategy(TimeSpan.FromSeconds(15), 5),
                RetryOnCosmosRequestRateExceededPolicy.Instance,
                false);
        }

        /// <summary>
        /// Executes the supplied method with standard retry strategy and policy.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="asyncMethod">The method to execute.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task<T> ExecuteWithStandardTestRetryRulesAsync<T>(Func<Task<T>> asyncMethod)
        {
            return Retriable.RetryAsync(
                asyncMethod,
                CancellationToken.None,
                new UseCosmosRetryAfterHeaderStrategy(TimeSpan.FromSeconds(15), 5),
                RetryOnCosmosRequestRateExceededPolicy.Instance,
                false);
        }
    }
}
