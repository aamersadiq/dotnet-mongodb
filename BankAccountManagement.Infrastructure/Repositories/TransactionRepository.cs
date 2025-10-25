using BankAccountManagement.Core.Entities;
using BankAccountManagement.Core.Interfaces.Repositories;
using BankAccountManagement.Infrastructure.Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankAccountManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of the transaction repository interface
    /// </summary>
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        /// <summary>
        /// Initializes a new instance of the TransactionRepository class
        /// </summary>
        /// <param name="context">MongoDB context</param>
        public TransactionRepository(MongoDbContext context) : base(context.Transactions)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId)
        {
            var filter = Builders<Transaction>.Filter.Or(
                Builders<Transaction>.Filter.Eq(t => t.FromAccountId, accountId),
                Builders<Transaction>.Filter.Eq(t => t.ToAccountId, accountId)
            );

            return await _collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Gets the ID value from a transaction entity
        /// </summary>
        /// <param name="entity">Transaction entity</param>
        /// <returns>Transaction ID</returns>
        protected override string GetId(Transaction entity)
        {
            return entity.Id;
        }
    }
}