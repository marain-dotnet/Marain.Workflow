// <copyright file="FakeServiceIdentityTokenSource.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;

#pragma warning disable RCS1079 // Throwing of new NotImplementedException.

    /// <summary>
    /// Stub implementation of <see cref="IServiceIdentityTokenSource"/>.
    /// </summary>
    internal class FakeServiceIdentityTokenSource : IServiceIdentityTokenSource
    {
        private readonly string token = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public Task<string> GetAccessToken(string resource) => Task.FromResult(resource + "/" + this.token);

        /// <inheritdoc/>
        public Task<string> GetAccessTokenSpecifyingAuthority(string authority, string resource, string scope) =>
            throw new NotImplementedException();
    }
}

#pragma warning restore RCS1079 // Throwing of new NotImplementedException.
