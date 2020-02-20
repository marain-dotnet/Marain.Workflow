// <copyright file="TenantedSqlWorkflowInstanceStoreFactory.cs" company="Endjin Limited">
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
    public class TenantedSqlWorkflowInstanceStoreFactory : ITenantedWorkflowInstanceStoreFactory
    {
        private readonly ITenantSqlConnectionFactory containerFactory;
        private readonly SqlConnectionDefinition connectionDefinition;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedSqlWorkflowInstanceStoreFactory"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The JSON serializer settings provider.</param>
        /// <param name="containerFactory">The <see cref="ITenantSqlConnectionFactory"/> that will be used to create
        /// underlying <see cref="SqlConnection"/> instances for the content stores.</param>
        /// <param name="connectionDefintiion">The <see cref="SqlConnectionDefinition"/> to use when creating tenanted
        /// <see cref="SqlConnection"/> instances.</param>
        public TenantedSqlWorkflowInstanceStoreFactory(
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ITenantSqlConnectionFactory containerFactory,
            SqlConnectionDefinition connectionDefintiion)
        {
            this.containerFactory = containerFactory;
            this.connectionDefinition = connectionDefintiion;
            this.serializerSettingsProvider = serializerSettingsProvider;
        }

        /// <inheritdoc/>
        public Task<IWorkflowInstanceStore> GetWorkflowInstanceStoreForTenantAsync(ITenant tenant)
        {
            return Task.FromResult<IWorkflowInstanceStore>(
                new SqlWorkflowInstanceStore(
                    this.serializerSettingsProvider,
                    () => this.containerFactory.GetSqlConnectionForTenantAsync(tenant, this.connectionDefinition)));
        }
    }
}
