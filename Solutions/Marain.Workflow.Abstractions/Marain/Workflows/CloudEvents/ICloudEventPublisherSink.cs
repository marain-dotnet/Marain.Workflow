// <copyright file="ICloudEventPublisherSink.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEvents
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A service which sends a CloudEvent to a specific type of recipient.
    /// </summary>
    public interface ICloudEventPublisherSink
    {
        /// <summary>
        /// Publishes the supplied CloudEvent.
        /// </summary>
        /// <param name="marainCloudEvent">The constructed CloudEvent.</param>
        /// <param name="directSubscriptions">A list of direct subscriptions. The sink is not required to make use of these; they support per workflow subscription configuration.</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Implementations are expected to implement appropriate retry logic for their target, and to only throw an
        /// exception once an appropriate number of retries have been attempted, or the supplied cancellation token
        /// has been used to request cancellation.
        /// </remarks>
        /// <exception cref="CloudEventPublisherException">
        /// Publishing the cloud event may have failed. For sinks which publish to multiple locations, this may mean
        /// partial success.
        /// </exception>
        Task PublishAsync(
            MarainCloudEvent marainCloudEvent,
            IEnumerable<WorkflowEventSubscription> directSubscriptions,
            CancellationToken cancellationToken);
    }
}
