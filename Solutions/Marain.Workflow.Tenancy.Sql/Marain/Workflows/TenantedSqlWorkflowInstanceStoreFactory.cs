// <copyright file="TenantedSqlWorkflowInstanceStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;

    using Corvus.Extensions.Json;
    using Corvus.Storage.Sql;
    using Corvus.Storage.Sql.Tenancy;
    using Corvus.Tenancy;

    using Marain.Workflows.Storage;

    using Microsoft.Data.SqlClient;

    /// <summary>
    /// Factory class for retrieving Sql-based instances of <see cref="IWorkflowStore"/> for specific <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedSqlWorkflowInstanceStoreFactory : ITenantedWorkflowInstanceStoreFactory
    {
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly ISqlConnectionFromDynamicConfiguration connectionSource;
        private readonly string configurationKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedSqlWorkflowInstanceStoreFactory"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The JSON serializer settings provider.</param>
        /// <param name="connectionSource">
        /// The <see cref="ISqlConnectionFromDynamicConfiguration"/> that will be used to create
        /// underlying <see cref="SqlConnection"/> instances for the content stores.
        /// </param>
        /// <param name="configurationKey">
        /// The tenant properties configuration key in which to find settings.
        /// </param>
        public TenantedSqlWorkflowInstanceStoreFactory(
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ISqlConnectionFromDynamicConfiguration connectionSource,
            string configurationKey)
        {
            this.serializerSettingsProvider = serializerSettingsProvider;
            this.connectionSource = connectionSource;
            this.configurationKey = configurationKey;
        }

        /// <inheritdoc/>
        public Task<IWorkflowInstanceStore> GetWorkflowInstanceStoreForTenantAsync(ITenant tenant)
        {
            return Task.FromResult<IWorkflowInstanceStore>(
                new SqlWorkflowInstanceStore(
                    this.serializerSettingsProvider,
                    () => this.connectionSource.GetSqlConnectionForTenantAsync(tenant, this.configurationKey)));
        }
    }
}