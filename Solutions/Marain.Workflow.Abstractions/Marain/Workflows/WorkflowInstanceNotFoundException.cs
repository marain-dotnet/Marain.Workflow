// <copyright file="WorkflowInstanceNotFoundException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Runtime.Serialization;

    /// <inheritdoc />
    [Serializable]
    public class WorkflowInstanceNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceNotFoundException"/> class.
        /// </summary>
        public WorkflowInstanceNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceNotFoundException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        public WorkflowInstanceNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceNotFoundException"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="inner"> The inner exception. </param>
        public WorkflowInstanceNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}