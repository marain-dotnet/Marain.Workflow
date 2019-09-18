// <copyright file="CreateCatalogItemTrigger.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;

    internal class FakeServiceIdentityTokenSource : IServiceIdentityTokenSource
    {
        private readonly string token = Guid.NewGuid().ToString();

        public Task<string> GetAccessToken(string resource) => Task.FromResult(resource + "/" + this.token);
    }
}
