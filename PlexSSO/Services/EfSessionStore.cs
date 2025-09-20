using Microsoft.EntityFrameworkCore;
using PlexSSO.Data;
using PlexSSO.Entities;
using PlexSSO.Interfaces.Services;

namespace PlexSSO.Services
{
    /// <summary>
    /// Entity Framework implementation of <see cref="ISessionStore"/>.
    /// </summary>
    /// <param name="db"></param>
    public class EfSessionStore(PlexBridgeDb db) : ISessionStore
    {
        /// <summary>
        /// Creates a new BridgeSession.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<BridgeSession> CreateAsync(BridgeSession s, CancellationToken ct = default)
        {
            db.Sessions.Add(s);
            await db.SaveChangesAsync(ct);
            return s;
        }

        /// <summary>
        /// Retrieves a BridgeSession by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<BridgeSession?> GetAsync(string id, CancellationToken ct = default)
            => db.Sessions.FirstOrDefaultAsync(s => s.Id == id, ct);
    }
}
