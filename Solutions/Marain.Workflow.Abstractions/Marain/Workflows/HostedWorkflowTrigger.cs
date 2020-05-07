// <copyright file="HostedWorkflowTrigger.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Corvus.Extensions.Json;
    using Corvus.Json;

    /// <summary>
    /// A trigger for use in hosted scenarios, where it is not possible to have bespoke
    /// trigger types inside the workflow engine.
    /// </summary>
    public class HostedWorkflowTrigger : IWorkflowTrigger
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.hosted.trigger";

        /// <summary>
        /// Initializes a new instance of the <see cref="HostedWorkflowTrigger"/> class.
        /// </summary>
        /// <param name="parameters">A set of parameters that will be sent with the trigger.</param>
        public HostedWorkflowTrigger(IPropertyBag parameters)
        {
            this.Parameters = parameters;
        }

        /// <inheritdoc/>
        public string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the name of the trigger.
        /// </summary>
        public string TriggerName { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of parameters for the trigger.
        /// </summary>
        public IPropertyBag Parameters { get; set; }

        /// <inheritdoc/>
        public string PartitionKey
        {
            get
            {
                if (this.Subjects?.Any() != true)
                {
                    return Guid.NewGuid().ToString();
                }

                return string.Concat(this.Subjects);
            }
        }

        /// <summary>
        /// Gets or sets the list of subjects.
        /// </summary>
        public IEnumerable<string> Subjects { get; set; }

        /// <inheritdoc/>
        public IEnumerable<string> GetSubjects() => this.Subjects;
    }
}