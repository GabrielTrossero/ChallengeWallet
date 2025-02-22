using AutoMapper;
using Kata.Wallet.Database.Repository;
using Kata.Wallet.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace Kata.Wallet.Services
{
    public interface ITransactionService 
    {
        TransactionDto ConvertToTransactionDto(Domain.Transaction transaction);
        Domain.Transaction ConvertToTransaction(TransactionDto dto);
        Task<string> Create(Domain.Transaction transaction, int idWalletOrigin, int idWalletDestino);
    }

    public class TransactionService : ITransactionService
    {
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletService _walletService;
        private readonly ResourceManager _resourceManager;

        public TransactionService(IMapper mapper, ITransactionRepository transactionRepository, IWalletService walletService, ResourceManager resourceManager)
        {
            _mapper = mapper;
            _transactionRepository = transactionRepository;
            _walletService = walletService;
            _resourceManager = resourceManager;
        }

        public TransactionDto ConvertToTransactionDto(Domain.Transaction transaction)
        {
            return _mapper.Map<TransactionDto>(transaction);
        }

        public Domain.Transaction ConvertToTransaction(TransactionDto dto)
        {
            return _mapper.Map<Domain.Transaction>(dto);
        }

        public async Task<string> Create(Domain.Transaction transaction, int idWalletOrigin, int idWalletDestino)
        {
            var walletOrigin = await _walletService.GetById(idWalletOrigin);
            var walletDestination = await _walletService.GetById(idWalletDestino);

            if (walletOrigin == null || walletDestination == null)
            {
                return _resourceManager.GetString("WalletNotFound");
            }

            if (walletOrigin.Currency != walletDestination.Currency)
            {
                return _resourceManager.GetString("DifferentCurrencies");
            }

            if (walletOrigin.Balance < transaction.Amount)
            {
                return _resourceManager.GetString("InsufficientFunds");
            }


            var isInMemoryDb = _transactionRepository.IsInMemoryDatabase();

            return await ExecuteTransaction(walletOrigin, walletDestination, transaction, isInMemoryDb);
        }

        private async Task<string> ExecuteTransaction(Domain.Wallet walletOrigin, Domain.Wallet walletDestination, Domain.Transaction transaction, bool isInMemoryDb)
        {
            try
            {
                // Update balance of wallets
                walletOrigin.Balance -= transaction.Amount;
                walletDestination.Balance += transaction.Amount;

                // If it's not an InMemory Database, then make a transaction
                if (!isInMemoryDb)
                {
                    using (var transactionScope = await _transactionRepository.BeginTransaction())
                    {
                        await _walletService.Update(walletOrigin);
                        await _walletService.Update(walletDestination);

                        await _transactionRepository.Create(transaction);

                        await transactionScope.CommitAsync();
                    }
                }
                else
                {
                    await _walletService.Update(walletOrigin);
                    await _walletService.Update(walletDestination);

                    await _transactionRepository.Create(transaction);
                }

                return string.Empty;
            }
            catch
            {
                return _resourceManager.GetString("TransactionError");
            }
        }
    }
}
