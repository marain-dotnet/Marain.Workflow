// <copyright file="StubWorkflowEventSubscriber.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class StubWorkflowEventSubscriber : IAsyncDisposable
    {
        private CancellationTokenSource tokenSource;
        private HttpListener listener;
        private Task listenerTask;

        public StubWorkflowEventSubscriber(int port, HttpStatusCode responseStatusCode)
        {
            this.Port = port;
            this.ResponseStatusCode = responseStatusCode;
        }

        public int Port { get; }

        public IList<(string Method, string Content)> ReceivedRequests { get; } = new List<(string, string)>();

        public HttpStatusCode ResponseStatusCode { get; }

        public void Start()
        {
            this.tokenSource = new CancellationTokenSource();

            this.listener = new HttpListener();
            this.listener.Prefixes.Add($"http://localhost:{this.Port}/");
            this.listener.Start();

            this.listenerTask = this.LogRequests(this.tokenSource.Token);
        }

        public async ValueTask DisposeAsync()
        {
            this.tokenSource.Cancel();
            this.listener.Abort();

            await this.listenerTask.ConfigureAwait(false);
        }

        private async Task LogRequests(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    HttpListenerContext context = await this.listener.GetContextAsync().ConfigureAwait(false);

                    using var reader = new StreamReader(context.Request.InputStream);
                    string content = reader.ReadToEnd();
                    this.ReceivedRequests.Add((context.Request.HttpMethod, content));
                    context.Response.StatusCode = (int)this.ResponseStatusCode;
                    context.Response.OutputStream.Close();
                }
            }
            catch (Exception)
            when (cancellationToken.IsCancellationRequested)
            {
                // An exception gets thrown from the call to GetContextAsync when the listener is stopped. Depending
                // on exactly how the listener is stopped, we either get an HttpListenerException or an
                // ObjectDisposedException. If the cancellation token has been cancelled, then swallow the exception as
                // it means we're expecting this to happen. Otherwise it happened without us attempting to stop the
                // listener, so let the exception propagate.
            }
        }
    }
}