using System;
using System.Net.Http;

namespace DemoIdentityModelExtras
{
    public interface IDefaultHttpClientFactory
    {
        HttpMessageHandler HttpMessageHandler { get; }
        HttpClient HttpClient { get; }
    }
}
