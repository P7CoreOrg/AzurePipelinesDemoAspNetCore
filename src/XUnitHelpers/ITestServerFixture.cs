using System.Net.Http;
using Microsoft.AspNetCore.TestHost;

namespace XUnitHelpers
{
    public interface ITestServerFixture
    {
        HttpClient Client { get; }
        HttpMessageHandler MessageHandler { get; }
        TestServer TestServer { get; }
    }
}