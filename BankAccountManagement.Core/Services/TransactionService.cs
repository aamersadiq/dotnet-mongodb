using BankAccountManagement.Core.Entities;
using BankAccountManagement.Core.Interfaces.Repositories;
using BankAccountManagement.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankAccountManagement.Core.Services
{
    /// <summary>
    /// Implementation of the transaction service interface
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        /// Initializes a new instance of the TransactionService class
        /// </summary>
        /// <param name="transactionRepository">Transaction repository</param>
        /// <param name="accountRepository">Account repository</param>
        public TransactionService(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("Account ID cannot be null or empty", nameof(accountId));
            }

            return await _transactionRepository.GetByAccountIdAsync(accountId);
        }

        /// <inheritdoc/>
        public async Task<Transaction> TransferAsync(string fromAccountId, string toAccountId, decimal amount, string description)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(fromAccountId))
            {
                throw new ArgumentException("Source account ID cannot be null or empty", nameof(fromAccountId));
            }

            if (string.IsNullOrEmpty(toAccountId))
            {
                throw new ArgumentException("Destination account ID cannot be null or empty", nameof(toAccountId));
            }

            if (fromAccountId == toAccountId)
            {
                throw new ArgumentException("Source and destination accounts cannot be the same");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Transfer amount must be greater than zero", nameof(amount));
            }

            // Get accounts
            var fromAccount = await _accountRepository.GetByIdAsync(fromAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(toAccountId);

            // Validate accounts exist
            if (fromAccount == null)
            {
                throw new ArgumentException($"Source account with ID {fromAccountId} not found");
            }

            if (toAccount == null)
            {
                throw new ArgumentException($"Destination account with ID {toAccountId} not found");
            }

            // Validate sufficient balance
            if (fromAccount.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance for transfer");
            }

            // Create transaction
            var transaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = amount,
                Description = description,
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Pending
            };

            try
            {
                // Update account balances
                await _accountRepository.UpdateBalanceAsync(fromAccountId, fromAccount.Balance - amount);
                await _accountRepository.UpdateBalanceAsync(toAccountId, toAccount.Balance + amount);

                // Update transaction status
                transaction.Status = TransactionStatus.Completed;
            }
            catch (Exception)
            {
                transaction.Status = TransactionStatus.Failed;
                throw;
            }

            // Save transaction
            return await _transactionRepository.AddAsync(transaction);
        }

        /// <inheritdoc/>
        public async Task<Transaction> DepositAsync(string accountId, decimal amount, string description)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("Account ID cannot be null or empty", nameof(accountId));
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be greater than zero", nameof(amount));
            }

            // Get account
            var account = await _accountRepository.GetByIdAsync(accountId);

            // Validate account exists
            if (account == null)
            {
                throw new ArgumentException($"Account with ID {accountId} not found");
            }

            // Create transaction
            var transaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                FromAccountId = null, // No source account for deposits
                ToAccountId = accountId,
                Amount = amount,
                Description = description ?? "Deposit",
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Pending
            };

            try
            {
                // Update account balance
                await _accountRepository.UpdateBalanceAsync(accountId, account.Balance + amount);

                // Update transaction status
                transaction.Status = TransactionStatus.Completed;
            }
            catch (Exception)
            {
                transaction.Status = TransactionStatus.Failed;
                throw;
            }

            // Save transaction
            return await _transactionRepository.AddAsync(transaction);
        }

        /// <inheritdoc/>
        public async Task<Transaction> WithdrawAsync(string accountId, decimal amount, string description)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("Account ID cannot be null or empty", nameof(accountId));
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be greater than zero", nameof(amount));
            }

            // Get account
            var account = await _accountRepository.GetByIdAsync(accountId);

            // Validate account exists
            if (account == null)
            {
                throw new ArgumentException($"Account with ID {accountId} not found");
            }

            // Validate sufficient balance
            if (account.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance for withdrawal");
            }

            // Create transaction
            var transaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                FromAccountId = accountId,
                ToAccountId = null, // No destination account for withdrawals
                Amount = amount,
                Description = description ?? "Withdrawal",
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Pending
            };

            try
            {
                // Update account balance
                await _accountRepository.UpdateBalanceAsync(accountId, account.Balance - amount);

                // Update transaction status
                transaction.Status = TransactionStatus.Completed;
            }
            catch (Exception)
            {
                transaction.Status = TransactionStatus.Failed;
                throw;
            }

            // Save transaction
            return await _transactionRepository.AddAsync(transaction);
        }
    }
}