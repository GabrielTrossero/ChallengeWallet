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

    }

}
