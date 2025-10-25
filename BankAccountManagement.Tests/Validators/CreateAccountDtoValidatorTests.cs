using BankAccountManagement.API.Models;
using BankAccountManagement.API.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BankAccountManagement.Tests.Validators
{
    public class CreateAccountDtoValidatorTests
    {
        private readonly CreateAccountDtoValidator _validator;

        public CreateAccountDtoValidatorTests()
        {
            _validator = new CreateAccountDtoValidator();
        }

        [Fact]
        public void ShouldHaveError_WhenNameIsEmpty()
        {
            // Arrange
            var model = new CreateAccountDto
            {
                AccountName = ""
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AccountName);
        }

        [Fact]
        public void ShouldHaveError_WhenNameIsTooShort()
        {
            // Arrange
            var model = new CreateAccountDto
            {
                AccountName = "A"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AccountName);
        }

        [Fact]
        public void ShouldHaveError_WhenNameIsTooLong()
        {
            // Arrange
            var model = new CreateAccountDto
            {
                AccountName = new string('A', 101) // 101 characters
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AccountName);
        }

        [Fact]
        public void ShouldNotHaveError_WhenNameIsValid()
        {
            // Arrange
            var model = new CreateAccountDto
            {
                AccountName = "Valid Account Name"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}