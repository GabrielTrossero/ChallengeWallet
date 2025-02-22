using AutoMapper;
using Kata.Wallet.Dtos;

namespace Kata.Wallet.Api.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Domain.Transaction, TransactionDto>();
        CreateMap<TransactionDto, Domain.Transaction>();

        CreateMap<Domain.Wallet, WalletDto>();
        CreateMap<WalletDto, Domain.Wallet>();
        CreateMap<WalletCreateDto, Domain.Wallet>().IncludeBase<WalletDto, Domain.Wallet>();
    }
}
