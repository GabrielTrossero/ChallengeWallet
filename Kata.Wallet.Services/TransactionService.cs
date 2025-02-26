using AutoMapper;
using Kata.Wallet.Database.Repository;
using Kata.Wallet.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using Kata.Wallet.Domain;

namespace Kata.Wallet.Services
{
    public interface ITransactionService 
    {
        Task<string?> Create(Domain.Transaction transaction, int idWalletOrigin, int idWalletDestino);
        Task<List<Domain.Transaction>> GetTransactions(int idWallet);
        Task<Domain.Transaction?> GetLastTransaction(int? idWalletOrigin, int? idWalletDestination);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletService _walletService;
        private readonly ResourceManager _resourceManager;

        public TransactionService(ITransactionRepository transactionRepository, IWalletService walletService, ResourceManager resourceManager)
        {
            _transactionRepository = transactionRepository;
            _walletService = walletService;
            _resourceManager = resourceManager;
        }

        public async Task<string?> Create(Domain.Transaction transaction, int idWalletOrigin, int idWalletDestino)
        {

            if (transaction.Amount < 0)
            {
                return _resourceManager.GetString("Positive_Amount");
            }

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

        private async Task<string?> ExecuteTransaction(Domain.Wallet walletOrigin, Domain.Wallet walletDestination, Domain.Transaction transaction, bool isInMemoryDb)
        {
                // Update balance of wallets
                walletOrigin.Balance -= transaction.Amount;
                walletDestination.Balance += transaction.Amount;

                // Asign wallets to transaction and date
                transaction.WalletIncoming = walletDestination;
                transaction.WalletOutgoing = walletOrigin;
                transaction.Date = DateTime.UtcNow;


                // If it's not an InMemory Database, then make a transaction
                if (!isInMemoryDb)
                {
                    using (var transactionScope = await _transactionRepository.BeginTransaction())
                    {
                        await DoTransaction(transaction, walletOrigin, walletDestination);

                        await transactionScope.CommitAsync();
                    }
                }
                else
                {
                    await DoTransaction(transaction, walletOrigin, walletDestination);
                }

                return string.Empty;
        }

        private async Task DoTransaction(Domain.Transaction transaction, Domain.Wallet walletOrigin, Domain.Wallet walletDestination)
        {
            var createdTransaction = await _transactionRepository.Create(transaction);

            // Add transaction to wallets
            walletOrigin.OutgoingTransactions ??= new List<Domain.Transaction>();
            walletDestination.IncomingTransactions ??= new List<Domain.Transaction>();

            walletOrigin.OutgoingTransactions.Add(transaction);
            walletDestination.IncomingTransactions.Add(transaction);

            await _walletService.Update(walletOrigin);
            await _walletService.Update(walletDestination);
        }

        public async Task<List<Domain.Transaction>> GetTransactions(int idWallet)
        {
            var transactions = await _transactionRepository.GetTransactions(idWallet);

            return setAmount(transactions, idWallet);
        }

        // Set amount negative if is WalletOutgoing
        private List<Domain.Transaction> setAmount(List<Domain.Transaction> transactions, int idWallet)
        {
            foreach (var transaction in transactions)
            {
                if (transaction.WalletOutgoing?.Id == idWallet)
                {
                    transaction.Amount *= -1;
                }
            }

            return transactions;
        }
        public async Task<Domain.Transaction?> GetLastTransaction(int? idWalletOrigin, int? idWalletDestination)
        {
            return await _transactionRepository.GetLastTransaction(idWalletOrigin, idWalletDestination);
        }

    }
}
