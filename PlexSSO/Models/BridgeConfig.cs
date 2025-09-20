namespace PlexSSO.Models
{
    /// <summary>
    /// Configuration settings for the Plex SSO Bridge.
    /// </summary>
    public record BridgeConfig
    {
        /// <summary>
        /// The issuer URL for the identity provider.
        /// </summary>
        public required string Issuer { get; set; }

        // Plex headers
        /// <summary>
        /// The unique identifier for the Plex client application.
        /// </summary>
        public required string PlexClientIdentifier { get; set; }

        /// <summary>
        /// The name of the Plex client application.
        /// </summary>
        public required string PlexProduct { get; set; }

        /// <summary>
        /// The device name of the Plex client application.
        /// </summary>
        public required string PlexDevice { get; set; }

        /// <summary>
        /// The platform of the Plex client application.
        /// </summary>
        public required string PlexPlatform { get; set; }

        /// <summary>
        /// The version of the Plex client application.
        /// </summary>
        public required string PlexVersion { get; set; }

        /// <summary>
        /// The lifetime of authorization codes in seconds.
        /// </summary>
        public int CodeLifetimeSeconds { get; set; }

        /// <summary>
        /// The lifetime of ID tokens in seconds.
        /// </summary>
        public int IdTokenLifetimeSeconds { get; set; }

        /// <summary>
        /// The lifetime of access tokens in seconds.
        /// </summary>
        public int AccessTokenLifetimeSeconds { get; set; }
    }
}
