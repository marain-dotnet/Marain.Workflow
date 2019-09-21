namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    class FakeTenantProvider : ITenantProvider
    {
        public FakeTenantProvider(RootTenant rootTenant)
        {
            this.Root = rootTenant;
        }

        public ITenant Root { get; }

        public Task<ITenant> CreateChildTenantAsync(string parentTenantId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTenantAsync(string tenantId)
        {
            throw new NotImplementedException();
        }

        public Task<TenantCollectionResult> GetChildrenAsync(string tenantId, int limit = 20, string continuationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<ITenant> GetTenantAsync(string tenantId, string eTag = null)
        {
            if (tenantId == RootTenant.RootTenantId)
            {
                return Task.FromResult(this.Root);
            }

            return Task.FromResult(default(ITenant));
        }

        public Task<ITenant> UpdateTenantAsync(ITenant tenant)
        {
            throw new NotImplementedException();
        }
    }
}
