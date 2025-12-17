using System.Collections.Concurrent;
using PlayerWalletAPI.DTOs;
using PlayerWalletAPI.Models;
using PlayerWalletAPI.Repositories;

namespace PlayerWalletAPI.Services;

public interface IWalletService
{
    Task<IReadOnlyList<PlayerTransaction>> GetTransactionsAsync(
        string playerId,
        CancellationToken token = default);
    
    Task<decimal> GetBalanceAsync(
        string playerId,
        CancellationToken token = default);

    Task CreateTransactionAsync(
        PlayerTransactionEntity transaction,
        CancellationToken token = default);
}

public class WalletService(IWalletRepository walletRepository) : IWalletService
{
    // invalidated on every write, to be replaces with Redis/ inMemoryCache etc
    private readonly ConcurrentDictionary<string, IReadOnlyList<PlayerTransaction>> _transactionsCache = new();

    public async Task<IReadOnlyList<PlayerTransaction>> GetTransactionsAsync(
        string playerId, 
        CancellationToken token = default)
    {
        if (_transactionsCache.TryGetValue(playerId, out var cached))
        {
            return cached;
        }

        var transactions = (await walletRepository.GetPlayerTransactionsAsync(playerId, token))
            .Select(t => new PlayerTransaction(
                t.Id,
                t.PlayerId,
                t.Amount,
                t.Type,
                t.Reference,
                t.CreatedTimestamp))
            .ToList()
            .AsReadOnly();

        _transactionsCache[playerId] = transactions;
        return transactions;
    }

    public async Task<decimal> GetBalanceAsync(
        string playerId, 
        CancellationToken token = default) =>
        (await GetTransactionsAsync(playerId, token))
        .Sum(t => t.TransactionType == TransactionType.Credit ? t.Amount : -t.Amount);

    public async Task CreateTransactionAsync(
        PlayerTransactionEntity transaction, 
        CancellationToken token = default)
    {
        await walletRepository.AddAsync(transaction, token);
        _transactionsCache.TryRemove(transaction.PlayerId, out _);
    }
}