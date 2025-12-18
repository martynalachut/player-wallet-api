using Microsoft.EntityFrameworkCore;
using PlayerWalletAPI.Data;
using PlayerWalletAPI.Models;

namespace PlayerWalletAPI.Repositories;

public interface IWalletRepository
{
    Task<List<PlayerTransactionEntity>> GetAllAsync(CancellationToken token = default);
    Task<List<PlayerTransactionEntity>> GetPlayerTransactionsAsync(
        string playerId,
        CancellationToken token = default);
    Task<string> AddAsync(PlayerTransactionEntity playerTransaction, CancellationToken token = default);
    Task UpdateAsync(PlayerTransactionEntity playerTransaction, CancellationToken token = default);
}

public class WalletRepository(WalletDbContext walletDbContext) : IWalletRepository
{
    public async Task<List<PlayerTransactionEntity>> GetAllAsync(
        CancellationToken token = default)
    {
        return await walletDbContext.WalletTransactions
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedTimestamp)
            .ToListAsync(token);
    }

    public async Task<List<PlayerTransactionEntity>> GetPlayerTransactionsAsync(
        string playerId,
        CancellationToken token = default)
    {
        return await walletDbContext.WalletTransactions
            .AsNoTracking()
            .Where(t => t.PlayerId == playerId)
            .OrderByDescending(t => t.CreatedTimestamp)
            .ToListAsync(token);
    }

    public async Task<string> AddAsync(
        PlayerTransactionEntity playerTransaction,
        CancellationToken token = default)
    {
        walletDbContext.WalletTransactions.Add(playerTransaction);
        await walletDbContext.SaveChangesAsync(token);
        return playerTransaction.Id;
    }

    public async Task UpdateAsync(
        PlayerTransactionEntity playerTransaction,
        CancellationToken token = default)
    {
        walletDbContext.WalletTransactions.Update(playerTransaction);
        await walletDbContext.SaveChangesAsync(token);
    }
}