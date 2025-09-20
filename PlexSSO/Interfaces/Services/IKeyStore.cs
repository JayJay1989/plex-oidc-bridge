using System.Security.Cryptography;

namespace PlexSSO.Interfaces.Services
{
    /// <summary>
    /// Manages RSA key pairs for signing tokens.
    /// </summary>
    public interface IKeyStore
    {
        /// <summary>
        /// Gets an existing RSA key pair or creates a new one if none exist.
        /// </summary>
        /// <returns></returns>
        (RSA Rsa, string Kid) GetOrCreateRsa();
    }
}
