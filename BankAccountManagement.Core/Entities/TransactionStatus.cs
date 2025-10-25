namespace BankAccountManagement.Core.Entities
{
    /// <summary>
    /// Represents the status of a transaction
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Transaction is pending processing
        /// </summary>
        Pending,

        /// <summary>
        /// Transaction has been completed successfully
        /// </summary>
        Completed,

        /// <summary>
        /// Transaction has failed
        /// </summary>
        Failed
    }
}