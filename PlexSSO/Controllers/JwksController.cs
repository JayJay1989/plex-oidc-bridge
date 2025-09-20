using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlexSSO.Interfaces.Services;

namespace PlexSSO.Controllers
{
    [ApiController]
    [Route("jwks")]
    public class JwksController : ControllerBase
    {
        private readonly ITokenSigningService _signing;

        public JwksController(ITokenSigningService signing) => _signing = signing;

        [HttpGet]
        public IActionResult Get()
        {
            var rsaParams = _signing.Rsa.ExportParameters(false);
            string B64(byte[] x) => Base64UrlEncoder.Encode(x);

            var jwk = new
            {
                kty = "RSA",
                use = "sig",
                kid = _signing.KeyId,
                e = B64(rsaParams.Exponent!),
                n = B64(rsaParams.Modulus!)
            };
            return Ok(new { keys = new[] { jwk } });
        }
    }
}
