using PlexSSO.Models;

namespace PlexSSO.Interfaces.Clients
{
    /// <summary>
    /// Client for Plex API (https://plex.tv/api)
    /// </summary>
    public interface IPlexClient
    {
        /// <summary>
        /// Create a new Plex Pin (to be authorized by the user)
        /// </summary>
        /// <returns></returns>
        Task<PlexPin> CreatePinAsync();

        /// <summary>
        /// Try to exchange a previously created Pin for an auth token.
        /// </summary>
        /// <param name="pinId"></param>
        /// <returns></returns>
        Task<string?> TryExchangePinForTokenAsync(int pinId);

        /// <summary>
        /// Get user details for the given auth token.
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        Task<PlexUser> GetUserAsync(string authToken);
    }
}
