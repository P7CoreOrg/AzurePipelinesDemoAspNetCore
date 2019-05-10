using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DemoIdentityModelExtras;
using DemoLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TheWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SomeThingController : ControllerBase
    {
        private IDog _dog;
        private IDefaultHttpClientFactory _defaultHttpClientFactory;
        private IHttpContextAccessor _httpContextAccessor;

        public SomeThingController(
            IHttpContextAccessor httpContextAccessor,
            IDefaultHttpClientFactory defaultHttpClientFactory,
            IDog dog)
        {
            _httpContextAccessor = httpContextAccessor;
            _defaultHttpClientFactory = defaultHttpClientFactory;
            _dog = dog;

        }
        // GET: api/SomeThing
        [HttpGet]
        [Route("dog")]
        public IEnumerable<string> Get()
        {
            var httpClient = _defaultHttpClientFactory.HttpClient;
            var httpMessageHandler = _defaultHttpClientFactory.HttpMessageHandler;

            return new string[] { _dog.Name };
        }

        [HttpGet]
        [Route("pets")]
        public IEnumerable<string> GetPets()
        {
            return new string[] { "dog", "cat" };
        }

        // GET: api/SomeThing
        [HttpGet]
        [Route("from-external")]
        public async Task<IEnumerable<string>> GetFromExternal()
        {
            var httpClient = _defaultHttpClientFactory.HttpClient;
            var httpMessageHandler = _defaultHttpClientFactory.HttpMessageHandler;
            var httpContext = _httpContextAccessor.HttpContext;

            var req = new HttpRequestMessage(HttpMethod.Get, $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/SomeThing/pets");
            var response = await httpClient.SendAsync(req);

            httpContext = _httpContextAccessor.HttpContext;

            return new string[] { $"HttpContext:{httpContext != null}" };

        }

        // POST: api/SomeThing
        [HttpPost]
        [Route("dog")]
        public void Post([FromBody] string value)
        {
            _dog.Name = value;
        }
    }
}
