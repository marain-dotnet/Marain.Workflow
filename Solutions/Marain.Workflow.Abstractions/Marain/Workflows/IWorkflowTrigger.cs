// <copyright file="IWorkflowTrigger.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    ///     The interface for triggers which can be passed to <see cref="WorkflowInstance" />s
    ///     for processing.
    /// </summary>
    public interface IWorkflowTrigger
    {
        /// <summary>
        ///     Gets the content type that will be used when serializing/deserializing.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        ///     Gets or sets the id for this state.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets the partition key.
        /// </summary>
        [JsonIgnore]
        string PartitionKey { get; }

        /// <summary>
        ///     Gets the list of subjects for this state.
        /// </summary>
        /// <returns>
        ///     The list of subjects for the trigger. These will be used to identify which
        ///     <see cref="WorkflowInstance" />s could potentially accept the trigger.
        /// </returns>
        IEnumerable<string> GetSubjects();
    }
}