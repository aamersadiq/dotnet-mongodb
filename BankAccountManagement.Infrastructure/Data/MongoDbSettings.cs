namespace BankAccountManagement.Infrastructure.Data
{
    /// <summary>
    /// MongoDB connection settings
    /// </summary>
    public class MongoDbSettings
    {
        /// <summary>
        /// MongoDB connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// MongoDB database name
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// MongoDB collection name for accounts
        /// </summary>
        public string AccountsCollectionName { get; set; }

        /// <summary>
        /// MongoDB collection name for transactions
        /// </summary>
        public string TransactionsCollectionName { get; set; }
    }
}