// <copyright file="WorkflowPreconditionFailedException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Runtime.Serialization;

    /// <inheritdoc />
    [Serializable]
    public class WorkflowPreconditionFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowPreconditionFailedException"/> class.
        /// </summary>
        public WorkflowPreconditionFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowPreconditionFailedException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        public WorkflowPreconditionFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowPreconditionFailedException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="inner"> The inner exception. </param>
        public WorkflowPreconditionFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}