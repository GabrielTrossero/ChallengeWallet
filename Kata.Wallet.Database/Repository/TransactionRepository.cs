using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Database.Repository
{
    public interface ITransactionRepository
    {
        Task<Domain.Transaction> Create(Domain.Transaction transaction);
        Task<IDbContextTransaction> BeginTransaction();
        bool IsInMemoryDatabase();
        Task<List<Domain.Transaction>> GetTransactions(int idWallet);
    }
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext _context;

        public TransactionRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<Domain.Transaction> Create(Domain.Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public bool IsInMemoryDatabase()
        {
            return _context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        }

        public async Task<List<Domain.Transaction>> GetTransactions(int idWallet)
        {
            var transactions = await _context.Transactions
                                              .Where(t => t.WalletIncoming.Id == idWallet || t.WalletOutgoing.Id == idWallet)
                                              .Include(t => t.WalletIncoming) // Cargar la wallet de ingreso
                                              .Include(t => t.WalletOutgoing) // Cargar la wallet de egreso
                                              .ToListAsync();

            return transactions;
        }
    }
}
