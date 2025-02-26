using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Kata.Wallet.Services;
using Kata.Wallet.Domain;
using Kata.Wallet.Database.Repository;
using System.Resources;

namespace Kata.Wallet.Tests
{

    public class WalletServiceTests
    {
        private readonly Mock<IWalletRepository> _mockWalletRepository;
        private readonly Mock<ResourceManager> _mockResourceManager;
        private readonly IWalletService _walletService;


        public WalletServiceTests()
        {
            // Crearte mocks
            _mockWalletRepository = new Mock<IWalletRepository>();
            _mockResourceManager = new Mock<ResourceManager>();

            // Injecting the mocks into the service
            _walletService = new WalletService(_mockWalletRepository.Object, _mockResourceManager.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnMessage_WhenUserAlreadyHasWalletWithSameCurrency()
        {
            // Arrange
            var wallet = new Domain.Wallet
            {
                Id = 2,
                Balance = 200.0m,
                UserDocument = "40992777",
                UserName = "Gabriel",
                Currency = Currency.USD
            };

            var existingWallet = new Domain.Wallet
            {
                Id = 1,
                Balance = 100.0m,
                UserDocument = "40992777",
                UserName = "Gabriel",
                Currency = Currency.USD
            };

            // Simulate the repository returns an existing wallet
            _mockWalletRepository.Setup(repo => repo.Filter(It.IsAny<Domain.Wallet>()))
                                 .ReturnsAsync(new List<Domain.Wallet> { existingWallet });

            // Simulate the _resourceManager returning the error message
            _mockResourceManager.Setup(rm => rm.GetString("WalletAlreadyExistsWithSameCurrency"))
                                .Returns("A wallet with this currency already exists for the given user document.");

            // Act
            var result = await _walletService.Create(wallet);

            // Assert
            Assert.Equal("A wallet with this currency already exists for the given user document.", result);
        }


        [Fact]
        public async Task Create_ShouldAllowWalletCreation_WhenUserHasDifferentCurrencyWallet()
        {
            // Arrange
            var newWallet = new Domain.Wallet
            {
                Id = 2,
                Balance = 200.0m,
                UserDocument = "40992777",
                UserName = "Gabriel",
                Currency = Currency.EUR
            };

            var existingWallet = new Domain.Wallet
            {
                Id = 1,
                Balance = 100.0m,
                UserDocument = "40992777",
                UserName = "Gabriel",
                Currency = Currency.USD
            };

            // Simulate the repository returns an existing wallet
            _mockWalletRepository.Setup(repo => repo.Filter(It.IsAny<Domain.Wallet>()))
                                 .ReturnsAsync(new List<Domain.Wallet> { existingWallet });

            // Simulate the repository creates the new wallet
            _mockWalletRepository.Setup(repo => repo.Create(It.IsAny<Domain.Wallet>()))
                                 .ReturnsAsync(newWallet);

            // Act
            var result = await _walletService.Create(newWallet);

            // Assert
            Assert.Equal(string.Empty, result); // We verify that the result is an empty string, indicating success
        }

        [Fact]
        public async Task Create_ShouldReturnMessage_WhenBalanceIsNegative()
        {
            // Arrange
            var wallet = new Domain.Wallet
            {
                Id = 2,
                Balance = -50.0m,
                UserDocument = "40992777",
                UserName = "Gabriel",
                Currency = Currency.USD
            };

            // Simulate the resourceManager returning the error message for negative balance
            _mockResourceManager.Setup(rm => rm.GetString("Range_Balance"))
                                .Returns("Balance cannot be negative.");

            // Act
            var result = await _walletService.Create(wallet);

            // Assert
            Assert.Equal("Balance cannot be negative.", result); // We check the returned message
        }

        [Fact]
        public async Task GetWallet_ShouldReturnWallet_WhenWalletExists()
        {
            // Arrange
            var expectedWallet = new Domain.Wallet { Id = 1, Currency = Domain.Currency.USD, UserDocument = "40992777" };
            _mockWalletRepository.Setup(repo => repo.GetWallet("40992777", Domain.Currency.USD))
                                 .ReturnsAsync(expectedWallet);

            // Act
            var result = await _walletService.GetWallet("40992777", Domain.Currency.USD);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedWallet.Id, result.Id);
            Assert.Equal(expectedWallet.Currency, result.Currency);
            Assert.Equal(expectedWallet.UserDocument, result.UserDocument);
        }

        [Fact]
        public async Task GetWallet_ShouldReturnNull_WhenWalletDoesNotExist()
        {
            // Arrange
            _mockWalletRepository.Setup(repo => repo.GetWallet("40992777", Domain.Currency.USD))
                                 .ReturnsAsync((Domain.Wallet?)null);

            // Act
            var result = await _walletService.GetWallet("40992777", Domain.Currency.USD);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfWallets()
        {
            // Arrange
            var wallets = new List<Domain.Wallet>
            {
                new Domain.Wallet { Id = 1, Currency = Domain.Currency.USD },
                new Domain.Wallet { Id = 2, Currency = Domain.Currency.EUR },
                new Domain.Wallet { Id = 3, Currency = Domain.Currency.ARS },
                new Domain.Wallet { Id = 4, Currency = Domain.Currency.ARS }
            };

            _mockWalletRepository.Setup(repo => repo.Filter(It.IsAny<Domain.Wallet>())).ReturnsAsync(wallets);

            // Act
            var result = await _walletService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnWallet_WhenWalletExists()
        {
            // Arrange
            var wallet = new Domain.Wallet { Id = 1, Currency = Domain.Currency.USD };
            _mockWalletRepository.Setup(repo => repo.GetById(1)).ReturnsAsync(wallet);

            // Act
            var result = await _walletService.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenWalletDoesNotExist()
        {
            // Arrange
            _mockWalletRepository.Setup(repo => repo.GetById(99)).ReturnsAsync((Domain.Wallet?)null);

            // Act
            var result = await _walletService.GetById(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Update_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var wallet = new Domain.Wallet { Id = 1, Currency = Domain.Currency.USD };

            // Act
            await _walletService.Update(wallet);

            // Assert
            _mockWalletRepository.Verify(repo => repo.Update(wallet), Times.Once);
        }
    }
}
