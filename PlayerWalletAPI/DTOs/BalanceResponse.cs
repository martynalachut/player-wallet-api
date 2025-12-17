namespace PlayerWalletAPI.DTOs;

public class BalanceResponse
{
    public string PlayerId { get; init; }
    public decimal Amount { get; init; }
}