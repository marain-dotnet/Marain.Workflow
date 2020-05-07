// <copyright file="EntityIdCondition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// An <see cref="IWorkflowCondition" /> which checks a value from an <see cref="EntityIdTrigger" /> against
    /// a value from the context of the <see cref="WorkflowInstance" />, and ensures that the value of the
    /// <see cref="Activity" /> property matches the corresponding value on the trigger.
    /// </summary>
    /// <remarks>
    /// If the incoming trigger is not an <see cref="EntityIdTrigger" />, the condition will always evaluate
    /// to false.
    /// </remarks>
    public class EntityIdCondition : IWorkflowCondition
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.entityidcondition";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityIdCondition" /> class.
        /// </summary>
        public EntityIdCondition()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the activity, which will be compared against the corresponding value on the trigger.
        /// </summary>
        public string Activity { get; set; }

        /// <summary>
        /// Gets the content type that will be used when serializing/deserializing.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the key that will be used to retrieve the entity Id to compare from the
        /// <see cref="WorkflowInstance" />.
        /// </summary>
        public string EntityIdContextProperty { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            bool result = false;

            if (trigger is EntityIdTrigger etrigger)
            {
                if (instance.Context.TryGet(this.EntityIdContextProperty, out string value))
                {
                    result = value == etrigger.EntityId;
                }

                result = result && etrigger.Activity == this.Activity;
            }

            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            if (instance.Context.TryGet(this.EntityIdContextProperty, out string value))
            {
                return new[] { value };
            }

            return Enumerable.Empty<string>();
        }
    }
}