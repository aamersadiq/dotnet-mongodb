using System;

namespace BankAccountManagement.Core.Entities
{
    /// <summary>
    /// Represents a bank account in the system
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Unique identifier for the account
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the account
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Current balance of the account
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Date and time when the account was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the account was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}