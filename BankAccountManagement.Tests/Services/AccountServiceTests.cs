using BankAccountManagement.Core.Entities;
using BankAccountManagement.Core.Interfaces.Repositories;
using BankAccountManagement.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BankAccountManagement.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _accountService = new AccountService(_mockAccountRepository.Object);
        }

        [Fact]
        public async Task GetAllAccountsAsync_ShouldReturnAllAccounts()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new Account { Id = "1", AccountName = "Account 1", Balance = 100 },
                new Account { Id = "2", AccountName = "Account 2", Balance = 200 }
            };

            _mockAccountRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(accounts);

            // Act
            var result = await _accountService.GetAllAccountsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.Id == "1");
            Assert.Contains(result, a => a.Id == "2");
            _mockAccountRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAccountByIdAsync_WithValidId_ShouldReturnAccount()
        {
            // Arrange
            var accountId = "1";
            var account = new Account { Id = accountId, AccountName = "Test Account", Balance = 100 };

            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync(account);

            // Act
            var result = await _accountService.GetAccountByIdAsync(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.Id);
            Assert.Equal("Test Account", result.AccountName);
            Assert.Equal(100, result.Balance);
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(accountId), Times.Once);
        }

        [Fact]
        public async Task GetAccountByIdAsync_WithNullId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountService.GetAccountByIdAsync(null));
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetAccountByIdAsync_WithEmptyId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountService.GetAccountByIdAsync(string.Empty));
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateAccountAsync_WithValidName_ShouldCreateAccount()
        {
            // Arrange
            var accountName = "New Account";
            var createdAccount = new Account { Id = "new-id", AccountName = accountName, Balance = 0 };

            _mockAccountRepository.Setup(repo => repo.AddAsync(It.IsAny<Account>()))
                .ReturnsAsync((Account account) => account);

            // Act
            var result = await _accountService.CreateAccountAsync(accountName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountName, result.AccountName);
            Assert.Equal(0, result.Balance);
            Assert.NotNull(result.Id);
            _mockAccountRepository.Verify(repo => repo.AddAsync(It.IsAny<Account>()), Times.Once);
        }

        [Fact]
        public async Task CreateAccountAsync_WithNullName_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountService.CreateAccountAsync(null));
            _mockAccountRepository.Verify(repo => repo.AddAsync(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public async Task CreateAccountAsync_WithEmptyName_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountService.CreateAccountAsync(string.Empty));
            _mockAccountRepository.Verify(repo => repo.AddAsync(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAccountAsync_WithValidAccount_ShouldUpdateAccount()
        {
            // Arrange
            var account = new Account { Id = "1", AccountName = "Updated Account", Balance = 150 };

            _mockAccountRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Account>()))
                .ReturnsAsync(true);

            // Act
            var result = await _accountService.UpdateAccountAsync(account);

            // Assert
            Assert.True(result);
            _mockAccountRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Account>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAccountAsync_WithNullAccount_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _accountService.UpdateAccountAsync(null));
            _mockAccountRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAccountAsync_WithEmptyId_ShouldThrowArgumentException()
        {
            // Arrange
            var account = new Account { Id = "", AccountName = "Invalid Account", Balance = 100 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountService.UpdateAccountAsync(account));
            _mockAccountRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAccountAsync_WithValidId_ShouldDeleteAccount()
        {
            // Arrange
            var accountId = "1";

            _mockAccountRepository.Setup(repo => repo.DeleteAsync(accountId))
                .ReturnsAsync(true);

            // Act
            var result = await _accountService.DeleteAccountAsync(accountId);

            // Assert
            Assert.True(result);
            _mockAccountRepository.Verify(repo => repo.DeleteAsync(accountId), Times.Once);
        }

        [Fact]
        public async Task DeleteAccountAsync_WithNullId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountService.DeleteAccountAsync(null));
            _mockAccountRepository.Verify(repo => repo.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAccountAsync_WithEmptyId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountService.DeleteAccountAsync(string.Empty));
            _mockAccountRepository.Verify(repo => repo.DeleteAsync(It.IsAny<string>()), Times.Never);
        }
    }
}