using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace PlexSSO.Controllers
{
    [ApiController]
    [Route("userinfo")]
    public class UserInfoController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var auth = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer "))
                return Unauthorized();

            var token = auth.Substring("Bearer ".Length);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var name = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var picture = jwt.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            return Ok(new
            {
                sub,
                name,
                preferred_username = name,
                email,
                picture
            });
        }
    }
}
