using System.ComponentModel.DataAnnotations;

namespace BankAccountManagement.API.Models
{
    /// <summary>
    /// Data transfer object for deposit operations
    /// </summary>
    public class DepositDto
    {
        /// <summary>
        /// Amount to deposit
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// Description of the deposit
        /// </summary>
        public string Description { get; set; }
    }
}