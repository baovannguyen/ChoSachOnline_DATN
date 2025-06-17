using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities.Relationships;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ShopThueBanSach.Server.Services
{
    public class SaleBookService : ISaleBookService
    {
        private readonly AppDBContext _context;

        public SaleBookService(AppDBContext context)
        {
            _context = context;
        }

        // SaleBookService.cs (chỉnh sửa lại phương thức GetAllAsync)
        public async Task<List<SaleBookDto>> GetAllAsync()
        {
            var saleBooks = await _context.SaleBooks
                .Include(sb => sb.AuthorSaleBooks)
                .Include(sb => sb.CategorySaleBooks)
                .Include(sb => sb.Promotion)
                .ToListAsync();

            return saleBooks.Select(sb => new SaleBookDto
            {
                SaleBookId = sb.SaleBookId,
                Title = sb.Title,
                Description = sb.Description,
                Publisher = sb.Publisher,
                Translator = sb.Translator,
                PackagingSize = sb.Size,
                PageCount = sb.Pages,
                Price = sb.Price,
                FinalPrice = sb.Promotion != null
        ? sb.Price * (1 - (decimal)(sb.Promotion.DiscountPercentage / 100.0))
        : sb.Price,
                PromotionName = sb.Promotion?.PromotionName,
                PromotionId = sb.PromotionId, // ✅ BỔ SUNG DÒNG NÀY
                Quantity = sb.Quantity,
                ImageUrl = sb.ImageUrl,
                IsHidden = sb.IsHidden,
                AuthorIds = sb.AuthorSaleBooks.Select(a => a.AuthorId).ToList(),
                CategoryIds = sb.CategorySaleBooks.Select(c => c.CategoryId).ToList()
            }).ToList();

        }

        public async Task<SaleBookDto?> GetByIdAsync(string id)
        {
            var sb = await _context.SaleBooks
                .Include(s => s.AuthorSaleBooks)
                .Include(s => s.CategorySaleBooks)
                .Include(s => s.Promotion) // ✅ THÊM DÒNG NÀY
                .FirstOrDefaultAsync(s => s.SaleBookId == id);

            if (sb == null) return null;

            return new SaleBookDto
            {
                SaleBookId = sb.SaleBookId,
                Title = sb.Title,
                Description = sb.Description,
                Publisher = sb.Publisher,
                Translator = sb.Translator,
                PackagingSize = sb.Size,
                PageCount = sb.Pages,
                Price = sb.Price,
                FinalPrice = sb.Promotion != null
                    ? sb.Price * (1 - (decimal)(sb.Promotion.DiscountPercentage / 100.0)) // ✅ THÊM GIÁ SAU KM
                    : sb.Price,
                PromotionName = sb.Promotion?.PromotionName, // ✅ THÊM TÊN KHUYẾN MÃI
                Quantity = sb.Quantity,
                ImageUrl = sb.ImageUrl,
                IsHidden = sb.IsHidden,
                AuthorIds = sb.AuthorSaleBooks.Select(a => a.AuthorId).ToList(),
                CategoryIds = sb.CategorySaleBooks.Select(c => c.CategoryId).ToList(),
                PromotionId = sb.PromotionId // ✅ Nếu cần trả về để binding
            };
        }

        public async Task<string> CreateAsync(CreateSaleBookDto dto)
        {
            var saleBook = new SaleBook
            {
                SaleBookId = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Description = dto.Description,
                Publisher = dto.Publisher,
                Translator = dto.Translator,
                Size = dto.Size,
                Pages = dto.Pages,
                Price = dto.Price,
                Quantity = dto.Quantity,
                ImageUrl = dto.ImageUrl,
                IsHidden = dto.IsHidden,
                AuthorSaleBooks = dto.AuthorIds.Select(aid => new AuthorSaleBook { AuthorId = aid }).ToList(),
                CategorySaleBooks = dto.CategoryIds.Select(cid => new CategorySaleBook { CategoryId = cid }).ToList()
            };

            // ✅ Nếu có PromotionId → kiểm tra hợp lệ
            if (!string.IsNullOrWhiteSpace(dto.PromotionId))
            {
                var promotion = await _context.Promotions.FindAsync(dto.PromotionId);
                if (promotion != null && promotion.StartDate <= DateTime.UtcNow && promotion.EndDate >= DateTime.UtcNow)
                {
                    saleBook.PromotionId = promotion.PromotionId;
                    saleBook.FinalPrice = saleBook.Price * (1 - ((decimal)promotion.DiscountPercentage / 100));

                }
                else
                {
                    saleBook.PromotionId = null; // ❗Không hợp lệ thì không gán
                    saleBook.FinalPrice = saleBook.Price;
                }
            }
            else
            {
                saleBook.PromotionId = null; // ❗Không nhập mã thì null
                saleBook.FinalPrice = saleBook.Price;
            }

            _context.SaleBooks.Add(saleBook);
            await _context.SaveChangesAsync();
            return saleBook.SaleBookId;
        }

        public async Task<bool> UpdateAsync(string id, SaleBookDto dto)
        {
            var saleBook = await _context.SaleBooks
                .Include(sb => sb.AuthorSaleBooks)
                .Include(sb => sb.CategorySaleBooks)
                .FirstOrDefaultAsync(sb => sb.SaleBookId == id);

            if (saleBook == null) return false;

            // ✅ Cập nhật thông tin cơ bản
            saleBook.Title = dto.Title;
            saleBook.Description = dto.Description;
            saleBook.Publisher = dto.Publisher;
            saleBook.Translator = dto.Translator;
            saleBook.Size = dto.PackagingSize;
            saleBook.Pages = dto.PageCount;
            saleBook.Price = dto.Price;
            saleBook.Quantity = dto.Quantity;
            saleBook.ImageUrl = dto.ImageUrl;
            saleBook.IsHidden = dto.IsHidden;

            // Nếu chuỗi rỗng hoặc null thì gỡ khuyến mãi
            saleBook.PromotionId = string.IsNullOrWhiteSpace(dto.PromotionId) ? null : dto.PromotionId;


            // ✅ Cập nhật AuthorSaleBooks
            saleBook.AuthorSaleBooks.Clear();
            saleBook.AuthorSaleBooks = dto.AuthorIds
                .Select(authorId => new AuthorSaleBook
                {
                    SaleBookId = id,
                    AuthorId = authorId
                }).ToList();

            // ✅ Cập nhật CategorySaleBooks
            saleBook.CategorySaleBooks.Clear();
            saleBook.CategorySaleBooks = dto.CategoryIds
                .Select(categoryId => new CategorySaleBook
                {
                    SaleBookId = id,
                    CategoryId = categoryId
                }).ToList();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var saleBook = await _context.SaleBooks.FindAsync(id);
            if (saleBook == null) return false;
            _context.SaleBooks.Remove(saleBook);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetVisibilityAsync(string id, bool isHidden)
        {
            var saleBook = await _context.SaleBooks.FindAsync(id);
            if (saleBook == null) return false;
            saleBook.IsHidden = isHidden;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
