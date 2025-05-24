using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Services.Interfaces;
using System;

namespace ShopThueBanSach.Server.Services
{
    public class SellBookService : ISellBookService
    {
        private readonly AppDBContext _context;

        public SellBookService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<SellBook>> GetAllAsync()
        {
            return await _context.SellBooks.ToListAsync();
        }

        public async Task<SellBook?> GetByIdAsync(int id)
        {
            return await _context.SellBooks.FindAsync(id);
        }

        public async Task<SellBook> CreateAsync(SellBook sachBan)
        {
            _context.SellBooks.Add(sachBan);
            await _context.SaveChangesAsync();
            return sachBan;
        }

        public async Task<bool> UpdateAsync(SellBook sachBan)
        {
            var existing = await _context.SellBooks.FindAsync(sachBan.SellBookId);
            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(sachBan);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sachBan = await _context.SellBooks.FindAsync(id);
            if (sachBan == null)
                return false;

            _context.SellBooks.Remove(sachBan);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
