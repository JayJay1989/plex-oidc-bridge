using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlexSSO.Interfaces.Services;
using PlexSSO.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PlexSSO.Data;

namespace PlexSSO.Controllers
{
    [ApiController]
    [Route("token")]
    public class TokenController(BridgeConfig cfg, ITokenSigningService signing, PlexBridgeDb db) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromServices] ICodeStore codes,
            [FromServices] ISessionStore sessions)
        {
            if (!Request.HasFormContentType) return BadRequest("form-encoded required");
            var form = await Request.ReadFormAsync();

            var grantType = form["grant_type"].ToString();
            if (grantType != "authorization_code") return BadRequest("unsupported grant_type");

            var code = form["code"].ToString();
            var clientId = form["client_id"].ToString();
            var clientSecret = form["client_secret"].ToString(); // ignored/optional
            var redirectUri = form["redirect_uri"].ToString();
            var codeVerifier = form["code_verifier"].ToString();

            var client = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId && c.Enabled);
            if (client is null)
            {
                return BadRequest("invalid_client");
            }

            // If client has a secret, require it
            if (!string.IsNullOrEmpty(client.ClientSecret) && client.ClientSecret != clientSecret)
            {
                return BadRequest("invalid_client_secret");
            }
            
            var ic = await codes.ConsumeCodeAsync(code);
            if (ic is null)
            {
                return BadRequest("invalid_code");
            }

            if (!string.Equals(ic.RedirectUri, redirectUri, StringComparison.Ordinal))
            {
                return BadRequest("redirect_uri mismatch");
            }

            if (!string.IsNullOrEmpty(ic.CodeChallenge))
            {
                if (string.IsNullOrEmpty(codeVerifier))
                {
                    return BadRequest("code_verifier required");
                }

                string computed = ic.CodeChallengeMethod == "S256"
                    ? Base64UrlEncoder.Encode(
                        System.Security.Cryptography.SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier)))
                    : codeVerifier;

                if (computed != ic.CodeChallenge)
                {
                    return BadRequest("invalid code_verifier");
                }
            }

            var session = await sessions.GetAsync(ic.SessionId);
            if (session is null)
            {
                return BadRequest("session expired");
            }

            var now = DateTimeOffset.UtcNow;

            var idClaims = new List<Claim>
            {
                new("sub", session.PlexId.ToString()),
                new("email", session.Email ?? ""),
                new("preferred_username", session.Username ?? ""),
                new("picture", session.Picture ?? ""),
                new("iss", cfg.Issuer),
                new("aud", clientId)
            };

            var idToken = new JwtSecurityToken(
                issuer: cfg.Issuer,
                audience: clientId,
                claims: idClaims,
                notBefore: now.UtcDateTime,
                expires: now.AddSeconds(cfg.IdTokenLifetimeSeconds).UtcDateTime,
                signingCredentials: signing.Creds);

            var idTokenStr = new JwtSecurityTokenHandler().WriteToken(idToken);

            var accessToken = new JwtSecurityToken(
                issuer: cfg.Issuer,
                audience: clientId,
                claims: new[]
                {
                    new Claim("sub", session.PlexId.ToString()),
                    new Claim("scope", "openid profile email")
                },
                notBefore: now.UtcDateTime,
                expires: now.AddSeconds(cfg.AccessTokenLifetimeSeconds).UtcDateTime,
                signingCredentials: signing.Creds);

            var accessTokenStr = new JwtSecurityTokenHandler().WriteToken(accessToken);

            return Ok(new
            {
                access_token = accessTokenStr,
                id_token = idTokenStr,
                token_type = "Bearer",
                expires_in = cfg.AccessTokenLifetimeSeconds
            });
        }
    }
}
