using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DemoIdentityModelExtras;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using XUnitTestServerBase;

namespace xUnit_TheWebAppTests
{
    public class UnitTestSomeThing : TestServerBaseTests
    {
        private readonly MyTestServerFixture _fixture;

        public UnitTestSomeThing(MyTestServerFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void AssureFixture()
        {
            _fixture.ShouldNotBeNull();
            var client = _fixture.Client;
            client.ShouldNotBeNull();

            var messageHandler = _fixture.MessageHandler;
            messageHandler.ShouldNotBeNull();

        }
        [Fact]
        public async Task Test_Get_NotFound()
        {
            var client = _fixture.Client;
            var req = new HttpRequestMessage(HttpMethod.Get, "/Does/Not/Exist")
            {
                // Content = new FormUrlEncodedContent(dict)
            };
            var response = await client.SendAsync(req);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task Test_Get_Success()
        {
            var client = _fixture.Client;
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/SomeThing")
            {
                // Content = new FormUrlEncodedContent(dict)
            };
            var response = await client.SendAsync(req);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();
            jsonString.ShouldNotBeNullOrWhiteSpace();

            var dogs = JsonConvert.DeserializeObject<List<string>>(jsonString);
            dogs.ShouldNotBeNull();
            dogs.Count.ShouldBeGreaterThan(0);

        }
        [Fact]
        public async Task Test_Post_NotFound()
        {
            var client = _fixture.Client;
            var name = Guid.NewGuid().ToString();
            var jsonBody = JsonConvert.SerializeObject(name);
            var reqPost = new HttpRequestMessage(HttpMethod.Post, "/Does/Not/Exist")
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(reqPost);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task Test_Post_Garbage()
        {
            var client = _fixture.Client;
            var reqPost = new HttpRequestMessage(HttpMethod.Post, "/api/SomeThing")
            {
                Content = new StringContent("Not a json", Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(reqPost);
            response.StatusCode.ShouldNotBe(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Test_Post_Get_Success()
        {
            var client = _fixture.Client;
            var name = Guid.NewGuid().ToString();
            var jsonBody = JsonConvert.SerializeObject(name);
            var reqPost = new HttpRequestMessage(HttpMethod.Post, "/api/SomeThing")
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(reqPost);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            var reqGet = new HttpRequestMessage(HttpMethod.Get, "/api/SomeThing");

            response = await client.SendAsync(reqGet);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();
            jsonString.ShouldNotBeNullOrWhiteSpace();

            var dogs = JsonConvert.DeserializeObject<List<string>>(jsonString);
            dogs.ShouldNotBeNull();
            dogs.Count.ShouldBeGreaterThan(0);

            dogs[0].ShouldBe(name);
        }
    }
}
