using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PlexSSO.Pages
{
    public class PlexAuthorizeModel : PageModel
    {
        public string PlexCode { get; set; } = default!;
        public string VerifyUrl { get; set; } = default!;
        public string RedirectUri { get; set; } = default!;
        public string State { get; set; } = default!;
        public string TxnId { get; set; } = default!;

        public void OnGet(string plexCode, string verifyUrl, string redirectUri, string state, string txnId)
        {
            PlexCode = plexCode;
            VerifyUrl = verifyUrl;
            RedirectUri = redirectUri;
            State = state;
            TxnId = txnId;
        }
    }
}
