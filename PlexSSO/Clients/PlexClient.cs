using PlexSSO.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlexSSO.Interfaces.Clients;

namespace PlexSSO.Clients
{
    /// <summary>
    /// Client for Plex API (https://plex.tv/api)
    /// </summary>
    /// <param name="cfg"></param>
    /// <param name="httpClientFactory"></param>
    public class PlexClient(BridgeConfig cfg, IHttpClientFactory httpClientFactory) : IPlexClient
    {
        /// <summary>
        /// Add required Plex headers to the request
        /// </summary>
        /// <param name="req"></param>
        private void AddHeaders(HttpRequestMessage req)
        {
            req.Headers.Add("X-Plex-Client-Identifier", cfg.PlexClientIdentifier);
            req.Headers.Add("X-Plex-Product", cfg.PlexProduct);
            req.Headers.Add("X-Plex-Device", cfg.PlexDevice);
            req.Headers.Add("X-Plex-Platform", cfg.PlexPlatform);
            req.Headers.Add("X-Plex-Version", cfg.PlexVersion);
            req.Headers.Add("X-Plex-Provides", "player");
            req.Headers.Add("Accept", "application/json");
        }

        /// <summary>
        /// Create a new Plex Pin (to be authorized by the user)
        /// </summary>
        /// <returns></returns>
        public async Task<PlexPin> CreatePinAsync()
        {
            var http = httpClientFactory.CreateClient("PlexAPI");
            var req = new HttpRequestMessage(HttpMethod.Post, "pins?strong=false");
            AddHeaders(req);
            var res = await http.SendAsync(req);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var pin = JsonSerializer.Deserialize<PlexPin>(json, JsonOpts())!;
            
            return pin;
        }

        /// <summary>
        /// Try to exchange a previously created Pin for an auth token.
        /// </summary>
        /// <param name="pinId"></param>
        /// <returns></returns>
        public async Task<string?> TryExchangePinForTokenAsync(int pinId)
        {
            var http = httpClientFactory.CreateClient("PlexAPI");
            var req = new HttpRequestMessage(HttpMethod.Get, $"pins/{pinId}");
            AddHeaders(req);
            var res = await http.SendAsync(req);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var pin = JsonSerializer.Deserialize<PlexPin>(json, JsonOpts())!;
            
            return string.IsNullOrWhiteSpace(pin.AuthToken) ? null : pin.AuthToken;
        }

        /// <summary>
        /// Get user details for the given auth token.
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public async Task<PlexUser> GetUserAsync(string authToken)
        {
            var http = httpClientFactory.CreateClient("PlexAPI");
            var req = new HttpRequestMessage(HttpMethod.Get, "user");
            AddHeaders(req);
            req.Headers.Add("X-Plex-Token", authToken);
            var res = await http.SendAsync(req);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<PlexUser>(json, JsonOpts())!;
        }

        /// <summary>
        /// JSON serialization options
        /// </summary>
        /// <returns></returns>
        private static JsonSerializerOptions JsonOpts() => new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
}
