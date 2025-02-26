using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Kata.Wallet.Dtos;
using Kata.Wallet.Database;
using Kata.Wallet.Database.Repository;
using Kata.Wallet.Domain;
using System.Resources;

namespace Kata.Wallet.Services
{
    public interface IWalletService
    {
        Task<string?> Create(Domain.Wallet wallet);
        Task<List<Domain.Wallet>> GetAll();
        Task<List<Domain.Wallet>> Filter(string? userDocument = null, Currency? currency = null);
        Task<Domain.Wallet?> GetById(int idWallet);
        Task Update(Domain.Wallet wallet);
        Task<Domain.Wallet?> GetWallet(string userDoc, Domain.Currency currency);
    }

    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ResourceManager _resourceManager;

        public WalletService(IWalletRepository walletRepository, ResourceManager resourceManager) 
        {
            _walletRepository = walletRepository;
            _resourceManager = resourceManager;
        }

        public async Task<string?> Create(Domain.Wallet wallet)
        {
            if (wallet.Balance < 0)
            {
                return _resourceManager.GetString("Range_Balance");
            }

            var existingWallets = await _walletRepository.Filter(wallet.UserDocument, wallet.Currency);

            if (existingWallets.Any(w => w.Currency == wallet.Currency))
            {
                return _resourceManager.GetString("WalletAlreadyExistsWithSameCurrency");
            }

            await _walletRepository.Create(wallet);

            return string.Empty;
        }

        public async Task<Domain.Wallet?> GetWallet(string userDoc, Domain.Currency currency)
        {
            return await _walletRepository.GetWallet(userDoc, currency);
        }

        public async Task<List<Domain.Wallet>> GetAll()
        {
            return await _walletRepository.Filter();
        }

        public async Task<List<Domain.Wallet>> Filter(string? userDocument = null, Currency? currency = null)
        {
            return await _walletRepository.Filter(userDocument, currency);
        }

        public async Task<Domain.Wallet?> GetById(int idWallet)
        {
            return await _walletRepository.GetById(idWallet);
        }

        public async Task Update(Domain.Wallet wallet)
        {
            await _walletRepository.Update(wallet);
        }
    }
}
