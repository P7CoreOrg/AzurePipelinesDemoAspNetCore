using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using DemoIdentityModelExtras;
using DemoLibrary;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shouldly;
using TheWebApp.Controllers;
using Xunit;

namespace xUnit_TheWebAppTests
{
    public class UnitTestSomeThing : IClassFixture<MyTestServerFixture>
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
        public void raw_get_dog()
        {
            var fakeLogger = A.Fake<ILogger<SomeThingController>>();
            var fakeDefaultHttpClientFactory = A.Fake<IDefaultHttpClientFactory>();
            A.CallTo(() => fakeDefaultHttpClientFactory.HttpClient).Returns(null);
            A.CallTo(() => fakeDefaultHttpClientFactory.HttpMessageHandler).Returns(null);
            var fakeDog = A.Fake<IDog>();
            A.CallTo(() => fakeDog.Name).Returns("Fido");

            Fixture fixture = new Fixture();
            var context = fixture.Create<SomeContext<ComplexData>>();

            var controller = new SomeThingController(fakeLogger, null,
                fakeDefaultHttpClientFactory, fakeDog);
            var result = controller.GetDogName(context);
            result.ShouldBeSameAs("Fido");
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
        public async Task Test_Get_SomeThing_Success()
        {
            var client = _fixture.Client;
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/SomeThing/dog")
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
        public async Task Test_Get_SomeThing2_Success()
        {
            var client = _fixture.Client;
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/SomeThing2/dog")
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
        public async Task Test_from_external_Success()
        {

            var client = _fixture.Client;
            var req = new HttpRequestMessage(HttpMethod.Get, "/api/SomeThing/from-external");
            var response = await client.SendAsync(req);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();
            jsonString.ShouldNotBeNullOrWhiteSpace();

            var result = JsonConvert.DeserializeObject<string>(jsonString);
            result.ShouldNotBe("HttpContext:true");

            req = new HttpRequestMessage(HttpMethod.Get, "/api/SomeThing/from-external");

            response = await client.SendAsync(req);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            jsonString = await response.Content.ReadAsStringAsync();
            jsonString.ShouldNotBeNullOrWhiteSpace();

            result = JsonConvert.DeserializeObject<string>(jsonString);
            result.ShouldNotBe("HttpContext:true");

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
            var reqPost = new HttpRequestMessage(HttpMethod.Post, "/api/SomeThing/dog")
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
            var reqPost = new HttpRequestMessage(HttpMethod.Post, "/api/SomeThing/dog")
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(reqPost);
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            var reqGet = new HttpRequestMessage(HttpMethod.Get, "/api/SomeThing/dog");

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
