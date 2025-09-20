namespace PlexSSO.ViewModels.Admin
{
    public class AdminCreateClientModel
    {
        public string ClientId { get; set; } = default!;
        public string? ClientSecret { get; set; }
        public string RedirectUri { get; set; } = default!;
    }
}
