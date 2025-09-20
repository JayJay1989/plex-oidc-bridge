namespace PlexSSO.Entities
{
    public class AdminUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public byte[] PasswordHash { get; set; } = default!;
        public byte[] PasswordSalt { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
