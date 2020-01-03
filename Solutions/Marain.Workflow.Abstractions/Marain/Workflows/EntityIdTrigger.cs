// <copyright file="EntityIdTrigger.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An <see cref="IWorkflowTrigger" /> implementation which  is used in conjunction with an
    /// <see cref="EntityIdCondition" /> to allow the condition to succeed only if a named context
    /// value from the <see cref="WorkflowInstance" /> has a value matching the <see cref="EntityId" />
    /// property of the trigger, and if the <see cref="Activity" /> property of the trigger matches that
    /// of the condition.
    /// </summary>
    public class EntityIdTrigger : IWorkflowTrigger
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.entityid";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityIdTrigger" /> class.
        /// </summary>
        public EntityIdTrigger()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityIdTrigger" /> class.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <param name="activity">
        /// The activity.Yes.
        /// </param>
        public EntityIdTrigger(string entityId, string activity)
            : this()
        {
            this.EntityId = entityId;
            this.Activity = activity;
        }

        /// <summary>
        /// Gets or sets the name of the activity this trigger relates to. This must match the
        /// Activity specified in the corresponding condition for the trigger to be accepted.
        /// </summary>
        public string Activity { get; set; }

        /// <inheritdoc />
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the value that must match the context value from the <see cref="WorkflowInstance" />
        /// whose key is specified in the corresponding condition.
        /// </summary>
        public string EntityId { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public string PartitionKey => this.EntityId;

        /// <inheritdoc />
        public IEnumerable<string> GetSubjects()
        {
            return new[] { this.EntityId };
        }
    }
}