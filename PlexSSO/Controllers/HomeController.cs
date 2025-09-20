using Microsoft.AspNetCore.Mvc;
using PlexSSO.Models;

namespace PlexSSO.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController(BridgeConfig cfg) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { service = "Plex OIDC Bridge", issuer = cfg.Issuer });
    }
}
