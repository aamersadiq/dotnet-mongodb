using BankAccountManagement.Core.Entities;
using BankAccountManagement.Core.Interfaces.Repositories;
using BankAccountManagement.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankAccountManagement.Core.Services
{
    /// <summary>
    /// Implementation of the account service interface
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        /// Initializes a new instance of the AccountService class
        /// </summary>
        /// <param name="accountRepository">Account repository</param>
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<Account> GetAccountByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Account ID cannot be null or empty", nameof(id));
            }

            return await _accountRepository.GetByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task<Account> CreateAccountAsync(string accountName)
        {
            if (string.IsNullOrEmpty(accountName))
            {
                throw new ArgumentException("Account name cannot be null or empty", nameof(accountName));
            }

            var account = new Account
            {
                Id = Guid.NewGuid().ToString(),
                AccountName = accountName,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _accountRepository.AddAsync(account);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAccountAsync(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (string.IsNullOrEmpty(account.Id))
            {
                throw new ArgumentException("Account ID cannot be null or empty", nameof(account.Id));
            }

            account.UpdatedAt = DateTime.UtcNow;
            return await _accountRepository.UpdateAsync(account);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAccountAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Account ID cannot be null or empty", nameof(id));
            }

            return await _accountRepository.DeleteAsync(id);
        }
    }
}