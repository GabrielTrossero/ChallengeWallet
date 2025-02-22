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
        Task Create(Domain.Wallet wallet);
        Task<List<Domain.Wallet>> Filter(Domain.Wallet filter);
    }

    public class WalletRepository : IWalletRepository
    {
        private readonly DataContext _context;

        public WalletRepository(DataContext context) 
        {
            _context = context;
        }

        public async Task Create(Domain.Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Domain.Wallet>> Filter(Domain.Wallet filter)
        {
            var query = _context.Wallets.AsQueryable();

            // Filtrar por moneda si se proporciona
            if (Enum.IsDefined(typeof(Currency), filter.Currency))
            {
                query = query.Where(w => w.Currency == filter.Currency);
            }

            // Filtrar por documento de usuario si se proporciona
            if (!string.IsNullOrEmpty(filter.UserDocument))
            {
                query = query.Where(w => w.UserDocument == filter.UserDocument);
            }

            return await query.ToListAsync();
        }
    }
}
