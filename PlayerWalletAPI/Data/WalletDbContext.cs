using Microsoft.EntityFrameworkCore;
using PlayerWalletAPI.Models;

namespace PlayerWalletAPI.Data;

public class WalletDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<PlayerTransactionEntity>  WalletTransactions { get; set; }
}