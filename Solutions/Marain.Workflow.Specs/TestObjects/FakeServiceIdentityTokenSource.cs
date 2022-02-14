// <copyright file="FakeServiceIdentityTokenSource.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Azure.Core;

    using Corvus.Identity.ClientAuthentication.Azure;

    /// <summary>
    /// Stub implementation of <see cref="IServiceIdentityAzureTokenCredentialSource"/>.
    /// </summary>
    internal class FakeServiceIdentityTokenSource : IServiceIdentityAzureTokenCredentialSource
    {
        private readonly string token = Guid.NewGuid().ToString();
        private readonly IServiceIdentityAzureTokenCredentialSource realTokenProvider;

        public FakeServiceIdentityTokenSource(IServiceIdentityAzureTokenCredentialSource realTokenProvider)
        {
            this.realTokenProvider = realTokenProvider;
        }

        /// <inheritdoc/>
        public ValueTask<TokenCredential> GetAccessTokenAsync() => this.GetTokenCredentialAsync();

        /// <inheritdoc/>
        public ValueTask<TokenCredential> GetReplacementForFailedTokenCredentialAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async ValueTask<TokenCredential> GetTokenCredentialAsync(CancellationToken cancellationToken = default)
        {
            return new FakeTokenCredential(await this.realTokenProvider.GetTokenCredentialAsync(cancellationToken), this.token);
        }

        private class FakeTokenCredential : TokenCredential
        {
            private readonly string token;
            private readonly TokenCredential realTokenProvider;

            public FakeTokenCredential(TokenCredential realTokenCredential, string token)
            {
                this.realTokenProvider = realTokenCredential;
                this.token = token;
            }

            public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                if (!requestContext.Scopes[0].StartsWith("foobar"))
                {
                    return this.realTokenProvider.GetToken(requestContext, cancellationToken);
                }

                return new AccessToken(
                    requestContext.Scopes[0] + "/" + this.token,
                    DateTimeOffset.UtcNow.AddHours(1));
            }

            public async override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                if (!requestContext.Scopes[0].StartsWith("foobar"))
                {
                    return await this.realTokenProvider.GetTokenAsync(requestContext, cancellationToken);
                }

                return this.GetToken(requestContext, cancellationToken);
            }
        }
    }
}