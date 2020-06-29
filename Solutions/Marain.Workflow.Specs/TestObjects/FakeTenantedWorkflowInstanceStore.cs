// <copyright file="FakeTenantedWorkflowInstanceStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Workflows;

    public class FakeTenantedWorkflowInstanceStore : IWorkflowInstanceStore
    {
        public Task DeleteWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null)
        {
            return Task.CompletedTask;
        }

        public Task<int> GetMatchingWorkflowInstanceCountForSubjectsAsync(IEnumerable<string> subjects)
        {
            return Task.FromResult(0);
        }

        public Task<IEnumerable<string>> GetMatchingWorkflowInstanceIdsForSubjectsAsync(IEnumerable<string> subjects, int pageSize, int pageNumber)
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }

        public Task<WorkflowInstance> GetWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null)
        {
            throw new WorkflowInstanceNotFoundException();
        }

        public Task UpsertWorkflowInstanceAsync(WorkflowInstance workflowInstance, string partitionKey = null)
        {
            throw new NotImplementedException();
        }
    }
}
