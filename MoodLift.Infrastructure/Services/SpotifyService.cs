using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace MoodLift.Infrastructure.Services
{
    /// <summary>
    /// Service for interacting with the Spotify Web API.
    /// Handles authentication and track search functionality.
    /// </summary>
    public class SpotifyService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the SpotifyService class.
        /// </summary>
        /// <param name="httpFactory">The HTTP client factory for creating HTTP clients.</param>
        /// <param name="config">The configuration to access Spotify API credentials.</param>
        public SpotifyService(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _config = config;
        }

        /// <summary>
        /// Retrieves an access token from Spotify using client credentials flow.
        /// </summary>
        /// <returns>A Spotify access token as a string.</returns>
        private async Task<string> GetAccessTokenAsync()
        {
            var http = _httpFactory.CreateClient();
            var tokenUrl = "https://accounts.spotify.com/api/token";
            var clientId = _config["Spotify:ClientId"]!;
            var clientSecret = _config["Spotify:ClientSecret"]!;
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString()!;
        }

        /// <summary>
        /// Searches for tracks on Spotify based on a query string.
        /// </summary>
        /// <param name="query">The search query to find tracks.</param>
        /// <param name="limit">The maximum number of tracks to return. Defaults to 8.</param>
        /// <returns>A list of Spotify track IDs matching the search query.</returns>
        /// <remarks>
        /// The method automatically handles authentication by obtaining a new access token.
        /// The search results are filtered to only return valid track IDs.
        /// </remarks>
        public async Task<List<string>> SearchTracksAsync(string query, int limit = 8)
        {
            var token = await GetAccessTokenAsync();
            var http = _httpFactory.CreateClient();

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit={limit}";
            var response = await http.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            var items = doc.RootElement
                .GetProperty("tracks")
                .GetProperty("items")
                .EnumerateArray();

            return items
                .Select(track => track.GetProperty("id").GetString()!)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();
        }
    }
}
