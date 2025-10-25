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
    /// API controller for transaction operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Initializes a new instance of the TransactionsController class
        /// </summary>
        /// <param name="transactionService">Transaction service</param>
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        /// <summary>
        /// Transfers money between accounts
        /// </summary>
        /// <param name="transferDto">Transfer details</param>
        /// <returns>Created transaction</returns>
        [HttpPost("transfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Gets all transactions for a specific account
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <returns>Collection of transactions involving the specified account</returns>
        [HttpGet("account/{accountId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByAccountId(string accountId)
        {
            try
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
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deposits money into an account
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <param name="depositDto">Deposit details</param>
        /// <returns>Created transaction</returns>
        [HttpPost("account/{accountId}/deposit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDto>> Deposit(string accountId, DepositDto depositDto)
        {
            try
            {
                var transaction = await _transactionService.DepositAsync(
                    accountId,
                    depositDto.Amount,
                    depositDto.Description);

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
        }

        /// <summary>
        /// Withdraws money from an account
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <param name="withdrawDto">Withdrawal details</param>
        /// <returns>Created transaction</returns>
        [HttpPost("account/{accountId}/withdraw")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDto>> Withdraw(string accountId, WithdrawDto withdrawDto)
        {
            try
            {
                var transaction = await _transactionService.WithdrawAsync(
                    accountId,
                    withdrawDto.Amount,
                    withdrawDto.Description);

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