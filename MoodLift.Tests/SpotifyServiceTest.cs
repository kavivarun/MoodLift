using Microsoft.Extensions.Configuration;
using MoodLift.Infrastructure.Services;
using Moq;
using System.Net;
using System.Text;

namespace MoodLift.Tests
{
        public class SpotifyServiceTest
        {
            private Mock<IHttpClientFactory> _httpFactoryMock = null!;
            private IConfiguration _config = null!;

            [SetUp]
            public void Setup()
            {
                _httpFactoryMock = new Mock<IHttpClientFactory>();

                // Fake config with dummy Spotify keys
                var inMemorySettings = new Dictionary<string, string?>
            {
                {"Spotify:ClientId", "test-client-id"},
                {"Spotify:ClientSecret", "test-client-secret"}
            };
                _config = new ConfigurationBuilder()
                    .AddInMemoryCollection(inMemorySettings)
                    .Build();
            }

            [Test]
            public async Task SearchTracksAsync_ReturnsTrackIds()
            {
                // Arrange: first response is token, second response is search results
                var tokenJson = "{\"access_token\":\"fake-token\"}";
                var trackJson = @"
            {
                ""tracks"": {
                    ""items"": [
                        { ""id"": ""track1"" },
                        { ""id"": ""track2"" }
                    ]
                }
            }";

                var responses = new Queue<HttpResponseMessage>(new[]
                {
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(tokenJson, Encoding.UTF8, "application/json")
                },
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(trackJson, Encoding.UTF8, "application/json")
                }
            });

                var client = new HttpClient(new FakeHttpMessageHandler(() => responses.Dequeue()));
                _httpFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

                var service = new SpotifyService(_httpFactoryMock.Object, _config);

                // Act
                var tracks = await service.SearchTracksAsync("test");

                // Assert
                Assert.That(tracks, Is.EquivalentTo(new[] { "track1", "track2" }));
            }

            // Helper fake handler
            private class FakeHttpMessageHandler : HttpMessageHandler
            {
                private readonly Func<HttpResponseMessage> _responseFactory;

                public FakeHttpMessageHandler(Func<HttpResponseMessage> responseFactory)
                {
                    _responseFactory = responseFactory;
                }

                protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    return Task.FromResult(_responseFactory());
                }
            }
        }
    }

