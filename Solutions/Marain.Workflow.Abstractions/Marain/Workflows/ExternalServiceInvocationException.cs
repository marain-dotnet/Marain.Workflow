// <copyright file="ExternalServiceInvocationException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Net;

    /// <summary>
    /// Exception to be thrown when there is a problem invoking an external action or condition.
    /// </summary>
    [Serializable]
    public class ExternalServiceInvocationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalServiceInvocationException"/> class.
        /// </summary>
        public ExternalServiceInvocationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalServiceInvocationException"/> class.
        /// </summary>
        /// <param name="message"><see cref="Exception.Message"/>.</param>
        public ExternalServiceInvocationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalServiceInvocationException"/> class.
        /// </summary>
        /// <param name="message"><see cref="Exception.Message"/>.</param>
        /// <param name="inner"><see cref="Exception.InnerException"/>.</param>
        public ExternalServiceInvocationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalServiceInvocationException"/> class.
        /// </summary>
        /// <param name="actionOrConditionContentType">
        /// The content type of the workflow action or condition that was executing when this error occurred.
        /// </param>
        /// <param name="actionOrConditionId">
        /// The Id of the workflow action or condition that was executing when this error occurred.
        /// </param>
        /// <param name="workflowInstanceId">
        /// The Id of the workflow instance that the action or condition was being applied to.
        /// </param>
        /// <param name="triggerId">The Id of the trigger whose execution led to this error.</param>
        /// <param name="responseStatusCode">The status code received from the external service.</param>
        /// <param name="responseReasonPhrase">The reason phrase received from the external service.</param>
        /// <param name="innerException">The underlying exception that resulted in this exception being thrown, if any.</param>
        public ExternalServiceInvocationException(
            string actionOrConditionContentType,
            string actionOrConditionId,
            string workflowInstanceId,
            string triggerId,
            HttpStatusCode responseStatusCode,
            string responseReasonPhrase,
            Exception innerException = null)
            : this($"Action or condition [{actionOrConditionContentType}] with Id [{actionOrConditionId}] return status code [{responseStatusCode} - {responseReasonPhrase}] when executing for workflow instance [{workflowInstanceId}] as part of trigger [{triggerId}]", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalServiceInvocationException"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ExternalServiceInvocationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}