using System.ComponentModel.DataAnnotations;

namespace BankAccountManagement.API.Models
{
    /// <summary>
    /// Data transfer object for withdrawal operations
    /// </summary>
    public class WithdrawDto
    {
        /// <summary>
        /// Amount to withdraw
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// Description of the withdrawal
        /// </summary>
        public string Description { get; set; }
    }
}