// <copyright file="HostedWorkflowTriggerParameterCondition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// A condition that operates on <see cref="HostedWorkflowTrigger"/> and tests a parameter exists and has the specified
    /// value.
    /// </summary>
    public class HostedWorkflowTriggerParameterCondition : IWorkflowCondition
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.hosted.triggerparametervaluecondition";

        /// <inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the name of the parameter to test.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the value to test against the parameter.
        /// </summary>
        public string ParameterValue { get; set; }

        /// <summary>
        /// Gets or sets the type of test that will be carried out.
        /// </summary>
        public HostedWorkflowTriggerParameterTestType ParameterTestType { get; set; }

        /// <inheritdoc/>
        public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            if (trigger is HostedWorkflowTrigger hostedTrigger)
            {
                string actualValue = null;
                bool parameterExists = hostedTrigger.Parameters?.TryGetValue(this.ParameterName, out actualValue) ?? false;

                switch (this.ParameterTestType)
                {
                    case HostedWorkflowTriggerParameterTestType.Equals:
                        return Task.FromResult(parameterExists && actualValue == this.ParameterValue);

                    case HostedWorkflowTriggerParameterTestType.DoesNotEqual:
                        return Task.FromResult(parameterExists && actualValue != this.ParameterValue);

                    case HostedWorkflowTriggerParameterTestType.Exists:
                        return Task.FromResult(parameterExists);

                    case HostedWorkflowTriggerParameterTestType.DoesNotExist:
                        return Task.FromResult(!parameterExists);
                }
            }

            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            return Enumerable.Empty<string>();
        }
    }
}
