using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using PlayerWalletAPI.Controllers;
using PlayerWalletAPI.DTOs;
using PlayerWalletAPI.Models;
using PlayerWalletAPI.Services;

namespace PlayerWalletAPI.Tests
{
    public class WalletControllerTests
    {
        private readonly Mock<IWalletService> _serviceMock;
        private readonly WalletController _controller;

        public WalletControllerTests()
        {
            _serviceMock = new Mock<IWalletService>();
            _controller = new WalletController(_serviceMock.Object);
        }

        [Fact]
        public async Task CreateTransaction_ValidRequest_ReturnsSuccess()
        {
            var request = new CreateTransactionRequest(
                25.43m,
                "credit",
                "REF");

            _serviceMock
                .Setup(s => s.CreateTransactionAsync(
                    "0005",
                    25.43m,
                    TransactionType.Credit,
                    "REF",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid().ToString());

            var result = await _controller.CreateTransaction(
                "0005",
                request,
                CancellationToken.None);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(actionResult);
            Assert.Equal(actionResult.StatusCode, 200);
            Assert.IsType<string>(actionResult.Value);
        }

        [Fact]
        public async Task CreateTransaction_AmountLessOrEqualZero_ReturnsBadRequest()
        {
            var request = new CreateTransactionRequest(
                0,
                "credit",
                "REF");

            var result = await _controller.CreateTransaction(
                "0003",
                request,
                CancellationToken.None);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task CreateTransaction_InvalidTransactionType_ReturnsBadRequest()
        {
            var request = new CreateTransactionRequest(
                43.67m,
                "reversal",
                "REF");

            var result = await _controller.CreateTransaction(
                "0007",
                request,
                CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateTransaction_InsufficientBalance_ReturnsConflict()
        {
            var request = new CreateTransactionRequest(
                50,
                "debit",
                "REF");

            _serviceMock
                .Setup(s => s.CreateTransactionAsync(
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    TransactionType.Debit,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Insufficient balance"));

            var result = await _controller.CreateTransaction(
                "0004",
                request,
                CancellationToken.None);

            var objResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal(422, objResult.StatusCode);
        }

        [Fact]
        public async Task GetBalance_ReturnsOkWithBalance()
        {
            _serviceMock
                .Setup(s => s.GetBalanceAsync("0009", It.IsAny<CancellationToken>()))
                .ReturnsAsync(150m);

            var result = await _controller.GetBalance("0009", CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok);
            var balanceProp = ok.Value.GetType().GetProperty("Balance");
            var playerIdProp = ok.Value.GetType().GetProperty("PlayerId");

            Assert.NotNull(balanceProp);
            Assert.NotNull(playerIdProp);
            Assert.Equal(150m, balanceProp.GetValue(ok.Value));
            Assert.Equal("0009", playerIdProp.GetValue(ok.Value));
        }

        [Fact]
        public async Task GetTransactions_ReturnsTransactions()
        {
            _serviceMock
                .Setup(s => s.GetTransactionsAsync("0006", It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await _controller.GetTransactions("0006", CancellationToken.None);

            Assert.IsType<OkObjectResult>(result);
        }

    }
}