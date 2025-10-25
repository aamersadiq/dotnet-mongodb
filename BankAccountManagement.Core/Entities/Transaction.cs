using System;

namespace BankAccountManagement.Core.Entities
{
    /// <summary>
    /// Represents a transaction between bank accounts
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Unique identifier for the transaction
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Identifier of the source account
        /// </summary>
        public string FromAccountId { get; set; }

        /// <summary>
        /// Identifier of the destination account
        /// </summary>
        public string ToAccountId { get; set; }

        /// <summary>
        /// Amount transferred in the transaction
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Date and time when the transaction occurred
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Description or purpose of the transaction
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Current status of the transaction
        /// </summary>
        public TransactionStatus Status { get; set; }
    }
}