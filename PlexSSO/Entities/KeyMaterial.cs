namespace PlexSSO.Entities
{
    /// <summary>
    /// Represents RSA key material used for signing tokens.
    /// </summary>
    public class KeyMaterial
    {
        public int Id { get; set; }
        public string Kid { get; set; } = default!;
        public string RsaPrivatePem { get; set; } = default!; // PKCS#8 PEM
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
