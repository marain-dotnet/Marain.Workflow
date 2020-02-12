// <copyright file="HostedWorkflowTriggerNameCondition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A condition for hosted workflows that checks the TriggerName property of the trigger.
    /// </summary>
    public class HostedWorkflowTriggerNameCondition : IWorkflowCondition
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.hosted.triggernamecondition";

        /// <inheritdoc />
        public string ContentType => RegisteredContentType;

        /// <inheritdoc />
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the name of the trigger to look for.
        /// </summary>
        public string TriggerName { get; set; }

        /// <inheritdoc />
        public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            if (trigger is HostedWorkflowTrigger hostedTrigger)
            {
                bool result = hostedTrigger.TriggerName == this.TriggerName;
                return Task.FromResult(result);
            }

            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            return new[] { this.TriggerName };
        }
    }
}