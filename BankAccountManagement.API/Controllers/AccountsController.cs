using BankAccountManagement.API.Models;
using BankAccountManagement.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAccountManagement.API.Controllers
{
    /// <summary>
    /// API controller for account operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Initializes a new instance of the AccountsController class
        /// </summary>
        /// <param name="accountService">Account service</param>
        /// <param name="transactionService">Transaction service</param>
        public AccountsController(IAccountService accountService, ITransactionService transactionService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        /// <summary>
        /// Gets all accounts
        /// </summary>
        /// <returns>Collection of accounts</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        /// <summary>
        /// Gets an account by ID
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <returns>Account details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Creates a new account
        /// </summary>
        /// <param name="createAccountDto">Account creation data</param>
        /// <returns>Created account</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountDto>> CreateAccount(CreateAccountDto createAccountDto)
        {
            try
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets all transactions for an account
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <returns>Collection of transactions</returns>
        [HttpGet("{accountId}/transactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAccountTransactions(string accountId)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return NotFound();
            }

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
    }
}