using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Services
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly AppDBContext _context;

        public DiscountCodeService(AppDBContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(DiscountCodeDTO model)
        {
            var entity = new DiscountCode
            {
                DiscountCodeName = model.DiscountCodeName,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                AvailableQuantity = model.AvailableQuantity,
                RequiredPoints = model.RequiredPoints,
                DiscountValue = model.DiscountValue
            };

            _context.DiscountCodes.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _context.DiscountCodes.FindAsync(id);
            if (entity == null) return false;

            _context.DiscountCodes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<DiscountCodeDTO>> GetAllAsync()
        {
            return await _context.DiscountCodes
                .Select(d => new DiscountCodeDTO
                {
                    DiscountCodeId = d.DiscountCodeId,
                    DiscountCodeName = d.DiscountCodeName,
                    Description = d.Description,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    AvailableQuantity = d.AvailableQuantity,
                    RequiredPoints = d.RequiredPoints,
                    DiscountValue = d.DiscountValue
                }).ToListAsync();
        }

        public async Task<DiscountCodeDTO?> GetByIdAsync(string id)
        {
            var d = await _context.DiscountCodes.FindAsync(id);
            if (d == null) return null;

            return new DiscountCodeDTO
            {
                DiscountCodeId = d.DiscountCodeId,
                DiscountCodeName = d.DiscountCodeName,
                Description = d.Description,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                AvailableQuantity = d.AvailableQuantity,
                RequiredPoints = d.RequiredPoints,
                DiscountValue = d.DiscountValue
            };
        }

        public async Task<bool> UpdateAsync(string id, DiscountCodeDTO model)
        {
            var entity = await _context.DiscountCodes.FindAsync(id);
            if (entity == null) return false;

            entity.DiscountCodeName = model.DiscountCodeName;
            entity.Description = model.Description;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            entity.AvailableQuantity = model.AvailableQuantity;
            entity.RequiredPoints = model.RequiredPoints;
            entity.DiscountValue = model.DiscountValue;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<DiscountCodeDTO>> GetAvailableForExchangeAsync()
        {
            var today = DateTime.UtcNow;
            return await _context.DiscountCodes
                .Where(dc => dc.AvailableQuantity > 0 && dc.StartDate <= today && dc.EndDate >= today)
                .Select(d => new DiscountCodeDTO
                {
                    DiscountCodeId = d.DiscountCodeId,
                    DiscountCodeName = d.DiscountCodeName,
                    Description = d.Description,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    AvailableQuantity = d.AvailableQuantity,
                    RequiredPoints = d.RequiredPoints,
                    DiscountValue = d.DiscountValue
                }).ToListAsync();
        }

    }
}
