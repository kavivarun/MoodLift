using Microsoft.Extensions.Configuration;
using MoodLift.Infrastructure.Services;
using Moq;
using System.Net;
using System.Text;

namespace MoodLift.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="SpotifyService"/> class.
    /// Tests the functionality of Spotify API integration including authentication and track searching.
    /// </summary>
    public class SpotifyServiceTest
    {
        private Mock<IHttpClientFactory> _httpFactoryMock = null!;
        private IConfiguration _config = null!;

        /// <summary>
        /// Sets up the test environment before each test execution.
        /// Initializes mocked dependencies including HTTP client factory and configuration.
        /// </summary>
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

        /// <summary>
        /// Tests that the <see cref="SpotifyService.SearchTracksAsync"/> method returns the expected track IDs
        /// when provided with a search query.
        /// </summary>
        /// <remarks>
        /// This test verifies the complete flow:
        /// 1. Authentication token retrieval from Spotify
        /// 2. Track search API call
        /// 3. Proper parsing of JSON response
        /// 4. Extraction of track IDs from the response
        /// </remarks>
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

        /// <summary>
        /// A fake HTTP message handler used for testing HTTP requests without making actual network calls.
        /// This handler allows controlled responses to be returned for testing purposes.
        /// </summary>
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpResponseMessage> _responseFactory;

            /// <summary>
            /// Initializes a new instance of the <see cref="FakeHttpMessageHandler"/> class.
            /// </summary>
            /// <param name="responseFactory">A factory function that provides HTTP responses for each request.</param>
            public FakeHttpMessageHandler(Func<HttpResponseMessage> responseFactory)
            {
                _responseFactory = responseFactory;
            }

            /// <summary>
            /// Handles HTTP requests by returning pre-configured responses instead of making actual network calls.
            /// </summary>
            /// <param name="request">The HTTP request message (not used in this fake implementation).</param>
            /// <param name="cancellationToken">The cancellation token (not used in this fake implementation).</param>
            /// <returns>A task that represents the asynchronous operation, containing the configured HTTP response.</returns>
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_responseFactory());
            }
        }
    }
}

