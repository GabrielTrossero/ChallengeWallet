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
    }
}
