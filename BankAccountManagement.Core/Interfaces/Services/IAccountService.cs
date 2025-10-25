using BankAccountManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankAccountManagement.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for account operations
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Gets all accounts
        /// </summary>
        /// <returns>Collection of all accounts</returns>
        Task<IEnumerable<Account>> GetAllAccountsAsync();

        /// <summary>
        /// Gets an account by its identifier
        /// </summary>
        /// <param name="id">Account identifier</param>
        /// <returns>Account if found, null otherwise</returns>
        Task<Account> GetAccountByIdAsync(string id);

        /// <summary>
        /// Creates a new account with the specified name and zero balance
        /// </summary>
        /// <param name="accountName">Name for the new account</param>
        /// <returns>Created account</returns>
        Task<Account> CreateAccountAsync(string accountName);

        /// <summary>
        /// Updates an existing account
        /// </summary>
        /// <param name="account">Account to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        Task<bool> UpdateAccountAsync(Account account);

        /// <summary>
        /// Deletes an account by its identifier
        /// </summary>
        /// <param name="id">Account identifier</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteAccountAsync(string id);
    }
}