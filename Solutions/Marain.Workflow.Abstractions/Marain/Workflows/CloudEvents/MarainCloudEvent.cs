// <copyright file="MarainCloudEvent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.CloudEvents
{
    using System;
    using NodaTime;
    using NodaTime.Text;

    /// <summary>
    /// A basic representation of a CloudEvent.
    /// </summary>
    public class MarainCloudEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarainCloudEvent"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Id" />.</param>
        /// <param name="source">The <see cref="Source" />.</param>
        /// <param name="subject">The <see cref="Subject" />.</param>
        /// <param name="time">The <see cref="Time" />.</param>
        /// <param name="typeSuffix">The <see cref="TypeSuffix" />.</param>
        /// <param name="marainTenantId">The <see cref="MarainTenantId" />.</param>
        /// <param name="dataContentType">The <see cref="DataContentType" />.</param>
        /// <param name="data">The <see cref="Data" />.</param>
        public MarainCloudEvent(
            string id,
            string source,
            string subject,
            DateTimeOffset? time,
            string typeSuffix,
            string marainTenantId,
            string dataContentType,
            object data)
        {
            this.Id = id;
            this.Source = source;
            this.Subject = subject;
            this.Time = time ?? DateTimeOffset.UtcNow;
            this.TypeSuffix = typeSuffix;
            this.DataContentType = dataContentType;
            this.MarainTenantId = marainTenantId;
            this.Data = data;
        }

        /// <summary>
        /// Gets the Id of the CloudEvent.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the CloudEvent source - https://github.com/cloudevents/spec/blob/v1.0/spec.md#source.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the CloudEvent spec version.
        /// </summary>
        public string SpecVersion { get; } = "1.0";

        /// <summary>
        /// Gets the CloudEvent subject - https://github.com/cloudevents/spec/blob/v1.0/spec.md#subject.
        /// </summary>
        /// <remarks>This should contain the Marain tenant Id. TODO: Improve this?.</remarks>
        public string Subject { get; }

        /// <summary>
        /// Gets the event time, in ISO8601 format.
        /// </summary>
        /// <remarks>
        /// Defaults to the current UTC date/time.
        /// </remarks>
        public DateTimeOffset Time { get; }

        /// <summary>
        /// Gets the CloudEvent event type - https://github.com/cloudevents/spec/blob/v1.0/spec.md#type.
        /// </summary>
        /// <remarks>
        /// Constructed by concatenating "io.marain." with <see cref="TypeSuffix"/>.
        /// </remarks>
        public string Type => "io.marain." + this.TypeSuffix;

        /// <summary>
        /// Gets the Marain-specific type suffix that will be used to construct <see cref="Type"/>.
        /// </summary>
        public string TypeSuffix { get; }

        /// <summary>
        /// Gets the datacontenttype of the payload - https://github.com/cloudevents/spec/blob/v1.0/spec.md#datacontenttype.
        /// </summary>
        public string DataContentType { get; }

        /// <summary>
        /// Gets the CloudEvent payload - https://github.com/cloudevents/spec/blob/v1.0/spec.md#event-data.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// Gets the Marain Tenant Id that this event belongs to.
        /// </summary>
        public string MarainTenantId { get; }
    }
}
