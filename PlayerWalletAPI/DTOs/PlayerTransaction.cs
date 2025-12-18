using PlayerWalletAPI.Models;

namespace PlayerWalletAPI.DTOs;

public class PlayerTransactionDto
{
    public string Id { get; init; }
    public string PlayerId { get; init; }
    public decimal Amount { get; init; }
    public TransactionType TransactionType { get; init; }
    public string Reference { get; init; }
    public DateTime CreatedTimestamp { get; init; }
}