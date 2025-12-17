namespace PlayerWalletAPI.Models;

public record PlayerTransactionEntity
{
    public string Id { get; set; }
    public string PlayerId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Reference { get; set; }
    public DateTime CreatedTimestamp { get; set; }
}

public enum TransactionType
{
    Credit,
    Debit
}