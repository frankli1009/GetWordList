using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace GetWordsAPI.Tests
{
    [Collection("Integration Tests")]
    public class GetWordsServiceControllerTests
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public GetWordsServiceControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetWords_SuccessOnWholeList()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/gws/skhier/5");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseObject = JsonConvert.DeserializeObject<List<string>>(
                await response.Content.ReadAsStringAsync());
            Assert.Equal(720, responseObject?.Count);
        }

        [Fact]
        public async Task GetWords_SuccessOnExtraRegExp()
        {
            // Arrange
            var client = _factory.CreateClient();
            string extra = "..i..";

            // Act
            var response = await client.GetAsync("/gws/skhier/5/" +
                HttpUtility.UrlEncode(extra));

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseObject = JsonConvert.DeserializeObject<List<string>>(
                await response.Content.ReadAsStringAsync());
            Assert.Equal(120, responseObject?.Count);
        }

        [Fact]
        public async Task GetWords_SuccessOnRejected()
        {
            // Arrange
            var client = _factory.CreateClient();
            List<string> rejected = new List<string>();
            rejected.Add("shire");
            rejected.Add("shirk");
            var rejectedJson = JsonConvert.SerializeObject(rejected);

            // Act
            var response = await client.GetAsync("/gws/skhier/5?rejected=" +
                HttpUtility.UrlEncode(rejectedJson));

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseObject = JsonConvert.DeserializeObject<List<string>>(
                await response.Content.ReadAsStringAsync());
            Assert.Equal(718, responseObject?.Count);
        }

        [Fact]
        public async Task GetWords_SuccessOnRegExpAndRejected()
        {
            // Arrange
            var client = _factory.CreateClient();
            string extra = "..i..";
            List<string> rejected = new List<string>();
            rejected.Add("shire");
            rejected.Add("shirk");
            var rejectedJson = JsonConvert.SerializeObject(rejected);

            // Act
            var response = await client.GetAsync("/gws/skhier/5/" +
                HttpUtility.UrlEncode(extra) + "?rejected=" +
                HttpUtility.UrlEncode(rejectedJson));

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var responseObject = JsonConvert.DeserializeObject<List<string>>(
                await response.Content.ReadAsStringAsync());
            Assert.Equal(118, responseObject?.Count);
        }

        [Theory]
        [InlineData("/health", "Healthy")]
        public async Task GetPath_ReturnsSuccessAndExpectedStatus(string path, string expectedStatus)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(path);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response.Content);
            var responseObject = JsonConvert.DeserializeObject<ResponseType>(
                await response.Content.ReadAsStringAsync());
            Assert.Equal(expectedStatus, responseObject?.Status);
        }

        private class ResponseType
        {
            public string Status { get; set; }
        }
    }
}
