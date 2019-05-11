

using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using DemoIdentityModelExtras;

namespace XUnitHelpers
{
    public class TestDefaultHttpClientFactory : IDefaultHttpClientFactory
    {
        public HttpMessageHandler HttpMessageHandler { get; set; }
        public HttpClient HttpClient { get; set; }
    }
}
