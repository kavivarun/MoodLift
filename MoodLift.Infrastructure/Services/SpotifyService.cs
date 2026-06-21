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
        private const int MaxLimit = 50;

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
            var clientId = _config["Spotify:ClientId"];
            var clientSecret = _config["Spotify:ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new InvalidOperationException("Spotify credentials are missing. Add Spotify:ClientId and Spotify:ClientSecret to configuration.");
            }

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            using var response = await http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Spotify authentication failed ({(int)response.StatusCode}): {GetSpotifyErrorMessage(json)}");
            }

            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("access_token", out var tokenElement))
            {
                throw new InvalidOperationException("Spotify authentication response did not include an access token.");
            }

            var token = tokenElement.GetString();
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("Spotify authentication response included an empty access token.");
            }

            return token;
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
            if (string.IsNullOrWhiteSpace(query))
            {
                query = "happy upbeat songs";
            }

            limit = Math.Clamp(limit, 1, MaxLimit);

            var token = await GetAccessTokenAsync();
            var http = _httpFactory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, BuildSearchUrl(query, limit));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Spotify track search failed ({(int)response.StatusCode}): {GetSpotifyErrorMessage(json)}");
            }

            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("tracks", out var tracksElement) ||
                !tracksElement.TryGetProperty("items", out var itemsElement) ||
                itemsElement.ValueKind != JsonValueKind.Array)
            {
                return new List<string>();
            }

            return itemsElement
                .EnumerateArray()
                .Where(track => track.TryGetProperty("id", out _))
                .Select(track => track.GetProperty("id").GetString())
                .Where(id => !string.IsNullOrEmpty(id))
                .Cast<string>()
                .ToList();
        }

        private static string BuildSearchUrl(string query, int limit)
        {
            return $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit={limit}";
        }

        private static string GetSpotifyErrorMessage(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return "No response body returned.";
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("error_description", out var description))
                {
                    return description.GetString() ?? json;
                }

                if (doc.RootElement.TryGetProperty("error", out var error))
                {
                    if (error.ValueKind == JsonValueKind.Object &&
                        error.TryGetProperty("message", out var message))
                    {
                        return message.GetString() ?? json;
                    }

                    return error.GetString() ?? json;
                }
            }
            catch (JsonException)
            {
                return json;
            }

            return json;
        }
    }
}
