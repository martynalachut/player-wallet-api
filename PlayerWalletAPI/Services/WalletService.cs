using System.Collections.Concurrent;
using PlayerWalletAPI.DTOs;
using PlayerWalletAPI.Models;
using PlayerWalletAPI.Repositories;

namespace PlayerWalletAPI.Services;

public interface IWalletService
{
    Task<IReadOnlyList<PlayerTransactionDto>> GetTransactionsAsync(
        string playerId,
        CancellationToken token = default);
    
    Task<decimal> GetBalanceAsync(
        string playerId,
        CancellationToken token = default);

    Task<string> CreateTransactionAsync(
        string playerId,
        decimal amount,
        TransactionType transactionType,
        string reference,
        CancellationToken token = default);
}

public class WalletService(IWalletRepository walletRepository) : IWalletService
{
    // invalidated on every write, to be replaces with Redis/ inMemoryCache etc
    private readonly ConcurrentDictionary<string, IReadOnlyList<PlayerTransactionDto>> _transactionsCache = new();

    public async Task<IReadOnlyList<PlayerTransactionDto>> GetTransactionsAsync(
        string playerId, 
        CancellationToken token = default)
    {
        var key = playerId.ToLowerInvariant();
        if (_transactionsCache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var transactions = (await walletRepository.GetPlayerTransactionsAsync(playerId, token))
            .Select(t => new PlayerTransactionDto
            {
                Id = t.Id,
                PlayerId = t.PlayerId,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                Reference = t.Reference,
                CreatedTimestamp = t.CreatedTimestamp
            })
            .ToList()
            .AsReadOnly();

        _transactionsCache[key] = transactions;
        return transactions;
    }

    public async Task<decimal> GetBalanceAsync(
        string playerId, 
        CancellationToken token = default) =>
        (await GetTransactionsAsync(playerId, token))
        .Sum(t => t.TransactionType == TransactionType.Credit ? t.Amount : -t.Amount);

    public async Task<string> CreateTransactionAsync(
        string playerId,
        decimal amount,
        TransactionType transactionType,
        string reference,
        CancellationToken token = default)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than 0");
        }

        var balance = await GetBalanceAsync(playerId, token);

        if (transactionType == TransactionType.Debit && balance < amount)
        {
            throw new InvalidOperationException("Insufficient balance");
        }

        var entity = new PlayerTransactionEntity
        {
            Id = Guid.NewGuid().ToString(),
            PlayerId = playerId,
            Amount = amount,
            TransactionType = transactionType,
            Reference = reference,
            CreatedTimestamp = DateTime.UtcNow
        };

        var result = await walletRepository.AddAsync(entity, token);
        _transactionsCache.TryRemove(playerId, out _);

        return result;
    }
}