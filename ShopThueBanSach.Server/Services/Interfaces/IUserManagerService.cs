using ShopThueBanSach.Server.Models.AuthModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IUserManagerService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(string id);
        Task<bool> UpdateAsync(UpdateUserDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
