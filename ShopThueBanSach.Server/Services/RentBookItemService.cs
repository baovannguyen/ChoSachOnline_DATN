using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Services
{
    public class RentBookItemService : IRentBookItemService
    {
        private readonly AppDBContext _context;

        public RentBookItemService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<RentBookItemDto>> GetAllAsync()
        {
            return await _context.RentBookItems
                .Include(x => x.RentBook)
                .Select(x => new RentBookItemDto
                {
                    RentBookItemId = x.RentBookItemId,
                    RentBookId = x.RentBookId,
                    RentBookTitle = x.RentBook != null ? x.RentBook.Title : null,
                    Status = x.Status,
                    Condition = x.Condition,
                    IsHidden = x.IsHidden
                })
                .ToListAsync();
        }

        public async Task<RentBookItemDto?> GetByIdAsync(string id)
        {
            var item = await _context.RentBookItems
                .Include(x => x.RentBook)
                .FirstOrDefaultAsync(x => x.RentBookItemId == id);

            if (item == null) return null;

            return new RentBookItemDto
            {
                RentBookItemId = item.RentBookItemId,
                RentBookId = item.RentBookId,
                RentBookTitle = item.RentBook?.Title,
                Status = item.Status,
                Condition = item.Condition,
                IsHidden = item.IsHidden
            };
        }

        public async Task<RentBookItemDto?> CreateAsync(RentBookItemDto dto)
        {
            var rentBook = await _context.RentBooks
                .Include(r => r.RentBookItems)
                .FirstOrDefaultAsync(r => r.RentBookId == dto.RentBookId);

            if (rentBook == null)
                return null;

            // Kiểm tra số lượng sách con đã tạo
            int currentCount = rentBook.RentBookItems.Count;
            if (currentCount >= rentBook.Quantity)
                throw new InvalidOperationException("Số lượng sách thuê đã đạt tối đa.");

            var entity = new RentBookItem
            {
                RentBookId = dto.RentBookId,
                Condition = dto.Condition,
                Status = RentBookItemStatus.Available,
                IsHidden = dto.IsHidden
            };

            _context.RentBookItems.Add(entity);
            await _context.SaveChangesAsync();

            dto.RentBookItemId = entity.RentBookItemId;
            return dto;
        }


        public async Task<RentBookItemDto?> UpdateAsync(string id, RentBookItemDto dto)
        {
            var entity = await _context.RentBookItems.FindAsync(id);
            if (entity == null || dto.Condition < 0 || dto.Condition > 100)
                return null;

            entity.RentBookId = dto.RentBookId;
            entity.Status = dto.Status;
            entity.Condition = dto.Condition;
            entity.IsHidden = dto.Condition >= 80;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }


        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _context.RentBookItems.FindAsync(id);
            if (entity == null) return false;

            _context.RentBookItems.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}