// <copyright file="CreateCatalogItemTrigger.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Provides an external HTTP service.
    /// </summary>
    [Binding]
    public static class ExternalServiceBindings
    {
        /// <summary>
        /// Gets the external service associated with the current scenario.
        /// </summary>
        public static ExternalService GetExternalService (ScenarioContext context) => context.Get<ExternalService>(typeof(ExternalService).FullName);

        /// <summary>
        /// Create a suitable Http listener.
        /// </summary>
        /// <param name="scenarioContext"></param>
        [BeforeScenario("@externalServiceRequired")]
        public static void InitializeService(ScenarioContext scenarioContext)
        {
            // IANA recommends this range for dynamic or private ports.
            const int MinPort = 49215;
            const int MaxPort = 65535;

            HttpListener listener = null;
            for (int i = MinPort; i <= MaxPort; ++i)
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://localhost:{i}/");
                try
                {
                    listener.Start();
                    break;
                }
                catch (HttpListenerException x)
                {
                    Debug.WriteLine(x.ErrorCode);
                    listener = null;
                }
            }

            if (listener == null)
            {
                Assert.Fail("Unable to find an available port to create test HTTP listener");
            }

            var externalService = new ExternalService(listener);
            scenarioContext.Add(typeof(ExternalService).FullName, externalService);
        }

        /// <summary>
        /// Ensure we tear down the listener.
        /// </summary>
        /// <param name="scenarioContext"></param>
        /// <returns></returns>
        [AfterScenario("@externalServiceRequired")]
        public static Task TearDownService(ScenarioContext scenarioContext)
        {
            return scenarioContext.RunAndStoreExceptionsAsync(
                () =>
                {
                    var service = (ExternalService)scenarioContext[typeof(ExternalService).FullName];
                    return service.StopAsync();
                });
        }

        /// <summary>
        /// Stand-in for an external HTTP-based service, providing various test endpoints.
        /// </summary>
        public class ExternalService
        {
            private const string UrlPath = "/the/url";

            private readonly HttpListener listener;
            private readonly Uri baseUrl;

            private readonly Task mainLoopTask;
            private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();

            /// <summary>
            /// Creates a new instance of hte external service bindings for a specific http listener
            /// </summary>
            /// <param name="listener"></param>
            public ExternalService(HttpListener listener)
            {
                this.listener = listener;
                this.baseUrl = new Uri(this.listener.Prefixes.First());

                this.mainLoopTask = Task.Run(() => this.MainLoop(this.cancellationSource.Token));
            }

            /// <summary>
            /// Gets a URL to which a POST may be performed, and which will produce a 200 result
            /// with <c>true</c> in the response body.
            /// </summary>
            public Uri TestUrl => new Uri(this.baseUrl, UrlPath);

            /// <summary>
            /// Gets or sets the status code to return.
            /// </summary>
            public int StatusCode { get; set; }

            /// <summary>
            /// Gets or sets the content to return as a result.
            /// </summary>
            public string ResponseBody { get; set; }

            /// <summary>
            /// All of the requests made to this service.
            /// </summary>
            public List<RequestInfo> Requests { get; } = new List<RequestInfo>();

            /// <summary>
            /// Shuts the listener down.
            /// </summary>
            /// <returns>A task that completes once the listener has been shut down.</returns>
            public async Task StopAsync()
            {
                this.cancellationSource.Cancel();
                try
                {
                    this.listener.Stop();
                    await this.mainLoopTask;
                    ((IDisposable)this.listener).Dispose();
                }
                catch(ObjectDisposedException ex)
                {
                }
            }

            private async Task MainLoop(CancellationToken cancel)
            {
                while (!cancel.IsCancellationRequested)
                {
                    try
                    {
                        HttpListenerContext context = await this.listener.GetContextAsync().ConfigureAwait(false);

                        var info = new RequestInfo
                        {
                            Url = context.Request.Url,
                            Verb = context.Request.HttpMethod.ToUpperInvariant()
                        };
                        this.Requests.Add(info);
                        foreach (string headerName in context.Request.Headers.AllKeys)
                        {
                            info.Headers.Add(headerName, context.Request.Headers.GetValues(headerName).Single());
                        }

                        if (context.Request.HasEntityBody)
                        {
                            using (var r = new StreamReader(context.Request.InputStream))
                            {
                                info.RequestBody = r.ReadToEnd();
                            }
                            context.Request.InputStream.Close();
                        }

                        HttpListenerResponse response = context.Response;
                        string requestPath = context.Request.Url.AbsolutePath;
                        response.StatusCode = this.StatusCode;
                        if (!string.IsNullOrWhiteSpace(this.ResponseBody))
                        {
                            WriteJsonResponse(response, this.ResponseBody);
                        }

                        response.Close();
                    }
                    catch (HttpListenerException x)
                    when (x.ErrorCode == 995)
                    {
                        // This is the normal error we expect when being shut down.
                    }
                }
            }

            private static void WriteJsonResponse(HttpListenerResponse response, string responseString)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

                response.ContentType = "application/json";
            }

            /// <summary>
            /// Provides information about a request.
            /// </summary>
            public class RequestInfo
            {
                /// <summary>
                /// The URL to which the request was made.
                /// </summary>
                public Uri Url { get; set; }

                /// <summary>
                /// The HTTP verb of the request.
                /// </summary>
                public string Verb { get; set; }

                /// <summary>
                /// Gets a dictionary of the headers in the request.
                /// </summary>
                public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

                /// <summary>
                /// The request body.
                /// </summary>
                public string RequestBody { get; set; }
            }
        }
    }
}
