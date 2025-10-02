using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace MoodLift.Infrastructure.Services
{
    public class SpotifyService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;

        public SpotifyService(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _config = config;
        }

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
