using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.SlideModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Services
{
    public class SlideService : ISlideService
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideService(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<Slide>> GetAllAsync()
        {
            return await _context.Slides.ToListAsync();
        }

        public async Task<Slide?> GetByIdAsync(string id)
        {
            return await _context.Slides.FindAsync(id);
        }

        public async Task<Slide> CreateAsync(SlideDto dto)
        {
            string imageUrl = await SaveImageAsync(dto.ImageFile);

            var slide = new Slide
            {
                ImageUrl = imageUrl,
                LinkUrl = dto.LinkUrl
            };

            _context.Slides.Add(slide);
            await _context.SaveChangesAsync();
            return slide;
        }

        public async Task<Slide?> UpdateAsync(string id, SlideDto dto)
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null) return null;

            // Xoá ảnh cũ
            if (!string.IsNullOrEmpty(slide.ImageUrl))
            {
                var oldPath = Path.Combine(_env.WebRootPath, slide.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(oldPath)) File.Delete(oldPath);
            }

            // Lưu ảnh mới
            slide.ImageUrl = await SaveImageAsync(dto.ImageFile);
            slide.LinkUrl = dto.LinkUrl;

            await _context.SaveChangesAsync();
            return slide;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null) return false;

            if (!string.IsNullOrEmpty(slide.ImageUrl))
            {
                var path = Path.Combine(_env.WebRootPath, slide.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(path)) File.Delete(path);
            }

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "Images", "Slides");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/Images/Slides/{uniqueFileName}";
        }
    }
}
