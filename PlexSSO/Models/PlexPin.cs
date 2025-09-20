using System.Text.Json.Serialization;

namespace PlexSSO.Models
{
    /// <summary>
    /// Plex Pin model
    /// </summary>
    public record PlexPin
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("code")] public string Code { get; set; } = default!;
        [JsonPropertyName("authToken")] public string? AuthToken { get; set; }
        [JsonPropertyName("expiresIn")] public int ExpiresIn { get; set; }
        [JsonPropertyName("verifyUrl")] public string VerifyUrl { get; set; } = default!;
    }
}
