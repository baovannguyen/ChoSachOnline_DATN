using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.BooksModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IRentBookService
    {
        Task<List<RentBookDto>> GetAllAsync();
        Task<RentBookDto?> GetByIdAsync(string id);

        // Chỉ giữ 1 method CreateAsync với CreateRentBookDto
        Task<string> CreateAsync(CreateRentBookDto dto);

        Task<bool> UpdateAsync(string id, RentBookDto dto);
        Task<bool> DeleteAsync(string id);
        Task<bool> SetVisibilityAsync(string id, bool isHidden);
    }

}
