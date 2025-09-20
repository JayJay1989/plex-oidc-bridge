using PlexSSO.Entities;

namespace PlexSSO.ViewModels.Admin
{
    public class AdminIndexModel
    {
        public List<OidcClientApp> Clients { get; set; } = new();

        public OidcClientApp Post { get; set; }
    }
}
