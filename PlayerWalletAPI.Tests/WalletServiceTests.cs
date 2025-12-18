using Moq;
using PlayerWalletAPI.Models;
using PlayerWalletAPI.Repositories;
using PlayerWalletAPI.Services;

namespace PlayerWalletAPI.Tests;

public class WalletServiceTests
{
    private readonly Mock<IWalletRepository> _repoMock;
    private readonly WalletService _service;

    public WalletServiceTests()
    {
        _repoMock = new Mock<IWalletRepository>();
        _service = new WalletService(_repoMock.Object);
    }

    [Fact]
    public async Task CreateTransactionAsync_Credit_AddsTransaction()
    {
        var playerId = "0001";
        var txnId = "8350e554-6a29-409c-8c42-08a44b8afdfb";

        _repoMock
        .Setup(r => r.GetPlayerTransactionsAsync(playerId, It.IsAny<CancellationToken>()))
        .ReturnsAsync([]);

        _repoMock
        .Setup(r => r.AddAsync(It.IsAny<PlayerTransactionEntity>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(txnId);

        var result = await _service.CreateTransactionAsync(
            playerId,
            12.67m,
            TransactionType.Credit,
            "TEST_REF");

        Assert.Equal(txnId, result);

        _repoMock.Verify(
            r => r.AddAsync(
                It.Is<PlayerTransactionEntity>(t =>
                    t.PlayerId == playerId &&
                    t.Amount == 12.67m &&
                    t.TransactionType == TransactionType.Credit),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetBalanceAsync_ReturnsCorrectBalance()
    {
        var playerId = "0001";

        var transactions = new List<PlayerTransactionEntity>
        {
            new() { Amount = 82.70m, TransactionType = TransactionType.Credit },
            new() { Amount = 30.46m, TransactionType = TransactionType.Debit }
        };

        _repoMock
            .Setup(r => r.GetPlayerTransactionsAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        var balance = await _service.GetBalanceAsync(playerId);

        Assert.Equal(52.24m, balance);
    }

    [Fact]
    public async Task GetBalanceAsync_NoTransactions_ReturnsZero()
    {
        var playerId = "0003";

        _repoMock
            .Setup(r => r.GetPlayerTransactionsAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var balance = await _service.GetBalanceAsync(playerId);

        Assert.Equal(0m, balance);
    }

    [Fact]
    public async Task GetTransactionsAsync_UsesCache()
    {
        var playerId = "0002";

        _repoMock
            .Setup(r => r.GetPlayerTransactionsAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _service.GetTransactionsAsync(playerId);

        _repoMock.Verify(
            r => r.GetPlayerTransactionsAsync(playerId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateTransactionAsync_InvalidatesCache()
    {
        var playerId = "0003";

        _repoMock
            .Setup(r => r.GetPlayerTransactionsAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _service.GetTransactionsAsync(playerId);

        await _service.CreateTransactionAsync(
            playerId,
            10,
            TransactionType.Credit,
            "REF");

        await _service.GetTransactionsAsync(playerId);

        _repoMock.Verify(
            r => r.GetPlayerTransactionsAsync(playerId, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}