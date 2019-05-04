using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public SomeThingController(IDog dog)
        {
            _dog = dog;

        }
        // GET: api/SomeThing
        [HttpGet]
        public IEnumerable<string> Get()
        {
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
