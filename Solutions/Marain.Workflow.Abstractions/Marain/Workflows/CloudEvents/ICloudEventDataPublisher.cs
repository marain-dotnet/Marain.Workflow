// <copyright file="ICloudEventDataPublisher.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEvents
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for classes that publish cloud events.
    /// </summary>
    public interface ICloudEventDataPublisher
    {
        /// <summary>
        /// Publishes the supplied event data as a cloud event (https://github.com/cloudevents/spec) to the list of subscriptions.
        /// </summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="source">The CloudEvent source - https://github.com/cloudevents/spec/blob/v1.0/spec.md#source.</param>
        /// <param name="workflowEventType">The CloudEvent event type - https://github.com/cloudevents/spec/blob/v1.0/spec.md#type. This will be prefixed with "io.marain." when creating the CloudEvent.</param>
        /// <param name="subject">The CloudEvent subject - https://github.com/cloudevents/spec/blob/v1.0/spec.md#subject.</param>
        /// <param name="dataContentType">The CloudEvent datacontenttype - https://github.com/cloudevents/spec/blob/v1.0/spec.md#datacontenttype.</param>
        /// <param name="eventData">The CloudEvent payload - https://github.com/cloudevents/spec/blob/v1.0/spec.md#event-data.</param>
        /// <param name="subscriptions">The list of subscriptions to publish the event to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PublishWorkflowEventDataAsync<T>(
            string source,
            string workflowEventType,
            string subject,
            string dataContentType,
            T eventData,
            params ExternalCloudEventSubscription[] subscriptions);
    }
}
