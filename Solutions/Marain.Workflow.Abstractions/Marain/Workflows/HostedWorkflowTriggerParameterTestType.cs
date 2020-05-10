// <copyright file="HostedWorkflowTriggerParameterTestType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    /// <summary>
    /// Types of test for the <see cref="HostedWorkflowTriggerParameterCondition"/>.
    /// </summary>
    public enum HostedWorkflowTriggerParameterTestType
    {
        /// <summary>
        /// The parameter value should match the value specified in the condition.
        /// </summary>
        Equals,

        /// <summary>
        /// The parameter value should not match the value specified in the condition.
        /// </summary>
        DoesNotEqual,

        /// <summary>
        /// The specified parameter must be supplied.
        /// </summary>
        Exists,

        /// <summary>
        /// The specified parameter must not be present.
        /// </summary>
        DoesNotExist,
    }
}