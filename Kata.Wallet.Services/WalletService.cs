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

namespace Kata.Wallet.Services
{
    public interface IWalletService
    {
        WalletDto ConvertToWalletDto(Domain.Wallet wallet);
        Domain.Wallet ConvertToWallet(WalletDto dto);
        Task Create(Domain.Wallet wallet);
        Task<List<Domain.Wallet>> GetAll();
        Task<List<Domain.Wallet>> Filter(Domain.Wallet filter);
        Task<Domain.Wallet?> GetById(int idWallet);
        Task Update(Domain.Wallet wallet);
    }

    public class WalletService : IWalletService
    {
        private readonly IMapper _mapper;
        private readonly IWalletRepository _walletRepository;

        public WalletService(IMapper mapper, IWalletRepository walletRepository) 
        {
            _mapper = mapper;
            _walletRepository = walletRepository;
        }

        public WalletDto ConvertToWalletDto(Domain.Wallet wallet)
        {
            return _mapper.Map<WalletDto>(wallet);
        }

        public Domain.Wallet ConvertToWallet(WalletDto dto)
        {
            return _mapper.Map<Domain.Wallet>(dto);
        }

        public async Task Create(Domain.Wallet wallet)
        {
            await _walletRepository.Create(wallet);
        }

        public async Task<List<Domain.Wallet>> GetAll()
        {
            var filter = new Domain.Wallet();
            return await _walletRepository.Filter(filter);
        }

        public async Task<List<Domain.Wallet>> Filter(Domain.Wallet filter)
        {
            return await _walletRepository.Filter(filter);
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
