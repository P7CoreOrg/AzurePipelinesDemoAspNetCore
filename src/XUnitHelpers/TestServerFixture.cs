using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DemoIdentityModelExtras;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;

namespace XUnitHelpers
{
    /// <summary>
    /// An HTTP handler that suppresses the execution context flow.
    /// Workaround from: https://github.com/aspnet/AspNetCore/issues/7975#issuecomment-481536061
    /// </summary>
    internal class SuppressExecutionContextHandler : DelegatingHandler
    {
        public SuppressExecutionContextHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // NOTE: We DO NOT want to 'await' the task inside this using. We're just suppressing execution context flow
            // while the task itself is created (which is what would capture the context). After that we just return the
            // (now detached task) to the caller.
            Task<HttpResponseMessage> t;
            using (ExecutionContext.SuppressFlow())
            {
                t = Task.Run(() => base.SendAsync(request, cancellationToken));
            }

            return t;
        }
    }
    public abstract class TestServerFixture<TStartup> : IDisposable where TStartup : class
    {
        public readonly TestServer _testServer;
        private string _environmentUrl;

        public HttpMessageHandler MessageHandler { get; }

        // RelativePathToHostProject = @"..\..\..\..\TheWebApp";
        protected abstract string RelativePathToHostProject { get; }

        public TestServerFixture()
        {
            var contentRootPath = GetContentRootPath();
            var builder = new WebHostBuilder()
                .UseContentRoot(contentRootPath)
                .UseEnvironment("Development")
                .ConfigureServices(services =>
                {
                    services.RemoveAll<IDefaultHttpClientFactory>();
                    services.TryAddTransient<IDefaultHttpClientFactory>(serviceProvider =>
                    {
                        return new TestDefaultHttpClientFactory()
                        {
                            HttpClient = Client,
                            HttpMessageHandler = MessageHandler
                        };
                    });
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                    LoadConfigurations(config, environmentName);

                })
                .UseStartup<TStartup>(); // Uses Start up class from your API Host project to configure the test server
            string environmentUrl = Environment.GetEnvironmentVariable("TestEnvironmentUrl");
            IsUsingInProcTestServer = false;
            if (string.IsNullOrWhiteSpace(environmentUrl))
            {
                environmentUrl = "http://localhost/";

                _testServer = new TestServer(builder);

                MessageHandler = _testServer.CreateHandler();
                IsUsingInProcTestServer = true;

                // We need to suppress the execution context because there is no boundary between the client and server while using TestServer
                MessageHandler = new SuppressExecutionContextHandler(MessageHandler);
            }

            _environmentUrl = environmentUrl;

        }

        public bool IsUsingInProcTestServer { get; set; }

        public HttpClient CreateHttpClient()
            => new HttpClient(new SessionMessageHandler(MessageHandler)) { BaseAddress = new Uri(_environmentUrl) };
        public HttpClient Client => CreateHttpClient();

        /// <summary>
        /// An <see cref="HttpMessageHandler"/> that maintains session consistency between requests.
        /// </summary>
        private class SessionMessageHandler : DelegatingHandler
        {
            private string _sessionToken;

            public SessionMessageHandler(HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (!string.IsNullOrEmpty(_sessionToken))
                {
                    request.Headers.TryAddWithoutValidation("x-ms-session-token", _sessionToken);
                }

                request.Headers.TryAddWithoutValidation("x-ms-consistency-level", "Session");

                var response = await base.SendAsync(request, cancellationToken);

                if (response.Headers.TryGetValues("x-ms-session-token", out var tokens))
                {
                    _sessionToken = tokens.SingleOrDefault();
                }

                return response;
            }
        }
        protected abstract void LoadConfigurations(IConfigurationBuilder config, string environmentName);

        private string GetContentRootPath()
        {
            var testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;
            return Path.Combine(testProjectPath, RelativePathToHostProject);
        }

        public void Dispose()
        {
            Client.Dispose();
            _testServer.Dispose();
        }
    }
}
