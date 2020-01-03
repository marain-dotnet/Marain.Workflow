// <copyright file="OperationsControlClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Client
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// Workflow Public API client for use in scenarios where authentication is not required.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In scenarios in which inter-service communication is secured at a networking level, it
    /// might be unnecessary to authenticate requests. The base proxy type supports this but only
    /// through protected constructors. This type makes a suitable constructor available publicly.
    /// </para>
    /// </remarks>
    internal class UnauthenticatedWorkflowService : WorkflowService
    {
        /// <summary>
        /// Create an <see cref="UnauthenticatedWorkflowService"/>.
        /// </summary>
        /// <param name="baseUri">The base URI of the Workflow Engine service.</param>
        /// <param name="handlers">Optional request processing handlers.</param>
        public UnauthenticatedWorkflowService(Uri baseUri, params DelegatingHandler[] handlers)
            : base(baseUri, handlers)
        {
        }
    }
}
