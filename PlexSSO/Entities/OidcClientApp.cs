namespace PlexSSO.Entities
{
    public class OidcClientApp
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = default!;
        public string? ClientSecret { get; set; } // optional; null means public client
        public string RedirectUri { get; set; } = default!;
        public bool Enabled { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
