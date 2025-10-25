using BankAccountManagement.API.Models;
using BankAccountManagement.API.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BankAccountManagement.Tests.Validators
{
    public class WithdrawDtoValidatorTests
    {
        private readonly WithdrawDtoValidator _validator;

        public WithdrawDtoValidatorTests()
        {
            _validator = new WithdrawDtoValidator();
        }

        [Fact]
        public void Should_Pass_When_Amount_Is_Greater_Than_Zero()
        {
            // Arrange
            var model = new WithdrawDto
            {
                Amount = 100,
                Description = "Test withdrawal"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_Amount_Is_Zero()
        {
            // Arrange
            var model = new WithdrawDto
            {
                Amount = 0,
                Description = "Test withdrawal"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Amount must be greater than zero");
        }

        [Fact]
        public void Should_Fail_When_Amount_Is_Negative()
        {
            // Arrange
            var model = new WithdrawDto
            {
                Amount = -100,
                Description = "Test withdrawal"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Amount must be greater than zero");
        }

        [Fact]
        public void Should_Fail_When_Description_Exceeds_Maximum_Length()
        {
            // Arrange
            var model = new WithdrawDto
            {
                Amount = 100,
                Description = new string('A', 501) // 501 characters
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description cannot exceed 500 characters");
        }
    }
}