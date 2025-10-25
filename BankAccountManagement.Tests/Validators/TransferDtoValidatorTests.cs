using BankAccountManagement.API.Models;
using BankAccountManagement.API.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BankAccountManagement.Tests.Validators
{
    public class TransferDtoValidatorTests
    {
        private readonly TransferDtoValidator _validator;

        public TransferDtoValidatorTests()
        {
            _validator = new TransferDtoValidator();
        }

        [Fact]
        public void ShouldHaveError_WhenFromAccountIdIsEmpty()
        {
            // Arrange
            var model = new TransferDto
            {
                FromAccountId = "",
                ToAccountId = "2",
                Amount = 100,
                Description = "Test"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FromAccountId);
        }

        [Fact]
        public void ShouldHaveError_WhenToAccountIdIsEmpty()
        {
            // Arrange
            var model = new TransferDto
            {
                FromAccountId = "1",
                ToAccountId = "",
                Amount = 100,
                Description = "Test"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ToAccountId);
        }

        [Fact]
        public void ShouldHaveError_WhenFromAndToAccountIdsAreSame()
        {
            // Arrange
            var model = new TransferDto
            {
                FromAccountId = "1",
                ToAccountId = "1",
                Amount = 100,
                Description = "Test"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ToAccountId);
        }

        [Fact]
        public void ShouldHaveError_WhenAmountIsZero()
        {
            // Arrange
            var model = new TransferDto
            {
                FromAccountId = "1",
                ToAccountId = "2",
                Amount = 0,
                Description = "Test"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Amount);
        }

        [Fact]
        public void ShouldHaveError_WhenAmountIsNegative()
        {
            // Arrange
            var model = new TransferDto
            {
                FromAccountId = "1",
                ToAccountId = "2",
                Amount = -100,
                Description = "Test"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Amount);
        }

        [Fact]
        public void ShouldNotHaveError_WhenModelIsValid()
        {
            // Arrange
            var model = new TransferDto
            {
                FromAccountId = "1",
                ToAccountId = "2",
                Amount = 100,
                Description = "Test"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}