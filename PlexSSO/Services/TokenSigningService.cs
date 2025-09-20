using Microsoft.IdentityModel.Tokens;
using PlexSSO.Interfaces.Services;
using System.Security.Cryptography;

namespace PlexSSO.Services
{
    /// <summary>
    /// Provides RSA, KeyId, and SigningCredentials by loading/creating RSA via IKeyStore once.
    /// </summary>
    public class TokenSigningService : ITokenSigningService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Lazy<(RSA Rsa, string KeyId, SigningCredentials Creds)> _lazy;

        public TokenSigningService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _lazy = new Lazy<(RSA, string, SigningCredentials)>(Init, isThreadSafe: true);
        }

        /// <summary>
        /// Initializes the RSA, KeyId, and SigningCredentials by retrieving them from IKeyStore.
        /// </summary>
        /// <returns></returns>
        private (RSA Rsa, string KeyId, SigningCredentials Creds) Init()
        {
            using var scope = _scopeFactory.CreateScope();
            var ks = scope.ServiceProvider.GetRequiredService<IKeyStore>();
            var (rsa, keyId) = ks.GetOrCreateRsa();
            var key = new RsaSecurityKey(rsa) { KeyId = keyId };
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            return (rsa, keyId, creds);
        }

        public RSA Rsa => _lazy.Value.Rsa;
        public string KeyId => _lazy.Value.KeyId;
        public SecurityKey SecurityKey => new RsaSecurityKey(Rsa) { KeyId = KeyId };
        public SigningCredentials Creds => _lazy.Value.Creds;
    }
}
