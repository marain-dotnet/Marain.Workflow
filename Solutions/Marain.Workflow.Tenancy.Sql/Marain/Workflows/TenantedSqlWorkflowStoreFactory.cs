// <copyright file="TenantedSqlWorkflowStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Sql.Tenancy;
    using Corvus.Tenancy;
    using Marain.Workflows.Storage;

    /// <summary>
    /// Factory class for retrieving Sql-based instances of <see cref="IWorkflowStore"/> for specific <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedSqlWorkflowStoreFactory : ITenantedWorkflowStoreFactory
    {
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly ITenantSqlConnectionFactory containerFactory;
        private readonly SqlConnectionDefinition connectionDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedSqlWorkflowStoreFactory"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The JSON serializer settings provider.</param>
        /// <param name="containerFactory">The <see cref="ITenantSqlConnectionFactory"/> that will be used to create
        /// underlying <see cref="SqlConnection"/> instances for the content stores.</param>
        /// <param name="connectionDefinition">The <see cref="SqlConnectionDefinition"/> to use when creating tenanted
        /// <see cref="SqlConnection"/> instances.</param>
        public TenantedSqlWorkflowStoreFactory(
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ITenantSqlConnectionFactory containerFactory,
            SqlConnectionDefinition connectionDefinition)
        {
            this.serializerSettingsProvider = serializerSettingsProvider;
            this.containerFactory = containerFactory;
            this.connectionDefinition = connectionDefinition;
        }

        /// <inheritdoc/>
        public Task<IWorkflowStore> GetWorkflowStoreForTenantAsync(ITenant tenant)
        {
            return Task.FromResult<IWorkflowStore>(
                new SqlWorkflowStore(
                    this.serializerSettingsProvider,
                    () => this.containerFactory.GetSqlConnectionForTenantAsync(tenant, this.connectionDefinition)));
        }
    }
}