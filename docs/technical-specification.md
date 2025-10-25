# Technical Specification: Bank Account Management System

## Technology Stack
- .NET 6
- ASP.NET Core Web API
- MongoDB
- MongoDB.Driver
- MongoDB.EntityFrameworkCore
- Swagger/OpenAPI
- xUnit for testing
- FluentValidation for request validation

## Project Structure

### Solution Structure
```
BankAccountManagement.sln
├── BankAccountManagement.API
├── BankAccountManagement.Core
├── BankAccountManagement.Infrastructure
└── BankAccountManagement.Tests
```

### Project Dependencies
- BankAccountManagement.API → BankAccountManagement.Core, BankAccountManagement.Infrastructure
- BankAccountManagement.Infrastructure → BankAccountManagement.Core
- BankAccountManagement.Tests → All projects

## Core Project

### Domain Models

#### Account Entity
```csharp
namespace BankAccountManagement.Core.Entities
{
    public class Account
    {
        public string Id { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
```

#### Transaction Entity
```csharp
namespace BankAccountManagement.Core.Entities
{
    public class Transaction
    {
        public string Id { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public TransactionStatus Status { get; set; }
    }

    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed
    }
}
```

### Repository Interfaces

#### Generic Repository Interface
```csharp
namespace BankAccountManagement.Core.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(string id);
    }
}
```

#### Account Repository Interface
```csharp
namespace BankAccountManagement.Core.Interfaces.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<bool> UpdateBalanceAsync(string accountId, decimal newBalance);
    }
}
```

#### Transaction Repository Interface
```csharp
namespace BankAccountManagement.Core.Interfaces.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId);
    }
}
```

### Service Interfaces

#### Account Service Interface
```csharp
namespace BankAccountManagement.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountByIdAsync(string id);
        Task<Account> CreateAccountAsync(string accountName);
        Task<bool> UpdateAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(string id);
    }
}
```

#### Transaction Service Interface
```csharp
namespace BankAccountManagement.Core.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(string accountId);
        Task<Transaction> TransferAsync(string fromAccountId, string toAccountId, decimal amount, string description);
    }
}
```

## Infrastructure Project

### MongoDB Configuration

#### MongoDbSettings
```csharp
namespace BankAccountManagement.Infrastructure.Data
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string AccountsCollectionName { get; set; }
        public string TransactionsCollectionName { get; set; }
    }
}
```

#### MongoDbContext
```csharp
namespace BankAccountManagement.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoCollection<Account> Accounts => 
            _database.GetCollection<Account>(_settings.AccountsCollectionName);

        public IMongoCollection<Transaction> Transactions => 
            _database.GetCollection<Transaction>(_settings.TransactionsCollectionName);
    }
}
```

### Repository Implementations

#### Base Repository Implementation
```csharp
namespace BankAccountManagement.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public BaseRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        // Implementation of IRepository<T> methods
    }
}
```

#### Account Repository Implementation
```csharp
namespace BankAccountManagement.Infrastructure.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(MongoDbContext context) : base(context.Accounts)
        {
        }

        public async Task<bool> UpdateBalanceAsync(string accountId, decimal newBalance)
        {
            var filter = Builders<Account>.Filter.Eq(a => a.Id, accountId);
            var update = Builders<Account>.Update
                .Set(a => a.Balance, newBalance)
                .Set(a => a.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
```

#### Transaction Repository Implementation
```csharp
namespace BankAccountManagement.Infrastructure.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(MongoDbContext context) : base(context.Transactions)
        {
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId)
        {
            var filter = Builders<Transaction>.Filter.Or(
                Builders<Transaction>.Filter.Eq(t => t.FromAccountId, accountId),
                Builders<Transaction>.Filter.Eq(t => t.ToAccountId, accountId)
            );

            return await _collection.Find(filter).ToListAsync();
        }
    }
}
```

## Service Implementations

### Account Service Implementation
```csharp
namespace BankAccountManagement.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Account> CreateAccountAsync(string accountName)
        {
            var account = new Account
            {
                AccountName = accountName,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _accountRepository.AddAsync(account);
        }

        // Other method implementations
    }
}
```

### Transaction Service Implementation
```csharp
namespace BankAccountManagement.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Transaction> TransferAsync(string fromAccountId, string toAccountId, decimal amount, string description)
        {
            // Get accounts
            var fromAccount = await _accountRepository.GetByIdAsync(fromAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(toAccountId);

            // Validate accounts exist
            if (fromAccount == null || toAccount == null)
            {
                throw new ArgumentException("One or both accounts do not exist");
            }

            // Validate sufficient balance
            if (fromAccount.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance for transfer");
            }

            // Create transaction
            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = amount,
                Description = description,
                Timestamp = DateTime.UtcNow,
                Status = TransactionStatus.Pending
            };

            // Update account balances
            await _accountRepository.UpdateBalanceAsync(fromAccountId, fromAccount.Balance - amount);
            await _accountRepository.UpdateBalanceAsync(toAccountId, toAccount.Balance + amount);

            // Update transaction status
            transaction.Status = TransactionStatus.Completed;
            
            // Save transaction
            return await _transactionRepository.AddAsync(transaction);
        }

        // Other method implementations
    }
}
```

## API Project

### DTOs (Data Transfer Objects)

#### Account DTOs
```csharp
namespace BankAccountManagement.API.Models
{
    public class AccountDto
    {
        public string Id { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
    }

    public class CreateAccountDto
    {
        public string AccountName { get; set; }
    }
}
```

#### Transaction DTOs
```csharp
namespace BankAccountManagement.API.Models
{
    public class TransactionDto
    {
        public string Id { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    public class TransferDto
    {
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
```

### Controllers

#### Accounts Controller
```csharp
namespace BankAccountManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccounts()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                AccountName = a.AccountName,
                Balance = a.Balance
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetAccount(string id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(new AccountDto
            {
                Id = account.Id,
                AccountName = account.AccountName,
                Balance = account.Balance
            });
        }

        [HttpPost]
        public async Task<ActionResult<AccountDto>> CreateAccount(CreateAccountDto createAccountDto)
        {
            var account = await _accountService.CreateAccountAsync(createAccountDto.AccountName);
            
            return CreatedAtAction(
                nameof(GetAccount),
                new { id = account.Id },
                new AccountDto
                {
                    Id = account.Id,
                    AccountName = account.AccountName,
                    Balance = account.Balance
                });
        }
    }
}
```

#### Transactions Controller
```csharp
namespace BankAccountManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("accounts/{accountId}")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsByAccountId(string accountId)
        {
            var transactions = await _transactionService.GetTransactionsByAccountIdAsync(accountId);
            return Ok(transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                FromAccountId = t.FromAccountId,
                ToAccountId = t.ToAccountId,
                Amount = t.Amount,
                Timestamp = t.Timestamp,
                Description = t.Description,
                Status = t.Status.ToString()
            }));
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<TransactionDto>> Transfer(TransferDto transferDto)
        {
            try
            {
                var transaction = await _transactionService.TransferAsync(
                    transferDto.FromAccountId,
                    transferDto.ToAccountId,
                    transferDto.Amount,
                    transferDto.Description);

                return Ok(new TransactionDto
                {
                    Id = transaction.Id,
                    FromAccountId = transaction.FromAccountId,
                    ToAccountId = transaction.ToAccountId,
                    Amount = transaction.Amount,
                    Timestamp = transaction.Timestamp,
                    Description = transaction.Description,
                    Status = transaction.Status.ToString()
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
```

### Validation

#### Transfer Validator
```csharp
namespace BankAccountManagement.API.Validators
{
    public class TransferDtoValidator : AbstractValidator<TransferDto>
    {
        public TransferDtoValidator()
        {
            RuleFor(x => x.FromAccountId)
                .NotEmpty().WithMessage("FromAccountId is required");

            RuleFor(x => x.ToAccountId)
                .NotEmpty().WithMessage("ToAccountId is required")
                .NotEqual(x => x.FromAccountId).WithMessage("Cannot transfer to the same account");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0");
        }
    }
}
```

### Program.cs Configuration
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register MongoDB context
builder.Services.AddSingleton<MongoDbContext>();

// Register repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Register services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Add controllers
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<TransferDtoValidator>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bank Account Management API",
        Version = "v1",
        Description = "An API to manage bank accounts and transactions"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Testing Strategy

### Unit Tests

#### Account Service Tests
```csharp
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
        public async Task CreateAccountAsync_ShouldCreateAccountWithZeroBalance()
        {
            // Arrange
            var accountName = "Test Account";
            var account = new Account
            {
                Id = "1",
                AccountName = accountName,
                Balance = 0
            };

            _mockAccountRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Account>()))
                .ReturnsAsync(account);

            // Act
            var result = await _accountService.CreateAccountAsync(accountName);

            // Assert
            Assert.Equal(accountName, result.AccountName);
            Assert.Equal(0, result.Balance);
        }

        // Additional tests
    }
}
```

#### Transaction Service Tests
```csharp
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
        public async Task TransferAsync_WithSufficientBalance_ShouldSucceed()
        {
            // Arrange
            var fromAccount = new Account { Id = "1", Balance = 100 };
            var toAccount = new Account { Id = "2", Balance = 50 };

            _mockAccountRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(fromAccount);

            _mockAccountRepository
                .Setup(repo => repo.GetByIdAsync("2"))
                .ReturnsAsync(toAccount);

            _mockAccountRepository
                .Setup(repo => repo.UpdateBalanceAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                .ReturnsAsync(true);

            _mockTransactionRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(new Transaction
                {
                    Id = "1",
                    FromAccountId = "1",
                    ToAccountId = "2",
                    Amount = 50,
                    Status = TransactionStatus.Completed
                });

            // Act
            var result = await _transactionService.TransferAsync("1", "2", 50, "Test transfer");

            // Assert
            Assert.Equal(TransactionStatus.Completed, result.Status);
            _mockAccountRepository.Verify(
                repo => repo.UpdateBalanceAsync("1", 50), Times.Once);
            _mockAccountRepository.Verify(
                repo => repo.UpdateBalanceAsync("2", 100), Times.Once);
        }

        [Fact]
        public async Task TransferAsync_WithInsufficientBalance_ShouldThrowException()
        {
            // Arrange
            var fromAccount = new Account { Id = "1", Balance = 30 };
            var toAccount = new Account { Id = "2", Balance = 50 };

            _mockAccountRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(fromAccount);

            _mockAccountRepository
                .Setup(repo => repo.GetByIdAsync("2"))
                .ReturnsAsync(toAccount);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _transactionService.TransferAsync("1", "2", 50, "Test transfer"));
        }

        // Additional tests
    }
}
```

## MongoDB Configuration

### appsettings.json
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "BankAccountDb",
    "AccountsCollectionName": "Accounts",
    "TransactionsCollectionName": "Transactions"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}