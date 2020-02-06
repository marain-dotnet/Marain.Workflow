// <copyright file="WorkflowMessageEnvelopeExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessagePreProcessingHost.Shared
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="WorkflowMessageEnvelope"/> class, allowing easier get/set of items in the
    /// <see cref="WorkflowMessageEnvelope.Properties"/> collection.
    /// </summary>
    public static class WorkflowMessageEnvelopeExtensions
    {
        /// <summary>
        /// Gets the page of workflow instances to request.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetWorkflowInstancesPageNumber(this WorkflowMessageEnvelope data)
        {
            if (data.Properties.TryGet<int>("GetWorkflowInstancesPageNumber", out int result))
            {
                return result;
            }

            throw new InvalidOperationException("No workflow instances page number has been stored in the WorkflowMessageEnvelope properties");
        }

        /// <summary>
        /// The set workflow instances page number.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="pageNumber">
        /// The page number.
        /// </param>
        public static void SetWorkflowInstancesPageNumber(this WorkflowMessageEnvelope data, int pageNumber)
        {
            data.Properties.Set("GetWorkflowInstancesPageNumber", pageNumber);
        }

        /// <summary>
        /// The get workflow instance id.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetWorkflowInstanceId(this WorkflowMessageEnvelope data)
        {
            if (data.Properties.TryGet<string>("WorkflowInstanceId", out string result))
            {
                return result;
            }

            throw new InvalidOperationException("No workflow instance Id has been stored in the WorkflowMessageEnvelope properties");
        }

        /// <summary>
        /// The set workflow instance id.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="workflowInstanceId">
        /// The workflow instance id.
        /// </param>
        public static void SetWorkflowInstanceId(this WorkflowMessageEnvelope data, string workflowInstanceId)
        {
            data.Properties.Set("WorkflowInstanceId", workflowInstanceId);
        }
    }
}
