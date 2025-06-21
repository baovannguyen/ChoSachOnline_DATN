using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities.Relationships;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Models.BooksModel.SaleBooks;

namespace ShopThueBanSach.Server.Services
{
    public class SaleBookService : ISaleBookService
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public SaleBookService(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                    ? sb.Price * (1 - (decimal)(sb.Promotion.DiscountPercentage / 100)) // ✅ THÊM GIÁ SAU KM
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
                IsHidden = dto.IsHidden,
                AuthorSaleBooks = dto.AuthorIds.Select(aid => new AuthorSaleBook { AuthorId = aid }).ToList(),
                CategorySaleBooks = dto.CategoryIds.Select(cid => new CategorySaleBook { CategoryId = cid }).ToList()
            };

            // ✅ Lưu ảnh nếu có
            if (dto.ImageUrl != null && dto.ImageUrl.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "Images", "SaleBooks");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{dto.ImageUrl.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageUrl.CopyToAsync(stream);
                }

                saleBook.ImageUrl = $"/Images/SaleBooks/{uniqueFileName}";
            }

            // ✅ Áp dụng khuyến mãi nếu có
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
                    saleBook.FinalPrice = saleBook.Price;
                }
            }
            else
            {
                saleBook.FinalPrice = saleBook.Price;
            }

            _context.SaleBooks.Add(saleBook);
            await _context.SaveChangesAsync();
            return saleBook.SaleBookId;
        }


        public async Task<bool> UpdateAsync(string id, UpdateSaleBookDto dto)
        {
            var saleBook = await _context.SaleBooks
                .Include(sb => sb.AuthorSaleBooks)
                .Include(sb => sb.CategorySaleBooks)
                .FirstOrDefaultAsync(sb => sb.SaleBookId == id);

            if (saleBook == null) return false;

            // Cập nhật thông tin cơ bản
            saleBook.Title = dto.Title;
            saleBook.Description = dto.Description;
            saleBook.Publisher = dto.Publisher;
            saleBook.Translator = dto.Translator;
            saleBook.Size = dto.Size;
            saleBook.Pages = dto.Pages;
            saleBook.Price = dto.Price;
            saleBook.Quantity = dto.Quantity;
            saleBook.IsHidden = dto.IsHidden;

            // ✅ Xử lý ảnh nếu có upload mới
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "Images", "SaleBooks");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(saleBook.ImageUrl))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, saleBook.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldPath))
                    {
                        File.Delete(oldPath);
                    }
                }

                // Lưu ảnh mới
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.ImageFile.FileName)}";
                var newPath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                // Cập nhật đường dẫn mới vào DB
                saleBook.ImageUrl = $"/Images/SaleBooks/{uniqueFileName}";
            }

            // ✅ Cập nhật khuyến mãi
            if (!string.IsNullOrWhiteSpace(dto.PromotionId))
            {
                var promotion = await _context.Promotions.FindAsync(dto.PromotionId);
                if (promotion != null && promotion.StartDate <= DateTime.UtcNow && promotion.EndDate >= DateTime.UtcNow)
                {
                    saleBook.PromotionId = dto.PromotionId;
                    saleBook.FinalPrice = saleBook.Price * (1 - ((decimal)promotion.DiscountPercentage / 100));
                }
                else
                {
                    saleBook.PromotionId = null;
                    saleBook.FinalPrice = saleBook.Price;
                }
            }
            else
            {
                saleBook.PromotionId = null;
                saleBook.FinalPrice = saleBook.Price;
            }

            // ✅ Update Author & Category
            saleBook.AuthorSaleBooks.Clear();
            saleBook.AuthorSaleBooks = dto.AuthorIds
                .Select(authorId => new AuthorSaleBook { AuthorId = authorId, SaleBookId = id })
                .ToList();

            saleBook.CategorySaleBooks.Clear();
            saleBook.CategorySaleBooks = dto.CategoryIds
                .Select(catId => new CategorySaleBook { CategoryId = catId, SaleBookId = id })
                .ToList();

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
        public async Task<bool> CheckTitleExistsAsync(string title, string? excludeId = null)
        {
            var query = _context.SaleBooks.Where(b => b.Title.ToLower() == title.Trim().ToLower());
            if (!string.IsNullOrEmpty(excludeId))
                query = query.Where(b => b.SaleBookId != excludeId);
            return await query.AnyAsync();
        }
    }
}
