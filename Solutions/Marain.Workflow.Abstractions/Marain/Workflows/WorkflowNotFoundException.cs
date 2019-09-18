// <copyright file="WorkflowNotFoundException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Runtime.Serialization;

    /// <inheritdoc />
    [Serializable]
    public class WorkflowNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowNotFoundException"/> class.
        /// </summary>
        public WorkflowNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowNotFoundException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        public WorkflowNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowNotFoundException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="inner"> The inner exception. </param>
        public WorkflowNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowNotFoundException"/> class.
        /// </summary>
        /// <param name="info"> The serialization info. </param>
        /// <param name="context"> The context. </param>
        protected WorkflowNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}