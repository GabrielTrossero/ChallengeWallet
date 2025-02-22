﻿using Kata.Wallet.Domain;
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
        Task<Domain.Wallet?> GetById(int id);
        Task Update(Domain.Wallet wallet);
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

            // Filter by Currency if provided
            if (Enum.IsDefined(typeof(Currency), filter.Currency))
            {
                query = query.Where(w => w.Currency == filter.Currency);
            }

            // Filter by UserDocument if provided
            if (!string.IsNullOrEmpty(filter.UserDocument))
            {
                query = query.Where(w => w.UserDocument == filter.UserDocument);
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
