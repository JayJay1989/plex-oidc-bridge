using PlexSSO.Entities;

namespace PlexSSO.Interfaces.Services
{
    /// <summary>
    /// Abstraction for a storage mechanism for authorization transactions and issued codes.
    /// </summary>
    public interface ICodeStore
    {
        /// <summary>
        /// Creates a new authorization transaction and returns its ID.
        /// </summary>
        /// <param name="txn"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<string> CreateAuthTxnAsync(AuthTxn txn, CancellationToken ct = default);

        /// <summary>
        /// Retrieves an existing authorization transaction by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<AuthTxn?> GetAuthTxnAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing authorization transaction.
        /// </summary>
        /// <param name="txn"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task UpdateAuthTxnAsync(AuthTxn txn, CancellationToken ct = default);

        /// <summary>
        /// Issues a new authorization code and returns it.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<string> IssueCodeAsync(IssuedCode code, CancellationToken ct = default);

        /// <summary>
        /// Consumes (invalidates) an issued authorization code and returns its details.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IssuedCode?> ConsumeCodeAsync(string code, CancellationToken ct = default);
    }
}
