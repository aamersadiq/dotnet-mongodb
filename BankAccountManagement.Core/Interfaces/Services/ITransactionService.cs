using BankAccountManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankAccountManagement.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for transaction operations
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Gets all transactions for a specific account
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <returns>Collection of transactions involving the specified account</returns>
        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(string accountId);

        /// <summary>
        /// Transfers money from one account to another
        /// </summary>
        /// <param name="fromAccountId">Source account identifier</param>
        /// <param name="toAccountId">Destination account identifier</param>
        /// <param name="amount">Amount to transfer</param>
        /// <param name="description">Description of the transaction</param>
        /// <returns>Created transaction</returns>
        /// <exception cref="System.ArgumentException">Thrown when one or both accounts do not exist</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when source account has insufficient balance</exception>
        Task<Transaction> TransferAsync(string fromAccountId, string toAccountId, decimal amount, string description);

        /// <summary>
        /// Deposits money into an account
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <param name="amount">Amount to deposit</param>
        /// <param name="description">Description of the transaction</param>
        /// <returns>Created transaction</returns>
        /// <exception cref="System.ArgumentException">Thrown when account does not exist</exception>
        /// <exception cref="System.ArgumentException">Thrown when amount is less than or equal to zero</exception>
        Task<Transaction> DepositAsync(string accountId, decimal amount, string description);

        /// <summary>
        /// Withdraws money from an account
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <param name="amount">Amount to withdraw</param>
        /// <param name="description">Description of the transaction</param>
        /// <returns>Created transaction</returns>
        /// <exception cref="System.ArgumentException">Thrown when account does not exist</exception>
        /// <exception cref="System.ArgumentException">Thrown when amount is less than or equal to zero</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when account has insufficient balance</exception>
        Task<Transaction> WithdrawAsync(string accountId, decimal amount, string description);
    }
}