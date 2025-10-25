using BankAccountManagement.Core.Entities;
using BankAccountManagement.Core.Interfaces.Repositories;
using BankAccountManagement.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BankAccountManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of the account repository interface
    /// </summary>
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        /// <summary>
        /// Initializes a new instance of the AccountRepository class
        /// </summary>
        /// <param name="context">MongoDB context</param>
        public AccountRepository(MongoDbContext context) : base(context.Accounts)
        {
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateBalanceAsync(string accountId, decimal newBalance)
        {
            var filter = Builders<Account>.Filter.Eq(a => a.Id, accountId);
            var update = Builders<Account>.Update
                .Set(a => a.Balance, newBalance)
                .Set(a => a.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Gets the ID value from an account entity
        /// </summary>
        /// <param name="entity">Account entity</param>
        /// <returns>Account ID</returns>
        protected override string GetId(Account entity)
        {
            return entity.Id;
        }
    }
}