// <copyright file="TriggerContentTypeCondition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     An <see cref="IWorkflowCondition" /> intended to be used on a transition that limits the
    ///     transition to only being used for a specific type of trigger, identified by its
    ///     <see cref="IWorkflowTrigger.ContentType" /> property.
    /// </summary>
    public class TriggerContentTypeCondition : IWorkflowCondition
    {
        /// <summary>
        ///     Gets the content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.triggercontenttypecondition";

        /// <inheritdoc />
        public string ContentType => RegisteredContentType;

        /// <inheritdoc />
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        ///     Gets or sets the content type of the trigger to target. If creating this in code, it's
        ///     recommended you use the <see cref="RegisteredContentType" /> constant that the all triggers
        ///     should
        ///     have.
        /// </summary>
        public string TriggerContentType { get; set; }

        /// <inheritdoc />
        public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            bool result = trigger.ContentType == this.TriggerContentType;

            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            return Enumerable.Empty<string>();
        }
    }
}