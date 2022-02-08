// <copyright file="WorkflowTenancyConstants.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows;

/// <summary>
/// String constants used in tenanted workflow blob storage.
/// </summary>
internal static class WorkflowTenancyConstants
{
    /// <summary>
    /// Gets the logical container name for the blob storage container holding workflow definitions..
    /// </summary>
    public const string StoreDefinitionsLogicalContainerName = "workflowdefinitions";

    /// <summary>
    /// Gets the key to use with the tenant properties to find the blob storage container definition
    /// that will be used for the tenanted workflow store when using old-style (Corvus.Tenancy v2) configuration.
    /// </summary>
    public const string DefinitionsStoreTenantConfigKey = "StorageConfiguration__" + StoreDefinitionsLogicalContainerName;

    /// <summary>
    /// Gets the container definition that will be used for the tenanted workflow store when
    /// using new-style (Corvus.Tenancy v3+) configuration.
    /// </summary>
    public static string DefinitionsStoreTenantConfigKeyV3 { get; } = "Workflow_BlobStorage_Definitions";
}