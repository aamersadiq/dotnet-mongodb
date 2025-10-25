using BankAccountManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankAccountManagement.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Transaction entity operations
    /// </summary>
    public interface ITransactionRepository : IRepository<Transaction>
    {
        /// <summary>
        /// Gets all transactions for a specific account (either as source or destination)
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <returns>Collection of transactions involving the specified account</returns>
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId);
    }
}