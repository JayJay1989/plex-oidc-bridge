namespace PlexSSO.Entities
{
    /// <summary>
    /// Represents an issued authorization code in the OAuth2 authorization code flow.
    /// </summary>
    public class IssuedCode
    {
        public string Code { get; set; } = Guid.NewGuid().ToString("N");
        public string ClientId { get; set; } = default!;
        public string RedirectUri { get; set; } = default!;
        public string State { get; set; } = default!;
        public string SessionId { get; set; } = default!;
        public string? CodeChallenge { get; set; }
        public string? CodeChallengeMethod { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
