using Kata.Wallet.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Database.Repository
{
    public interface IWalletRepository
    {
        Task<Domain.Wallet> Create(Domain.Wallet wallet);
        Task<List<Domain.Wallet>> Filter(string? userDocument = null, Currency? currency = null);
        Task<Domain.Wallet?> GetById(int id);
        Task Update(Domain.Wallet wallet);
        Task<Domain.Wallet?> GetWallet(string userDoc, Domain.Currency currency);
    }

    public class WalletRepository : IWalletRepository
    {
        private readonly DataContext _context;

        public WalletRepository(DataContext context) 
        {
            _context = context;
        }

        public async Task<Domain.Wallet> Create(Domain.Wallet wallet)
        {
            wallet.Id = 0; // Autoincrement

            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }

        public async Task<Domain.Wallet?> GetWallet(string userDoc, Domain.Currency currency)
        {
            return await _context.Wallets
                                 .FirstOrDefaultAsync(w => w.UserDocument == userDoc && w.Currency == currency);
        }

        public async Task<List<Domain.Wallet>> Filter(string? userDocument = null, Currency? currency = null)
        {
            var query = _context.Wallets.AsQueryable();

                // Filter by Currency if provided
                if (currency != null)
                {
                    query = query.Where(w => w.Currency == currency);
                }

                // Filter by UserDocument if provided
                if (!string.IsNullOrEmpty(userDocument))
                {
                    query = query.Where(w => w.UserDocument.Trim() == userDocument.Trim());
                }

            return await query.ToListAsync();
        }

        public async Task<Domain.Wallet?> GetById(int id)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task Update(Domain.Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
