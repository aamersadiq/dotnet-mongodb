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
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _transactionService = new TransactionService(
                _mockTransactionRepository.Object,
                _mockAccountRepository.Object);
        }

        [Fact]
        public async Task GetTransactionsByAccountIdAsync_WithValidId_ShouldReturnTransactions()
        {
            // Arrange
            var accountId = "1";
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = "t1",
                    FromAccountId = accountId,
                    ToAccountId = "2",
                    Amount = 100,
                    Description = "Test transaction 1",
                    Timestamp = DateTime.UtcNow.AddDays(-1),
                    Status = TransactionStatus.Completed
                },
                new Transaction
                {
                    Id = "t2",
                    FromAccountId = "3",
                    ToAccountId = accountId,
                    Amount = 200,
                    Description = "Test transaction 2",
                    Timestamp = DateTime.UtcNow,
                    Status = TransactionStatus.Completed
                }
            };

            _mockTransactionRepository.Setup(repo => repo.GetByAccountIdAsync(accountId))
                .ReturnsAsync(transactions);

            // Act
            var result = await _transactionService.GetTransactionsByAccountIdAsync(accountId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Id == "t1");
            Assert.Contains(result, t => t.Id == "t2");
            _mockTransactionRepository.Verify(repo => repo.GetByAccountIdAsync(accountId), Times.Once);
        }

        [Fact]
        public async Task GetTransactionsByAccountIdAsync_WithNullId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _transactionService.GetTransactionsByAccountIdAsync(null));
            _mockTransactionRepository.Verify(repo => repo.GetByAccountIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetTransactionsByAccountIdAsync_WithEmptyId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _transactionService.GetTransactionsByAccountIdAsync(string.Empty));
            _mockTransactionRepository.Verify(repo => repo.GetByAccountIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TransferAsync_WithValidParameters_ShouldCreateTransaction()
        {
            // Arrange
            var fromAccountId = "1";
            var toAccountId = "2";
            var amount = 100m;
            var description = "Test transfer";

            var fromAccount = new Account { Id = fromAccountId, AccountName = "Source Account", Balance = 500 };
            var toAccount = new Account { Id = toAccountId, AccountName = "Destination Account", Balance = 200 };

            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(fromAccountId))
                .ReturnsAsync(fromAccount);
            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(toAccountId))
                .ReturnsAsync(toAccount);
            _mockAccountRepository.Setup(repo => repo.UpdateBalanceAsync(fromAccountId, It.IsAny<decimal>()))
                .ReturnsAsync(true);
            _mockAccountRepository.Setup(repo => repo.UpdateBalanceAsync(toAccountId, It.IsAny<decimal>()))
                .ReturnsAsync(true);
            _mockTransactionRepository.Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync((Transaction t) => t);

            // Act
            var result = await _transactionService.TransferAsync(fromAccountId, toAccountId, amount, description);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fromAccountId, result.FromAccountId);
            Assert.Equal(toAccountId, result.ToAccountId);
            Assert.Equal(amount, result.Amount);
            Assert.Equal(description, result.Description);
            Assert.Equal(TransactionStatus.Completed, result.Status);
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(fromAccountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(toAccountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(fromAccountId, 400), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(toAccountId, 300), Times.Once);
            _mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task TransferAsync_WithInsufficientBalance_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var fromAccountId = "1";
            var toAccountId = "2";
            var amount = 1000m;
            var description = "Test transfer";

            var fromAccount = new Account { Id = fromAccountId, AccountName = "Source Account", Balance = 500 };
            var toAccount = new Account { Id = toAccountId, AccountName = "Destination Account", Balance = 200 };

            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(fromAccountId))
                .ReturnsAsync(fromAccount);
            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(toAccountId))
                .ReturnsAsync(toAccount);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _transactionService.TransferAsync(fromAccountId, toAccountId, amount, description));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(fromAccountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(toAccountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
            _mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task TransferAsync_WithNullFromAccountId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _transactionService.TransferAsync(null, "2", 100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TransferAsync_WithNullToAccountId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _transactionService.TransferAsync("1", null, 100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TransferAsync_WithSameAccountIds_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _transactionService.TransferAsync("1", "1", 100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TransferAsync_WithZeroAmount_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _transactionService.TransferAsync("1", "2", 0, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TransferAsync_WithNegativeAmount_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _transactionService.TransferAsync("1", "2", -100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TransferAsync_WithNonExistentFromAccount_ShouldThrowArgumentException()
        {
            // Arrange
            var fromAccountId = "1";
            var toAccountId = "2";
            
            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(fromAccountId))
                .ReturnsAsync((Account)null);
            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(toAccountId))
                .ReturnsAsync(new Account { Id = toAccountId, AccountName = "Destination Account", Balance = 200 });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _transactionService.TransferAsync(fromAccountId, toAccountId, 100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(fromAccountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        }

        [Fact]
        public async Task TransferAsync_WithNonExistentToAccount_ShouldThrowArgumentException()
        {
            // Arrange
            var fromAccountId = "1";
            var toAccountId = "2";
            
            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(fromAccountId))
                .ReturnsAsync(new Account { Id = fromAccountId, AccountName = "Source Account", Balance = 500 });
            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(toAccountId))
                .ReturnsAsync((Account)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _transactionService.TransferAsync(fromAccountId, toAccountId, 100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(fromAccountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(toAccountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        }

        [Fact]
        public async Task DepositAsync_WithValidParameters_ShouldCreateTransaction()
        {
            // Arrange
            var accountId = "1";
            var amount = 100m;
            var description = "Test deposit";

            var account = new Account { Id = accountId, AccountName = "Test Account", Balance = 500 };

            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync(account);
            _mockAccountRepository.Setup(repo => repo.UpdateBalanceAsync(accountId, It.IsAny<decimal>()))
                .ReturnsAsync(true);
            _mockTransactionRepository.Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync((Transaction t) => t);

            // Act
            var result = await _transactionService.DepositAsync(accountId, amount, description);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.FromAccountId);
            Assert.Equal(accountId, result.ToAccountId);
            Assert.Equal(amount, result.Amount);
            Assert.Equal(description, result.Description);
            Assert.Equal(TransactionStatus.Completed, result.Status);
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(accountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(accountId, 600), Times.Once);
            _mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task DepositAsync_WithNullAccountId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transactionService.DepositAsync(null, 100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DepositAsync_WithZeroAmount_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transactionService.DepositAsync("1", 0, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DepositAsync_WithNegativeAmount_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transactionService.DepositAsync("1", -100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DepositAsync_WithNonExistentAccount_ShouldThrowArgumentException()
        {
            // Arrange
            var accountId = "1";
            
            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync((Account)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transactionService.DepositAsync(accountId, 100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(accountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        }

        [Fact]
        public async Task WithdrawAsync_WithValidParameters_ShouldCreateTransaction()
        {
            // Arrange
            var accountId = "1";
            var amount = 100m;
            var description = "Test withdrawal";

            var account = new Account { Id = accountId, AccountName = "Test Account", Balance = 500 };

            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync(account);
            _mockAccountRepository.Setup(repo => repo.UpdateBalanceAsync(accountId, It.IsAny<decimal>()))
                .ReturnsAsync(true);
            _mockTransactionRepository.Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync((Transaction t) => t);

            // Act
            var result = await _transactionService.WithdrawAsync(accountId, amount, description);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.FromAccountId);
            Assert.Null(result.ToAccountId);
            Assert.Equal(amount, result.Amount);
            Assert.Equal(description, result.Description);
            Assert.Equal(TransactionStatus.Completed, result.Status);
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(accountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(accountId, 400), Times.Once);
            _mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task WithdrawAsync_WithInsufficientBalance_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var accountId = "1";
            var amount = 1000m;
            var description = "Test withdrawal";

            var account = new Account { Id = accountId, AccountName = "Test Account", Balance = 500 };

            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _transactionService.WithdrawAsync(accountId, amount, description));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(accountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
            _mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task WithdrawAsync_WithNullAccountId_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transactionService.WithdrawAsync(null, 100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task WithdrawAsync_WithZeroAmount_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transactionService.WithdrawAsync("1", 0, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task WithdrawAsync_WithNegativeAmount_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transactionService.WithdrawAsync("1", -100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task WithdrawAsync_WithNonExistentAccount_ShouldThrowArgumentException()
        {
            // Arrange
            var accountId = "1";
            
            _mockAccountRepository.Setup(repo => repo.GetByIdAsync(accountId))
                .ReturnsAsync((Account)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _transactionService.WithdrawAsync(accountId, 100, "Test"));
            
            _mockAccountRepository.Verify(repo => repo.GetByIdAsync(accountId), Times.Once);
            _mockAccountRepository.Verify(repo => repo.UpdateBalanceAsync(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        }
    }
}