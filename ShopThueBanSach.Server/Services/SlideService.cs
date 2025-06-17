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

        public SlideService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<Slide>> GetAllAsync()
        {
            return await _context.Slides.ToListAsync();
        }

        public async Task<Slide?> GetByIdAsync(int id)
        {
            return await _context.Slides.FindAsync(id);
        }

        public async Task<Slide> CreateAsync(SlideDto dto)
        {
            var slide = new Slide
            {
                ImageUrl = dto.ImageUrl,
                LinkUrl = dto.LinkUrl
            };
            _context.Slides.Add(slide);
            await _context.SaveChangesAsync();
            return slide;
        }

        public async Task<Slide?> UpdateAsync(int id, SlideDto dto)
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null) return null;

            slide.ImageUrl = dto.ImageUrl;
            slide.LinkUrl = dto.LinkUrl;

            await _context.SaveChangesAsync();
            return slide;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null) return false;

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
