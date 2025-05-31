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

        public async Task<List<SaleBookDto>> GetAllAsync()
        {
            return await _context.SaleBooks
                .Include(sb => sb.AuthorSaleBooks)
                .Include(sb => sb.CategorySaleBooks)
                .Select(sb => new SaleBookDto
                {
                    SaleBookId = sb.SaleBookId,
                    Title = sb.Title,
                    Description = sb.Description,
                    Publisher = sb.Publisher,
                    Translator = sb.Translator,
                    PackagingSize = sb.Size,
                    PageCount = sb.Pages,
                    Price = sb.Price,
                    Quantity = sb.Quantity,
                    ImageUrl = sb.ImageUrl,
                    IsHidden = sb.IsHidden,
                    AuthorIds = sb.AuthorSaleBooks.Select(a => a.AuthorId).ToList(),
                    CategoryIds = sb.CategorySaleBooks.Select(c => c.CategoryId).ToList()
                })
                .ToListAsync();
        }

        public async Task<SaleBookDto?> GetByIdAsync(string id)
        {
            var sb = await _context.SaleBooks
                .Include(s => s.AuthorSaleBooks)
                .Include(s => s.CategorySaleBooks)
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
                Quantity = sb.Quantity,
                ImageUrl = sb.ImageUrl,
                IsHidden = sb.IsHidden,
                AuthorIds = sb.AuthorSaleBooks.Select(a => a.AuthorId).ToList(),
                CategoryIds = sb.CategorySaleBooks.Select(c => c.CategoryId).ToList()
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
                CategorySaleBooks = dto.CategoryIds.Select(cid => new CategorySaleBook { CategoryId = cid }).ToList(),
            };

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

            // Cập nhật thông tin cơ bản
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

            // Cập nhật quan hệ nhiều-nhiều Author
            saleBook.AuthorSaleBooks.Clear();
            saleBook.AuthorSaleBooks = dto.AuthorIds.Select(aid => new AuthorSaleBook { AuthorId = aid, SaleBookId = id }).ToList();

            // Cập nhật quan hệ nhiều-nhiều Category
            saleBook.CategorySaleBooks.Clear();
            saleBook.CategorySaleBooks = dto.CategoryIds.Select(cid => new CategorySaleBook { CategoryId = cid, SaleBookId = id }).ToList();

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
