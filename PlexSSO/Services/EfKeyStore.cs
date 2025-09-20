using PlexSSO.Data;
using PlexSSO.Entities;
using PlexSSO.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PlexSSO.Services
{
    /// <summary>
    /// Implements <see cref="IKeyStore"/> using Entity Framework Core for persistence.
    /// </summary>
    public class EfKeyStore : IKeyStore
    {
        private readonly PlexBridgeDb _db;
        private (RSA Rsa, string Kid)? _cached;

        public EfKeyStore(PlexBridgeDb db) => _db = db;

        /// <summary>
        /// Gets an existing RSA key pair or creates a new one if none exist.
        /// </summary>
        /// <returns></returns>
        public (RSA Rsa, string Kid) GetOrCreateRsa()
        {
            if (_cached.HasValue) return _cached.Value;

            var row = _db.Keys.AsNoTracking().OrderByDescending(k => k.Id).FirstOrDefault();
            if (row == null)
            {
                using var rsa = RSA.Create(2048);
                var pkcs8 = ExportPkcs8Pem(rsa);
                var kid = Guid.NewGuid().ToString("N");
                row = new KeyMaterial { Kid = kid, RsaPrivatePem = pkcs8 };
                _db.Keys.Add(row);
                _db.SaveChanges();
            }

            var key = RSA.Create();
            key.ImportFromPem(row.RsaPrivatePem);
            _cached = (key, row.Kid);
            return _cached.Value;
        }

        /// <summary>
        /// Exports the RSA private key in PKCS#8 PEM format.
        /// </summary>
        /// <param name="rsa"></param>
        /// <returns></returns>
        private static string ExportPkcs8Pem(RSA rsa)
        {
            var builder = new StringBuilder();
            builder.AppendLine("-----BEGIN PRIVATE KEY-----");
            builder.AppendLine(Convert.ToBase64String(rsa.ExportPkcs8PrivateKey(), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END PRIVATE KEY-----");
            return builder.ToString();
        }
    }
}
