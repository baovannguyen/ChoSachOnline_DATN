using ShopThueBanSach.Server.Models;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileAsync(string userId);
        Task<string?> UpdateProfileAsync(UpdateProfileDto model);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}
