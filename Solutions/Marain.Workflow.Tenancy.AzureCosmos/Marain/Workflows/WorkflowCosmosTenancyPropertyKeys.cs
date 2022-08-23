// <copyright file="WorkflowCosmosTenancyPropertyKeys.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows;

/// <summary>
/// Keys used in tenant property dictionary when storing Cosmos DB configuration for workflow.
/// </summary>
public static class WorkflowCosmosTenancyPropertyKeys
{
    /// <summary>
    /// The key under which configuration for the workflow definitions Cosmos DB container must be
    /// stored in tenant properties, if using Cosmos DB to store definitions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Although we implement a Cosmos DB definition store, the Workflow v1 service manifest (id
    /// 3633754ac4c9be44b55bfe791b1780f177b464860334774cabb2f9d1b95b0c18) specifies that the
    /// definitions live in blob storage, with Cosmos DB being used only for the instance store.
    /// So in practice, this isn't used today in normal configurations.
    /// </para>
    /// </remarks>
    public const string Definitions = "Marain:Workflow:CosmosContainerConfiguration:Definitions";

    /// <summary>
    /// The key under which configuration for the workflow instances Cosmos DB container must be
    /// stored in tenant properties.
    /// </summary>
    public const string Instances = "Marain:Workflow:CosmosContainerConfiguration:Instances";
}