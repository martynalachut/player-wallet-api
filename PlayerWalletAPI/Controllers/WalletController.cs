using Microsoft.AspNetCore.Mvc;
using PlayerWalletAPI.DTOs;
using PlayerWalletAPI.Models;
using PlayerWalletAPI.Services;

namespace PlayerWalletAPI.Controllers;

[ApiController]
[Route("api/players/{playerId}")]
public class WalletController(IWalletService walletService) : ControllerBase
{
    private readonly IWalletService _walletService = walletService;

    /// <summary>
    /// Creates a wallet transaction for a player
    /// </summary>
    /// <param name="playerId">Unique player identifier</param>
    /// <param name="request">Transaction details</param>
    /// <response code="200">Transaction created successfully</response>
    /// <response code="400">Invalid input</response>
    /// <response code="409">Insufficient balance</response>
    [HttpPost("transactions")]
    public async Task<IActionResult> CreateTransaction(
        string playerId,
        CreateTransactionRequest request,
        CancellationToken token)
    {
        if (request.Amount <= 0)
        {
            return BadRequest("Amount must be greater than 0");
        }

        if (!Enum.TryParse<TransactionType>(
                request.TransactionType,
                ignoreCase: true,
                out var transactionType))
        {
            return BadRequest($"Invalid transaction type. Use '{TransactionType.Credit}' or '{TransactionType.Debit}'");
        }

        // use exception filter
        try
        {
            var result = await _walletService.CreateTransactionAsync(
                playerId,
                request.Amount,
                transactionType,
                request.Reference,
                token);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves wallet transaction for a player
    /// </summary>
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        string playerId,
        CancellationToken token)
    {
        var transactions = await _walletService.GetTransactionsAsync(playerId, token);

        return Ok(transactions);
    }

    /// <summary>
    /// Retrieves a player's balance
    /// </summary>
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance(
        string playerId,
        CancellationToken token)
    {
        var balance =
            await _walletService.GetBalanceAsync(playerId, token);

        return Ok(new
        {
            PlayerId = playerId,
            Balance = balance
        });
    }
}