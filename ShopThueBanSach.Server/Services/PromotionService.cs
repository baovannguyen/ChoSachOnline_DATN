using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly AppDBContext _context;

        public PromotionService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<bool> CreatePromotionAsync(PromotionDTO model)
        {
            var entity = new Promotion
            {
                PromotionName = model.PromotionName,
                DiscountPercentage = model.DiscountPercentage,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };
            _context.Promotions.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePromotionAsync(string id)
        {
            var entity = await _context.Promotions.Include(p => p.SaleBooks).FirstOrDefaultAsync(p => p.PromotionId == id);
            if (entity == null) return false;

            foreach (var book in entity.SaleBooks)
            {
                book.PromotionId = null;
            }

            _context.Promotions.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PromotionDTO>> GetAllPromotionsAsync()
        {
            var promotions = await _context.Promotions.ToListAsync();
            return promotions.Select(p => new PromotionDTO
            {
                PromotionId = p.PromotionId,
                PromotionName = p.PromotionName,
                DiscountPercentage = p.DiscountPercentage,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            }).ToList();
        }

        public async Task<PromotionDTO?> GetPromotionByIdAsync(string id)
        {
            var p = await _context.Promotions.FindAsync(id);
            if (p == null) return null;
            return new PromotionDTO
            {
                PromotionId = p.PromotionId,
                PromotionName = p.PromotionName,
                DiscountPercentage = p.DiscountPercentage,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            };
        }

        public async Task<bool> UpdatePromotionAsync(string id, PromotionDTO model)
        {
            var entity = await _context.Promotions.FindAsync(id);
            if (entity == null) return false;

            entity.PromotionName = model.PromotionName;
            entity.DiscountPercentage = model.DiscountPercentage;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ApplyPromotionToBooksAsync(ApplyPromotionDTO dto)
        {
            var promotion = await _context.Promotions.FindAsync(dto.PromotionId);
            if (promotion == null) return false;

            var books = await _context.SaleBooks
                .Where(b => dto.SaleBookIds.Contains(b.SaleBookId))
                .ToListAsync();

            foreach (var book in books)
            {
                book.PromotionId = promotion.PromotionId;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
//