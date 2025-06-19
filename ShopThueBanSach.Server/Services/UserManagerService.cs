using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Area.Admin.Entities;
using ShopThueBanSach.Server.Area.Admin.Service;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.AuthModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Services
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserManager<User> _userManager;
        private readonly IStaffService _staffService;
        public UserManagerService(UserManager<User> userManager, IStaffService staffService)
        {
            _userManager = userManager;
            _staffService = staffService;
        }

        // 🔸 LẤY TẤT CẢ USER CÓ ROLE NULL HOẶC "Khách hàng"
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userManager.Users
                                          .Where(u => u.Role == null || u.Role == "Khách hàng")
                                          .ToListAsync();

            return users.Select(u => MapToDto(u));
        }

        // 🔸 LẤY USER THEO ID và kiểm tra Role
        public async Task<UserDto?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !(user.Role == null || user.Role == "Khách hàng"))
                return null;

            return MapToDto(user);
        }

        // 🔸 CHỈ CẬP NHẬT NẾU USER ĐÚNG ĐỐI TƯỢNG
        public async Task<bool> UpdateAsync(UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null || !(user.Role == null || user.Role == "Khách hàng" || user.Role == "Staff"))
                return false;

            var oldRole = user.Role;

            if (dto.Email != null) user.Email = dto.Email;
            if (dto.Address != null) user.Address = dto.Address;
            if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth.Value;
            if (dto.Points.HasValue) user.Points = dto.Points.Value;
            if (dto.ImageUser != null) user.ImageUser = dto.ImageUser;
            if (dto.Role != null) user.Role = dto.Role;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            // Đồng bộ bảng Staff
            if (oldRole != "Staff" && user.Role == "Staff")
            {
                var staff = new Staff
                {
                    StaffId = "STF_" + Guid.NewGuid().ToString("N")[..8],
                    FullName = user.UserName,
                    Email = user.Email,
                    Address = user.Address,
                    DateOfBirth = user.DateOfBirth,
                    Role = "Staff",
                    Password = "Default@123" // có thể để trống hoặc random
                };

                await _staffService.AddAsync(staff);
            }
            else if (oldRole == "Staff" && user.Role != "Staff")
            {
                // Gọi bằng ép kiểu để dùng được DeleteByEmailAsync
                if (_staffService is StaffService concreteStaffService)
                {
                    await concreteStaffService.DeleteByEmailAsync(user.Email);
                }
            }

            return true;
        }
        // 🔸 XOÁ USER ĐÚNG ĐỐI TƯỢNG
        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !(user.Role == null || user.Role == "Khách hàng"))
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        // 👉 Helper: map entity → DTO
        private static UserDto MapToDto(User u) => new()
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            Role = u.Role,
            Address = u.Address,
            DateOfBirth = u.DateOfBirth,
            Points = u.Points,
            ImageUser = u.ImageUser
        };
    }
}
