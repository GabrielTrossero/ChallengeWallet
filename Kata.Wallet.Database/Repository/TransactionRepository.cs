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
        Task<Domain.Transaction?> GetLastTransaction(int? idWalletOrigin, int? idWalletDestination);
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
            transaction.Id = 0; // Auotincrement

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
                                              .Include(t => t.WalletIncoming) // Load WalletIncoming
                                              .Include(t => t.WalletOutgoing) // Load WalletOutgoing
                                              .ToListAsync();

            return transactions;
        }

        public async Task<Domain.Transaction?> GetLastTransaction(int? idWalletOrigin, int? idWalletDestination)
        {
            var transaction = new Domain.Transaction();

            // Verificamos que al menos uno de los parámetros se pase
            if (!idWalletOrigin.HasValue && !idWalletDestination.HasValue)
            {
                return transaction;
            }

            var query = _context.Transactions.AsQueryable();

            // Filtramos si se pasa un idWalletOrigin
            if (idWalletOrigin.HasValue)
            {
                query = query.Where(t => t.WalletIncoming.Id == idWalletOrigin || t.WalletOutgoing.Id == idWalletOrigin);
            }

            // Filtramos si se pasa un idWalletDestination
            if (idWalletDestination.HasValue)
            {
                query = query.Where(t => t.WalletIncoming.Id == idWalletDestination || t.WalletOutgoing.Id == idWalletDestination);
            }

            // Obtenemos la última transacción, ordenando por fecha descendente
            transaction = await query.OrderByDescending(t => t.Date).FirstOrDefaultAsync();

            return transaction;
        }
    }
}
