using BankAccountManagement.Core.Entities;
using System.Threading.Tasks;

namespace BankAccountManagement.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for Account entity operations
    /// </summary>
    public interface IAccountRepository : IRepository<Account>
    {
        /// <summary>
        /// Updates the balance of an account
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <param name="newBalance">New balance value</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateBalanceAsync(string accountId, decimal newBalance);
    }
}