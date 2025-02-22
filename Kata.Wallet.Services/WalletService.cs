using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Kata.Wallet.Dtos;
using Kata.Wallet.Database;
using Kata.Wallet.Database.Repository;

namespace Kata.Wallet.Services
{
    public interface IWalletService
    {
        WalletDto GetWalletDto(Domain.Wallet wallet); 
        Domain.Wallet ConvertToWallet(WalletDto dto);
        Task Create(Domain.Wallet wallet);
        Task<List<Domain.Wallet>> GetAll();
        Task<List<Domain.Wallet>> Filter(Domain.Wallet filter);
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

        public WalletDto GetWalletDto(Domain.Wallet wallet)
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
    }
}
