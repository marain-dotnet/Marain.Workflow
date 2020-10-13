// <copyright file="CloudEventPublisherException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1194 // Implement exception constructors.
namespace Marain.Workflows.CloudEvents
{
    using System;
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// Exception thrown when publish a CloudEvent to an Http endpoint fails.
    /// </summary>
    [Serializable]
    public class CloudEventPublisherException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudEventPublisherException"/> class.
        /// </summary>
        /// <param name="subject">The subject of the CloudEvent that could not be published.</param>
        /// <param name="source">The source of the CloudEvent that could not be published.</param>
        /// <param name="externalUrl">The Url that the CloudEvent was being published to.</param>
        /// <param name="statusCode">The <see cref="HttpResponseMessage.StatusCode"/>.</param>
        /// <param name="reasonPhrase">The <see cref="HttpResponseMessage.ReasonPhrase"/>.</param>
        public CloudEventPublisherException(string subject, string source, string externalUrl, HttpStatusCode statusCode, string reasonPhrase)
            : base($"Failed to publish event with subject '{subject}', source '{source}' to '{externalUrl}'. Received status code '{statusCode}' and reason '{reasonPhrase}'")
        {
        }
    }
}
