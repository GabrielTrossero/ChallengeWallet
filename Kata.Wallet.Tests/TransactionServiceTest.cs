using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kata.Wallet.Database.Repository;
using Kata.Wallet.Services;
using Moq;
using Xunit;
using System.Resources;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Kata.Wallet.Database;
using Microsoft.Extensions.Configuration;

namespace Kata.Wallet.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IWalletService> _mockWalletService;
        private readonly Mock<ResourceManager> _mockResourceManager;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockWalletService = new Mock<IWalletService>();
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockResourceManager = new Mock<ResourceManager>();

            _transactionService = new TransactionService(
                _mockTransactionRepository.Object,
                _mockWalletService.Object,
                _mockResourceManager.Object
            );
        }

        [Fact]
        public async Task Create_ShouldSucceed_WhenWalletsExistAndSameCurrency()
        {
            // Arrange
            var walletOrigin = new Domain.Wallet { Id = 1, Balance = 500, Currency = Domain.Currency.USD };
            var walletDestination = new Domain.Wallet { Id = 2, Balance = 100, Currency = Domain.Currency.USD };
            var transaction = new Domain.Transaction { Amount = 200 };

            // Config dependencies
            _mockWalletService.Setup(w => w.GetById(1)).ReturnsAsync(walletOrigin);
            _mockWalletService.Setup(w => w.GetById(2)).ReturnsAsync(walletDestination);
            _mockTransactionRepository.Setup(t => t.IsInMemoryDatabase()).Returns(true);

            // Act
            var result = await _transactionService.Create(transaction, 1, 2);

            // Assert
            Assert.Equal(string.Empty, result);
            Assert.Equal(300, walletOrigin.Balance);
            Assert.Equal(300, walletDestination.Balance);
        }

        [Fact]
        public async Task Create_ShouldReturnError_WhenWalletNotFound()
        {
            // Arrange
            _mockWalletService.Setup(w => w.GetById(1)).ReturnsAsync((Domain.Wallet?)null);
            _mockResourceManager.Setup(r => r.GetString("WalletNotFound")).Returns("Wallet not found");

            var transaction = new Domain.Transaction { Amount = 100 };

            // Act
            var result = await _transactionService.Create(transaction, 1, 2);

            // Assert
            Assert.Equal("Wallet not found", result);
        }

        [Fact]
        public async Task Create_ShouldReturnError_WhenWalletsHaveDifferentCurrencies()
        {
            // Arrange
            var walletOrigin = new Domain.Wallet { Id = 1, Balance = 500, Currency = Domain.Currency.USD };
            var walletDestination = new Domain.Wallet { Id = 2, Balance = 100, Currency = Domain.Currency.EUR };

            _mockWalletService.Setup(w => w.GetById(1)).ReturnsAsync(walletOrigin);
            _mockWalletService.Setup(w => w.GetById(2)).ReturnsAsync(walletDestination);
            _mockResourceManager.Setup(r => r.GetString("DifferentCurrencies")).Returns("Wallets have different currencies");

            var transaction = new Domain.Transaction { Amount = 100 };

            // Act
            var result = await _transactionService.Create(transaction, 1, 2);

            // Assert
            Assert.Equal("Wallets have different currencies", result);
        }

        [Fact]
        public async Task Create_ShouldReturnError_WhenInsufficientFunds()
        {
            // Arrange
            var walletOrigin = new Domain.Wallet { Id = 1, Balance = 50, Currency = Domain.Currency.USD };
            var walletDestination = new Domain.Wallet { Id = 2, Balance = 100, Currency = Domain.Currency.USD };

            _mockWalletService.Setup(w => w.GetById(1)).ReturnsAsync(walletOrigin);
            _mockWalletService.Setup(w => w.GetById(2)).ReturnsAsync(walletDestination);
            _mockResourceManager.Setup(r => r.GetString("InsufficientFunds")).Returns("Insufficient funds");

            var transaction = new Domain.Transaction { Amount = 100 };

            // Act
            var result = await _transactionService.Create(transaction, 1, 2);

            // Assert
            Assert.Equal("Insufficient funds", result);
        }

        [Fact]
        public async Task GetTransactions_ShouldReturnTransactionsWithCorrectAmountSigns()
        {
            // Arrange
            var walletId = 1;
            var transactions = new List<Domain.Transaction>
            {
                new Domain.Transaction { Amount = 100, WalletOutgoing = new Domain.Wallet { Id = walletId } },
                new Domain.Transaction { Amount = 50, WalletIncoming = new Domain.Wallet { Id = walletId } }
            };

            _mockTransactionRepository.Setup(t => t.GetTransactions(walletId)).ReturnsAsync(transactions);

            // Act
            var result = await _transactionService.GetTransactions(walletId);

            // Assert
            Assert.Equal(-100, result[0].Amount);
            Assert.Equal(50, result[1].Amount);
        }

        [Fact]
        public async Task GetLastTransaction_ShouldReturnCorrectTransaction()
        {
            // Arrange
            var expectedTransaction = new Domain.Transaction { Amount = 200 };
            _mockTransactionRepository.Setup(t => t.GetLastTransaction(1, 2)).ReturnsAsync(expectedTransaction);

            // Act
            var result = await _transactionService.GetLastTransaction(1, 2);

            // Assert
            Assert.Equal(expectedTransaction, result);
        }
    }
}
