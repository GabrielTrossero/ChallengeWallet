using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Dtos
{
    public interface IWalletMappingService
    {
        WalletDto ConvertToWalletDto(Domain.Wallet wallet);
        List<WalletDto> ConvertToWalletDto(List<Domain.Wallet> wallets);
        Domain.Wallet ConvertToWallet(WalletDto walletDto);
        List<Domain.Wallet> ConvertToTransaction(List<WalletDto> walletsDto);
    }

    public class WalletMappingService : IWalletMappingService
    {
        private readonly IMapper _mapper;

        public WalletMappingService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public WalletDto ConvertToWalletDto(Domain.Wallet wallet)
        {
            return _mapper.Map<WalletDto>(wallet);
        }

        public List<WalletDto> ConvertToWalletDto(List<Domain.Wallet> wallets)
        {
            return wallets.Select(wallet => _mapper.Map<WalletDto>(wallet)).ToList();
        }

        public Domain.Wallet ConvertToWallet(WalletDto walletDto)
        {
            return _mapper.Map<Domain.Wallet>(walletDto);
        }

        public List<Domain.Wallet> ConvertToTransaction(List<WalletDto> walletsDto)
        {
            return walletsDto.Select(wallet => _mapper.Map<Domain.Wallet>(wallet)).ToList();
        }
    }
}
