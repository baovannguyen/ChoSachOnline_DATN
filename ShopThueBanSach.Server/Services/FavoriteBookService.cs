using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Services
{
    public class FavoriteBookService : IFavoriteBookService
    {
        private readonly AppDBContext _context;

        public FavoriteBookService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<FavoriteBookDto>> GetFavoriteBooksAsync(string userId)
        {
            var favorites = await _context.FavoriteBooks
                .Where(f => f.UserId == userId)
                .Include(f => f.SaleBook)
                    .ThenInclude(sb => sb.Promotion)
                .Include(f => f.User) // 👈 Thêm Include User
                .ToListAsync();

            return favorites.Select(f => new FavoriteBookDto
            {
                SaleBookId = f.SaleBook.SaleBookId,
                Title = f.SaleBook.Title,
                ImageUrl = f.SaleBook.ImageUrl,
                Price = f.SaleBook.Price,
                PromotionName = f.SaleBook.Promotion?.PromotionName,
                DiscountPercentage = f.SaleBook.Promotion?.DiscountPercentage,
                UserName = f.User.UserName // 👈 Thêm UserName vào kết quả
            }).ToList();
        }



        public async Task<bool> AddFavoriteBookAsync(string userId, string saleBookId)
        {
            var exists = await _context.FavoriteBooks
                .AnyAsync(f => f.UserId == userId && f.SaleBookId == saleBookId);

            if (exists) return false;

            _context.FavoriteBooks.Add(new FavoriteBook
            {
                UserId = userId,
                SaleBookId = saleBookId
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFavoriteBookAsync(string userId, string saleBookId)
        {
            var favorite = await _context.FavoriteBooks
                .FirstOrDefaultAsync(f => f.UserId == userId && f.SaleBookId == saleBookId);

            if (favorite == null) return false;

            _context.FavoriteBooks.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
