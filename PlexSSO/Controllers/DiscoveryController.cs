using Microsoft.AspNetCore.Mvc;
using PlexSSO.Models;

namespace PlexSSO.Controllers
{
    [ApiController]
    [Route(".well-known")]
    public class DiscoveryController(BridgeConfig cfg) : ControllerBase
    {
        [HttpGet("openid-configuration")]
        public IActionResult OpenIdConfig()
        {
            var baseUrl = cfg.Issuer.TrimEnd('/');
            return Ok(new
            {
                issuer = baseUrl,
                authorization_endpoint = $"{baseUrl}/authorize",
                token_endpoint = $"{baseUrl}/token",
                userinfo_endpoint = $"{baseUrl}/userinfo",
                jwks_uri = $"{baseUrl}/jwks",
                response_types_supported = new[] { "code" },
                subject_types_supported = new[] { "public" },
                id_token_signing_alg_values_supported = new[] { "RS256" },
                token_endpoint_auth_methods_supported = new[] { "client_secret_post", "client_secret_basic" },
                grant_types_supported = new[] { "authorization_code" },
                code_challenge_methods_supported = new[] { "S256", "plain" }
            });
        }
    }
}
