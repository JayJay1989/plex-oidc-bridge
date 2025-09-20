namespace PlexSSO.Entities
{
    /// <summary>
    /// Represents an authentication transaction for OAuth2 authorization code flow.
    /// </summary>
    public class AuthTxn
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ClientId { get; set; } = default!;
        public string RedirectUri { get; set; } = default!;
        public string State { get; set; } = default!;
        public string? CodeChallenge { get; set; }
        public string? CodeChallengeMethod { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public int PlexPinId { get; set; }
        public string PlexPinCode { get; set; } = default!;
        public string PlexVerifyUrl { get; set; } = default!;

        public string? IssuedCode { get; set; }
    }
}
