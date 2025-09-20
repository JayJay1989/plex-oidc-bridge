using Microsoft.EntityFrameworkCore;
using PlexSSO.Data;
using PlexSSO.Entities;
using PlexSSO.Interfaces.Services;

namespace PlexSSO.Services
{
    /// <summary>
    /// Entity Framework Core implementation of <see cref="ICodeStore"/>.
    /// </summary>
    public class EfCodeStore(PlexBridgeDb db) : ICodeStore
    {
        /// <summary>
        /// Creates a new authorization transaction and returns its ID.
        /// </summary>
        /// <param name="txn"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<string> CreateAuthTxnAsync(AuthTxn txn, CancellationToken ct = default)
        {
            txn.CreatedAt = DateTimeOffset.UtcNow;
            db.AuthTxns.Add(txn);
            await db.SaveChangesAsync(ct);
            return txn.Id;
        }

        /// <summary>
        /// Retrieves an existing authorization transaction by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public Task<AuthTxn?> GetAuthTxnAsync(string id, CancellationToken ct = default)
            => db.AuthTxns.FirstOrDefaultAsync(t => t.Id == id, ct);

        /// <summary>
        /// Updates an existing authorization transaction.
        /// </summary>
        /// <param name="txn"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task UpdateAuthTxnAsync(AuthTxn txn, CancellationToken ct = default)
        {
            db.AuthTxns.Update(txn);
            await db.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Issues a new authorization code and returns it.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<string> IssueCodeAsync(IssuedCode code, CancellationToken ct = default)
        {
            db.Codes.Add(code);
            await db.SaveChangesAsync(ct);
            return code.Code;
        }


        /// <summary>
        /// Consumes (invalidates) an issued authorization code and returns its details.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<IssuedCode?> ConsumeCodeAsync(string code, CancellationToken ct = default)
        {
            var ic = await db.Codes.FirstOrDefaultAsync(c => c.Code == code, ct);
            if (ic == null) return null;
            if (ic.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                db.Codes.Remove(ic);
                await db.SaveChangesAsync(ct);
                return null;
            }
            db.Codes.Remove(ic);
            await db.SaveChangesAsync(ct);
            return ic;
        }
    }
}
