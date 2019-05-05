using System;
using System.Collections.Generic;
using System.Linq;
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

        public SomeThingController(
            IDefaultHttpClientFactory defaultHttpClientFactory,
            IDog dog)
        {
            _dog = dog;
            _defaultHttpClientFactory = defaultHttpClientFactory;

        }
        // GET: api/SomeThing
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var httpClient = _defaultHttpClientFactory.HttpClient;
            var httpMessageHandler = _defaultHttpClientFactory.HttpMessageHandler;

            return new string[] { _dog.Name };
        }


        // POST: api/SomeThing
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _dog.Name = value;
        }
    }
}
