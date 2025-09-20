using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlexSSO.Data;
using PlexSSO.Entities;
using PlexSSO.Interfaces.Clients;
using PlexSSO.Interfaces.Services;
using PlexSSO.Models;

namespace PlexSSO.Controllers
{
    [ApiController]
    public class AuthorizeController(BridgeConfig cfg, IPlexClient plexClient, ICodeStore codes, ISessionStore sessions, PlexBridgeDb db) : ControllerBase
    {
        // GET /authorize
        [HttpGet]
        [Route("authorize")]
        public async Task<IActionResult> Authorize(
            [FromQuery] string client_id,
            [FromQuery] string redirect_uri,
            [FromQuery] string response_type,
            [FromQuery] string scope,
            [FromQuery] string state,
            [FromQuery(Name = "code_challenge")] string codeChallenge,
            [FromQuery(Name = "code_challenge_method")] string codeChallengeMethod,
            CancellationToken ctx = default)
        {
            if (response_type != "code")
            {
                return BadRequest("Only response_type=code supported");
            }

            var client = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == client_id && c.Enabled, ctx);
            if (client is null) 
            {
                return BadRequest("Unknown or disabled client");
            }

            if (!string.Equals(client.RedirectUri, redirect_uri, StringComparison.Ordinal))
            {
                return BadRequest("Invalid redirect_uri");
            }

            var pin = await plexClient.CreatePinAsync();

            var authTxnId = await codes.CreateAuthTxnAsync(new AuthTxn
            {
                ClientId = client_id,
                RedirectUri = redirect_uri,
                State = state,
                CodeChallenge = codeChallenge,
                CodeChallengeMethod = string.IsNullOrWhiteSpace(codeChallengeMethod)
                    ? "plain"
                    : codeChallengeMethod.ToUpperInvariant(),
                CreatedAt = DateTimeOffset.UtcNow,
                PlexPinId = pin.Id,
                PlexPinCode = pin.Code,
                PlexVerifyUrl = "https://www.plex.tv/link/"
            }, ctx);

            return Redirect($"/PlexAuthorize?plexCode={Uri.EscapeDataString(pin.Code)}" +
                            $"&verifyUrl={Uri.EscapeDataString("https://www.plex.tv/link/")}" +
                            $"&redirectUri={Uri.EscapeDataString(redirect_uri)}" +
                            $"&state={Uri.EscapeDataString(state)}" +
                            $"&txnId={Uri.EscapeDataString(authTxnId)}");
        }

        // GET /poll/sse?txn=...
        [HttpGet]
        [Route("poll/sse")]
        public async Task<IActionResult> PollSse(
            [FromQuery] string txn,
            CancellationToken ctx = default)
        {
            var authTxn = await codes.GetAuthTxnAsync(txn, ctx);
            if (authTxn is null) return NotFound();

            Response.Headers.CacheControl = "no-cache";
            Response.Headers.Pragma = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no";
            Response.ContentType = "text/event-stream; charset=utf-8";

            async Task SendEvent(string evt, string data)
            {
                await Response.WriteAsync($"event: {evt}\n", ctx);
                await Response.WriteAsync($"data: {data}\n\n", ctx);
                await Response.Body.FlushAsync(ctx);
            }

            async Task Heartbeat() =>
                await Response.WriteAsync($": keep-alive {DateTimeOffset.UtcNow:O}\n\n", ctx);

            var abort = HttpContext.RequestAborted;
            
            var heartbeatEvery = TimeSpan.FromSeconds(15);
            var nextBeat = DateTimeOffset.UtcNow + heartbeatEvery;

            if (!string.IsNullOrEmpty(authTxn.IssuedCode))
            {
                await SendEvent("complete", $"{{\"code\":\"{authTxn.IssuedCode}\"}}");
                return new EmptyResult();
            }

            while (!abort.IsCancellationRequested)
            {
                var token = await plexClient.TryExchangePinForTokenAsync(authTxn.PlexPinId);
                if (token is null)
                {
                    await SendEvent("waiting", "{\"status\":\"pending\"}");
                }
                else
                {
                    var user = await plexClient.GetUserAsync(token);
                    var session = await sessions.CreateAsync(new BridgeSession
                    {
                        PlexToken = token,
                        PlexId = user.Id,
                        Email = user.Email,
                        Username = user.Username,
                        Picture = user.Thumb
                    }, ctx);

                    var code = new IssuedCode
                    {
                        ClientId = authTxn.ClientId,
                        RedirectUri = authTxn.RedirectUri,
                        State = authTxn.State,
                        SessionId = session.Id,
                        CodeChallenge = authTxn.CodeChallenge,
                        CodeChallengeMethod = authTxn.CodeChallengeMethod,
                        ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(cfg.CodeLifetimeSeconds)
                    };
                    var authCode = await codes.IssueCodeAsync(code, ctx);
                    authTxn.IssuedCode = authCode;
                    await codes.UpdateAuthTxnAsync(authTxn, ctx);

                    await SendEvent("complete", $"{{\"code\":\"{authCode}\"}}");
                    break;
                }

                if (DateTimeOffset.UtcNow >= nextBeat)
                {
                    await Heartbeat();
                    await Response.Body.FlushAsync(ctx);
                    nextBeat = DateTimeOffset.UtcNow + heartbeatEvery;
                }

                await Task.Delay(1500, abort);
            }

            return new EmptyResult();
        }
    }
}
