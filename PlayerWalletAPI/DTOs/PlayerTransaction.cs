using PlayerWalletAPI.Models;

namespace PlayerWalletAPI.DTOs;

public record PlayerTransaction(
    string Id,
    string PlayerId,
    decimal Amount,
    TransactionType TransactionType,
    string Reference,
    DateTime CreatedTimestamp);