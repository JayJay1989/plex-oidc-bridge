using PlexSSO.Entities;

namespace PlexSSO.Interfaces.Services
{
    /// <summary>
    /// Session store interface for managing BridgeSession entities.
    /// </summary>
    public interface ISessionStore
    {
        /// <summary>
        /// Creates a new BridgeSession.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<BridgeSession> CreateAsync(BridgeSession s, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a BridgeSession by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<BridgeSession?> GetAsync(string id, CancellationToken ct = default);
    }
}
