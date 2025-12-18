namespace PlayerWalletAPI.DTOs;

public record CreateTransactionRequest(
    decimal Amount,
    /// <example>credit</example>
    string TransactionType,
    /// <example>WELCOME_BONUS_001</example>
    string Reference
    );