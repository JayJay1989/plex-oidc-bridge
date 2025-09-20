using System.Text.Json.Serialization;

namespace PlexSSO.Models
{
    /// <summary>
    /// Plex User model
    /// </summary>
    public record PlexUser
    {
        [JsonPropertyName("id")] public long Id { get; set; }
        [JsonPropertyName("username")] public string? Username { get; set; }
        [JsonPropertyName("email")] public string? Email { get; set; }
        [JsonPropertyName("thumb")] public string? Thumb { get; set; }
        public string? Picture => Thumb;
    }
}
