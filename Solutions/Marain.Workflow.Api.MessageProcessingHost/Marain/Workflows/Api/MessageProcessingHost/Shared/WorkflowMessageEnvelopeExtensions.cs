// <copyright file="WorkflowMessageEnvelopeExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessageProcessingHost.Shared
{
    using System;
    using System.Collections.Generic;
    using Corvus.Json;

    /// <summary>
    /// Extension methods for the <see cref="WorkflowMessageEnvelope"/> class, allowing easier get/set of items in the
    /// <see cref="WorkflowMessageEnvelope.Properties"/> collection.
    /// </summary>
    public static class WorkflowMessageEnvelopeExtensions
    {
        /// <summary>
        /// Gets the page of workflow instances to request.
        /// </summary>
        /// <param name="data">The envelope to add the value to.</param>
        /// <returns>The page of workflow instances to process.</returns>
        public static int GetWorkflowInstancesPageNumber(this WorkflowMessageEnvelope data)
        {
            if (data.Properties.TryGet<int>("GetWorkflowInstancesPageNumber", out int result))
            {
                return result;
            }

            throw new InvalidOperationException("No workflow instances page number has been stored in the WorkflowMessageEnvelope properties");
        }

        /// <summary>
        /// Sets the workflow instances page number.
        /// </summary>
        /// <param name="data">The envelope to add the value to.</param>
        /// <param name="propertyBagFactory">The property bag factory.</param>
        /// <param name="pageNumber">The page number.</param>
        public static void SetWorkflowInstancesPageNumber(
            this WorkflowMessageEnvelope data,
            IPropertyBagFactory propertyBagFactory,
            int pageNumber)
        {
            data.Properties = propertyBagFactory.CreateModified(
                data.Properties,
                new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("GetWorkflowInstancesPageNumber", pageNumber) },
                null);
        }

        /// <summary>
        /// Gets the workflow instance Id to process.
        /// </summary>
        /// <param name="data">The envelope to add the value to.</param>
        /// <returns>The workflow instance id.</returns>
        public static string GetWorkflowInstanceId(this WorkflowMessageEnvelope data)
        {
            if (data.Properties.TryGet("WorkflowInstanceId", out string result))
            {
                return result;
            }

            throw new InvalidOperationException("No workflow instance Id has been stored in the WorkflowMessageEnvelope properties");
        }

        /// <summary>
        /// Sets the workflow instance Id to process.
        /// </summary>
        /// <param name="data">The envelope to add the value to.</param>
        /// <param name="propertyBagFactory">The property bag factory.</param>
        /// <param name="workflowInstanceId">The workflow instance id.</param>
        public static void SetWorkflowInstanceId(
            this WorkflowMessageEnvelope data,
            IPropertyBagFactory propertyBagFactory,
            string workflowInstanceId)
        {
            data.Properties = propertyBagFactory.CreateModified(
                data.Properties,
                new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("WorkflowInstanceId", workflowInstanceId) },
                null);
        }
    }
}
