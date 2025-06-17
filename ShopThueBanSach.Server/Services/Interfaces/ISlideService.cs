using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.SlideModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface ISlideService
    {
        Task<List<Slide>> GetAllAsync();
        Task<Slide?> GetByIdAsync(int id);
        Task<Slide> CreateAsync(SlideDto dto);
        Task<Slide?> UpdateAsync(int id, SlideDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
