// <copyright file="FakeTenantProvider.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

    internal class FakeTenantProvider : ITenantProvider
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

        public Task<ITenant> GetTenantAsync(string tenantId, string etag = null)
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
