namespace PlayerWalletAPI.DTOs;

public class CreateTransactionRequest
{
    public decimal Amount { get; init; }
    public string TransactionType { get; init; }
    public string Reference { get; init; }
}