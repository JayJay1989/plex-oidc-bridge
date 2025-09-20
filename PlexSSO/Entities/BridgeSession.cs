namespace PlexSSO.Entities
{
    /// <summary>
    /// Represents a session bridging Plex authentication with the SSO system.
    /// </summary>
    public class BridgeSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string PlexToken { get; set; } = default!;
        public long PlexId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Picture { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
