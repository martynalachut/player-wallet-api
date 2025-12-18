using Microsoft.EntityFrameworkCore;
using PlayerWalletAPI.Data;
using PlayerWalletAPI.Models;
using PlayerWalletAPI.Repositories;

namespace PlayerWalletAPI.Tests
{
    public class WalletRepositoryTests
    {
        private static WalletRepository CreateRepository(out WalletDbContext context)
        {
            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new WalletDbContext(options);
            return new WalletRepository(context);
        }

        [Fact]
        public async Task AddAsync_PersistsTransaction()
        {
            var repo = CreateRepository(out var context);

            var entity = new PlayerTransactionEntity
            {
                Id = Guid.NewGuid().ToString(),
                PlayerId = "0001",
                Amount = 100,
                TransactionType = TransactionType.Credit,
                Reference = "REF",
                CreatedTimestamp = DateTime.UtcNow
            };

            var result = await repo.AddAsync(entity);

            Assert.Equal(entity.Id, result);
            Assert.Single(context.WalletTransactions);
        }

        [Fact]
        public async Task GetPlayerTransactionsAsync_ReturnsOnlyPlayerTransactions()
        {
            var repo = CreateRepository(out var context);

            context.WalletTransactions.AddRange(
                new PlayerTransactionEntity
                {
                    Id = "1",
                    PlayerId = "0001",
                    Amount = 10.67m,
                    TransactionType = TransactionType.Credit,
                    Reference = "test_ref",
                    CreatedTimestamp = DateTime.UtcNow
                },
                new PlayerTransactionEntity
                {
                    Id = "2",
                    PlayerId = "0002",
                    Amount = 56.90m,
                    TransactionType = TransactionType.Credit,
                    Reference = "test_ref",
                    CreatedTimestamp = DateTime.UtcNow
                });

            await context.SaveChangesAsync();

            var result = await repo.GetPlayerTransactionsAsync("0001");

            Assert.Single(result);
            Assert.Equal("0001", result[0].PlayerId);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTransaction()
        {
            var repo = CreateRepository(out var context);

            var entity = new PlayerTransactionEntity
            {
                Id = "1",
                PlayerId = "0001",
                Amount = 10,
                Reference = "test_ref",
                TransactionType = TransactionType.Credit,
                CreatedTimestamp = DateTime.UtcNow
            };

            context.WalletTransactions.Add(entity);
            await context.SaveChangesAsync();

            entity.Amount = 56.67m;
            await repo.UpdateAsync(entity);

            var updated = await context.WalletTransactions.FindAsync("1");

            Assert.NotNull(updated);
            Assert.Equal(56.67m, updated.Amount);
        }
    }
}
