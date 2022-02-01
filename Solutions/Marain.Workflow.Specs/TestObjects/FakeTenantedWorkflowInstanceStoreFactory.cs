// <copyright file="FakeTenantedWorkflowInstanceStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Workflows;

    public class FakeTenantedWorkflowInstanceStoreFactory : ITenantedWorkflowInstanceStoreFactory
    {
        public Task<IWorkflowInstanceStore> GetWorkflowInstanceStoreForTenantAsync(ITenant tenant)
        {
            return Task.FromResult<IWorkflowInstanceStore>(new FakeTenantedWorkflowInstanceStore());
        }
    }
}