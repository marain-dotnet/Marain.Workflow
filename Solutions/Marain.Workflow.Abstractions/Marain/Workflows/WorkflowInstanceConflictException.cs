// <copyright file="WorkflowInstanceConflictException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Runtime.Serialization;

    /// <inheritdoc />
    [Serializable]
    public class WorkflowInstanceConflictException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceConflictException"/> class.
        /// </summary>
        public WorkflowInstanceConflictException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceConflictException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        public WorkflowInstanceConflictException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceConflictException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="inner"> The inner exception. </param>
        public WorkflowInstanceConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceConflictException"/> class.
        /// </summary>
        /// <param name="info"> The serialization info. </param>
        /// <param name="context"> The context. </param>
        protected WorkflowInstanceConflictException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}