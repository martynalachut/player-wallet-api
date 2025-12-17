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

    [HttpPost("transactions")]
    public async Task<IActionResult> CreateTransaction(
        string playerId,
        [FromBody] CreateTransactionRequest request,
        CancellationToken token)
    {
        if (request.Amount <= 0)
        {
            return BadRequest("Amount must be greater than 0");
        }

        if (!Enum.TryParse<TransactionType>(
                request.TransactionType,
                ignoreCase: true,
                out _))
        {
            return BadRequest(
                $"Invalid transaction type. Use '{TransactionType.Credit}' or '{TransactionType.Debit}'");
        }
        
        //_walletService.CreateTransactionAsync()
        return Ok();
    }
}