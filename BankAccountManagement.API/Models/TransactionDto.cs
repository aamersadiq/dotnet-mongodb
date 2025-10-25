using System;

namespace BankAccountManagement.API.Models
{
    /// <summary>
    /// Data transfer object for transaction information
    /// </summary>
    public class TransactionDto
    {
        /// <summary>
        /// Transaction identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Source account identifier
        /// </summary>
        public string FromAccountId { get; set; }

        /// <summary>
        /// Destination account identifier
        /// </summary>
        public string ToAccountId { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Transaction timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Transaction description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Transaction status
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a new transfer
    /// </summary>
    public class TransferDto
    {
        /// <summary>
        /// Source account identifier
        /// </summary>
        public string FromAccountId { get; set; }

        /// <summary>
        /// Destination account identifier
        /// </summary>
        public string ToAccountId { get; set; }

        /// <summary>
        /// Transfer amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Transfer description
        /// </summary>
        public string Description { get; set; }
    }
}