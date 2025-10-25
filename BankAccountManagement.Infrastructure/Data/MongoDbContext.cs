using BankAccountManagement.Core.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BankAccountManagement.Infrastructure.Data
{
    /// <summary>
    /// MongoDB context for database operations
    /// </summary>
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        /// <summary>
        /// Initializes a new instance of the MongoDbContext class
        /// </summary>
        /// <param name="settings">MongoDB settings</param>
        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
        }

        /// <summary>
        /// Gets the accounts collection
        /// </summary>
        public IMongoCollection<Account> Accounts => 
            _database.GetCollection<Account>(_settings.AccountsCollectionName);

        /// <summary>
        /// Gets the transactions collection
        /// </summary>
        public IMongoCollection<Transaction> Transactions => 
            _database.GetCollection<Transaction>(_settings.TransactionsCollectionName);
    }
}