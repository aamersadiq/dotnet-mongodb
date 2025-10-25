using BankAccountManagement.API.Models;
using FluentValidation;

namespace BankAccountManagement.API.Validators
{
    /// <summary>
    /// Validator for transfer requests
    /// </summary>
    public class TransferDtoValidator : AbstractValidator<TransferDto>
    {
        /// <summary>
        /// Initializes a new instance of the TransferDtoValidator class
        /// </summary>
        public TransferDtoValidator()
        {
            RuleFor(x => x.FromAccountId)
                .NotEmpty().WithMessage("Source account ID is required");

            RuleFor(x => x.ToAccountId)
                .NotEmpty().WithMessage("Destination account ID is required")
                .NotEqual(x => x.FromAccountId).WithMessage("Source and destination accounts cannot be the same");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}