using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace PlexSSO.Interfaces.Services
{
    /// <summary>
    /// Provides RSA, KeyId, and SigningCredentials by loading/creating RSA via IKeyStore once.
    /// </summary>
    public interface ITokenSigningService
    {
        /// <summary>
        /// The RSA instance used for signing tokens.
        /// </summary>
        RSA Rsa { get; }

        /// <summary>
        /// The KeyId (kid) associated with the RSA key.
        /// </summary>
        string KeyId { get; }

        /// <summary>
        /// The SecurityKey representation of the RSA key.
        /// </summary>
        SecurityKey SecurityKey { get; }

        /// <summary>
        /// The SigningCredentials used for signing tokens.
        /// </summary>
        SigningCredentials Creds { get; }
    }
}
