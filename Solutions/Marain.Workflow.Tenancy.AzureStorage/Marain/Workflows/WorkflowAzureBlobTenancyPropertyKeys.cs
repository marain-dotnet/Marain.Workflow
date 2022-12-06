// <copyright file="WorkflowAzureBlobTenancyPropertyKeys.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows;

/// <summary>
/// Keys used in tenant property dictionary when storing Cosmos DB configuration for workflow.
/// </summary>
public static class WorkflowAzureBlobTenancyPropertyKeys
{
    /// <summary>
    /// The key under which configuration for the workflow definitions blob container must be
    /// stored in tenant properties, if using Azure blob storage to store definitions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Blob storage is the normal storage mechanism for workflow definitions. Although we
    /// implement a Cosmos DB definition store, the Workflow v1 service manifest (id
    /// 3633754ac4c9be44b55bfe791b1780f177b464860334774cabb2f9d1b95b0c18) specifies that the
    /// definitions live in blob storage, with Cosmos DB being used only for the instance store.
    /// So in practice, this blob storage key is the one normally used..
    /// </para>
    /// </remarks>
    public const string Definitions = "Marain:Workflow:BlobContainerConfiguration:Definitions";

    /// <summary>
    /// Gets the logical container name for the blob storage container holding workflow definitions
    /// when using old-style (Corvus.Tenancy v2) configuration.
    /// </summary>
    internal const string StoreDefinitionsLogicalContainerName = "workflowdefinitions";

    /// <summary>
    /// Gets the key to use with the tenant properties to find the blob storage container definition
    /// that will be used for the tenanted workflow store when using old-style (Corvus.Tenancy v2) configuration.
    /// </summary>
    internal const string DefinitionsStoreTenantConfigKey = "StorageConfiguration__" + StoreDefinitionsLogicalContainerName;
}