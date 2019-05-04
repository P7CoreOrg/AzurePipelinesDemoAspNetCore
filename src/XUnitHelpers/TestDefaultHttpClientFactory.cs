

using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using DemoIdentityModelExtras;

namespace XUnitHelpers
{
    public class TestDefaultHttpClientFactory : IDefaultHttpClientFactory
    {
        public static TestServer TestServer { get; set; }
        public HttpMessageHandler HttpMessageHandler => TestServer.CreateHandler();
        public HttpClient HttpClient => TestServer.CreateClient();
    }
}
