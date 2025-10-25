namespace BankAccountManagement.API.Models
{
    /// <summary>
    /// Data transfer object for account information
    /// </summary>
    public class AccountDto
    {
        /// <summary>
        /// Account identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Account name
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Current balance
        /// </summary>
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a new account
    /// </summary>
    public class CreateAccountDto
    {
        /// <summary>
        /// Account name
        /// </summary>
        public string AccountName { get; set; }
    }
}