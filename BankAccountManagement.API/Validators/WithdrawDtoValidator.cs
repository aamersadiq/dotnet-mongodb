using FluentValidation;
using BankAccountManagement.API.Models;

namespace BankAccountManagement.API.Validators
{
    /// <summary>
    /// Validator for withdrawal operations
    /// </summary>
    public class WithdrawDtoValidator : AbstractValidator<WithdrawDto>
    {
        public WithdrawDtoValidator()
        {
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThan(0).WithMessage("Amount must be greater than zero");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}