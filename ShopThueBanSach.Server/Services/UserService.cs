using Microsoft.AspNetCore.Identity;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.UserModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Services
{
    public class UserService(UserManager<User> userManager, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IWebHostEnvironment _env = env;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<UserProfileDto?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                PhonNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Points = user.Points,
                ImageUser = user.ImageUser,
            };
        }

        public async Task<string> UpdateProfileAsync(UpdateProfileDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return "Không xác định được người dùng.";

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "Người dùng không tồn tại.";

            // Cập nhật thông tin cho phép
            user.UserName = dto.UserName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.DateOfBirth = dto.DateOfBirth;

            if (dto.ImageUser != null && dto.ImageUser.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "ImageUsers");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // XÓA ảnh cũ nếu có
                if (!string.IsNullOrEmpty(user.ImageUser))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, user.ImageUser.TrimStart('/'));
                    if (File.Exists(oldFilePath))
                        File.Delete(oldFilePath);
                }

                // Lưu ảnh mới
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.ImageUser.FileName)}";
                var newFilePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await dto.ImageUser.CopyToAsync(stream);
                }

                // Gán đường dẫn ảnh mới
                user.ImageUser = $"/ImageUsers/{uniqueFileName}";
            }

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? "Cập nhật hồ sơ thành công."
                : $"Lỗi: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }
    }
}
