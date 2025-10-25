using FluentValidation;
using BankAccountManagement.API.Models;

namespace BankAccountManagement.API.Validators
{
    /// <summary>
    /// Validator for deposit operations
    /// </summary>
    public class DepositDtoValidator : AbstractValidator<DepositDto>
    {
        public DepositDtoValidator()
        {
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThan(0).WithMessage("Amount must be greater than zero");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}