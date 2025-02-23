using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Dtos
{
    public interface ITransactionMappingService
    {
        TransactionDto ConvertToTransactionDto(Domain.Transaction transaction);
        Domain.Transaction ConvertToTransaction(TransactionDto dto);
        List<TransactionDto> ConvertToTransactionDto(List<Domain.Transaction> transactions);
        List<Domain.Transaction> ConvertToTransaction(List<TransactionDto> transactions);
    }

    public class TransactionMappingService : ITransactionMappingService
    {
        private readonly IMapper _mapper;

        public TransactionMappingService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public TransactionDto ConvertToTransactionDto(Domain.Transaction transaction)
        {
            return _mapper.Map<TransactionDto>(transaction);
        }

        public List<TransactionDto> ConvertToTransactionDto(List<Domain.Transaction> transactions)
        {
            return transactions.Select(transaction => _mapper.Map<TransactionDto>(transaction)).ToList();
        }

        public Domain.Transaction ConvertToTransaction(TransactionDto transactionDto)
        {
            return _mapper.Map<Domain.Transaction>(transactionDto);
        }

        public List<Domain.Transaction> ConvertToTransaction(List<TransactionDto> transactionsDto)
        {
            return transactionsDto.Select(transaction => _mapper.Map<Domain.Transaction>(transaction)).ToList();
        }
    }
}
