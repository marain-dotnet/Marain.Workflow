// <copyright file="WorkflowConflictException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Runtime.Serialization;

    /// <inheritdoc />
    [Serializable]
    public class WorkflowConflictException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowConflictException"/> class.
        /// </summary>
        public WorkflowConflictException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowConflictException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        public WorkflowConflictException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowConflictException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="inner"> The inner exception. </param>
        public WorkflowConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}