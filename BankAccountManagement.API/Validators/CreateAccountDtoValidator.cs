using BankAccountManagement.API.Models;
using FluentValidation;

namespace BankAccountManagement.API.Validators
{
    /// <summary>
    /// Validator for create account requests
    /// </summary>
    public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
    {
        /// <summary>
        /// Initializes a new instance of the CreateAccountDtoValidator class
        /// </summary>
        public CreateAccountDtoValidator()
        {
            RuleFor(x => x.AccountName)
                .NotEmpty().WithMessage("Account name is required")
                .MinimumLength(3).WithMessage("Account name must be at least 3 characters long")
                .MaximumLength(100).WithMessage("Account name cannot exceed 100 characters");
        }
    }
}