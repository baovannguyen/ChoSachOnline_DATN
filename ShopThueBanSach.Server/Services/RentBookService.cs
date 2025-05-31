using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities.Relationships;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace ShopThueBanSach.Server.Services
{
    public class RentBookService : IRentBookService
    {
        private readonly AppDBContext _context;

        public RentBookService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<RentBookDto>> GetAllAsync()
        {
            return await _context.RentBooks
                .Include(r => r.AuthorRentBooks)
                .Include(r => r.CategoryRentBooks)
                .Select(r => new RentBookDto
                {
                    RentBookId = r.RentBookId,
                    Title = r.Title,
                    Description = r.Description,
                    Publisher = r.Publisher,
                    PageCount = r.Pages,
                    Translator = r.Translator,
                    PackagingSize = r.Size,
                    Price = r.Price,
                    ImageUrl = r.ImageUrl,
                    Quantity = r.Quantity,
                    PromotionId = null,
                    IsHidden = r.IsHidden,  // <-- thêm
                    AuthorIds = r.AuthorRentBooks.Select(ar => ar.AuthorId).ToList(),
                    CategoryIds = r.CategoryRentBooks.Select(cr => cr.CategoryId).ToList()
                }).ToListAsync();
        }

        public async Task<RentBookDto?> GetByIdAsync(string id)
        {
            var rentBook = await _context.RentBooks
                .Include(r => r.AuthorRentBooks)
                .Include(r => r.CategoryRentBooks)
                .FirstOrDefaultAsync(r => r.RentBookId == id);

            if (rentBook == null) return null;

            return new RentBookDto
            {
                RentBookId = rentBook.RentBookId,
                Title = rentBook.Title,
                Description = rentBook.Description,
                Publisher = rentBook.Publisher,
                PageCount = rentBook.Pages,
                Translator = rentBook.Translator,
                PackagingSize = rentBook.Size,
                Price = rentBook.Price,
                ImageUrl = rentBook.ImageUrl,
                Quantity = rentBook.Quantity,
                PromotionId = null,
                IsHidden = rentBook.IsHidden,  // <-- thêm
                AuthorIds = rentBook.AuthorRentBooks.Select(ar => ar.AuthorId).ToList(),
                CategoryIds = rentBook.CategoryRentBooks.Select(cr => cr.CategoryId).ToList()
            };
        }

        public async Task<string> CreateAsync(RentBookDto dto)
        {
            var rentBook = new RentBook
            {
                Title = dto.Title,
                Description = dto.Description,
                Publisher = dto.Publisher,
                Translator = dto.Translator,
                Size = dto.PackagingSize,
                Pages = dto.PageCount,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                Quantity = dto.Quantity,
                IsHidden = dto.IsHidden,  // <-- cập nhật giá trị từ dto
                AuthorRentBooks = dto.AuthorIds.Select(id => new AuthorRentBook
                {
                    AuthorId = id
                }).ToList(),
                CategoryRentBooks = dto.CategoryIds.Select(id => new CategoryRentBook
                {
                    CategoryId = id
                }).ToList()
            };

            _context.RentBooks.Add(rentBook);
            await _context.SaveChangesAsync();
            return rentBook.RentBookId;
        }

        public async Task<bool> UpdateAsync(string id, RentBookDto dto)
        {
            var rentBook = await _context.RentBooks
                .Include(r => r.AuthorRentBooks)
                .Include(r => r.CategoryRentBooks)
                .FirstOrDefaultAsync(r => r.RentBookId == id);

            if (rentBook == null) return false;

            rentBook.Title = dto.Title;
            rentBook.Description = dto.Description;
            rentBook.Publisher = dto.Publisher;
            rentBook.Translator = dto.Translator;
            rentBook.Size = dto.PackagingSize;
            rentBook.Pages = dto.PageCount;
            rentBook.Price = dto.Price;
            rentBook.ImageUrl = dto.ImageUrl;
            rentBook.Quantity = dto.Quantity;
            rentBook.IsHidden = dto.IsHidden;  // <-- cập nhật giá trị

            rentBook.AuthorRentBooks.Clear();
            rentBook.CategoryRentBooks.Clear();

            rentBook.AuthorRentBooks = dto.AuthorIds.Select(id => new AuthorRentBook
            {
                AuthorId = id
            }).ToList();
            rentBook.CategoryRentBooks = dto.CategoryIds.Select(id => new CategoryRentBook
            {
                CategoryId = id
            }).ToList();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var rentBook = await _context.RentBooks.FindAsync(id);
            if (rentBook == null) return false;
            _context.RentBooks.Remove(rentBook);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetVisibilityAsync(string id, bool isHidden)
        {
            var rentBook = await _context.RentBooks.FindAsync(id);
            if (rentBook == null) return false;
            rentBook.IsHidden = isHidden;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> CreateAsync(CreateRentBookDto dto)
        {
            var rentBook = new RentBook
            {
                RentBookId = Guid.NewGuid().ToString(),
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
                AuthorRentBooks = dto.AuthorIds.Select(id => new AuthorRentBook
                {
                    AuthorId = id
                }).ToList(),
                CategoryRentBooks = dto.CategoryIds.Select(id => new CategoryRentBook
                {
                    CategoryId = id
                }).ToList()
            };

            _context.RentBooks.Add(rentBook);
            await _context.SaveChangesAsync();
            return rentBook.RentBookId;
        }
    }
}
