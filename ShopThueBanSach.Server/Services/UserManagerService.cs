using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Area.Admin.Entities;
using ShopThueBanSach.Server.Area.Admin.Service;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.AuthModel;
using ShopThueBanSach.Server.Models.UserModel;
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

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userManager.Users
                .Where(u => u.Role == null || u.Role == "Khách hàng")
                .ToListAsync();

            return users.Select(u => MapToDto(u));
        }

        public async Task<UserDto?> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || !(user.Role == null || user.Role == "Khách hàng"))
                return null;

            return MapToDto(user);
        }

        public async Task<bool> UpdateAsync(UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null || !(user.Role == null || user.Role == "Khách hàng" || user.Role == "Staff"))
                return false;

            var oldRole = user.Role;

            // ✅ KHÔNG cập nhật Email — tránh lỗi và không cho sửa
            // ✅ Đảm bảo Email không bị null (đề phòng người dùng cũ bị thiếu email)
            if (string.IsNullOrEmpty(user.Email))
            {
                var existing = await _userManager.FindByIdAsync(dto.Id);
                if (!string.IsNullOrEmpty(existing?.Email))
                    user.Email = existing.Email;
                else
                    user.Email = $"{Guid.NewGuid()}@placeholder.local"; // fallback nếu dữ liệu sai
            }

            // Cập nhật các thông tin cho phép
            if (dto.Address != null) user.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
            if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth.Value;
            if (dto.Points.HasValue) user.Points = dto.Points.Value;
            if (dto.ImageUser != null) user.ImageUser = dto.ImageUser;
            if (dto.Role != null) user.Role = dto.Role;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            // Nếu đổi sang Staff → tạo staff mới
            if (oldRole != "Staff" && user.Role == "Staff")
            {
                var existingStaff = await _staffService.GetByIdAsync(user.Id);
                if (existingStaff == null)
                {
                    var staff = new Staff
                    {
                        StaffId = user.Id,
                        FullName = user.UserName,
                        Address = user.Address,
                        DateOfBirth = user.DateOfBirth,
                        PhoneNumber = user.PhoneNumber,
                        Role = "Staff",
                        Password = "Default@123",
                        Email = user.Email
                    };

                    await _staffService.AddAsync(staff);
                }
            }
            // Nếu chuyển từ Staff sang khác → xóa staff
            else if (oldRole == "Staff" && user.Role != "Staff")
            {
                if (_staffService is StaffService concreteStaffService)
                {
                    await concreteStaffService.DeleteByIdAsync(user.Id);
                }
            }

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !(user.Role == null || user.Role == "Khách hàng"))
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        private static UserDto MapToDto(User u) => new()
        {
            Id = u.Id,
            UserName = u.UserName,
            Role = u.Role,
            Address = u.Address,
            PhoneNumber = u.PhoneNumber,
            DateOfBirth = u.DateOfBirth,
            Points = u.Points,
            ImageUser = u.ImageUser
        };
        public async Task<bool> UpdateCustomerAsync(UpdateCustomerDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null || !(user.Role == null || user.Role == "Khách hàng"))
                return false;

            if (dto.Address != null) user.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
            if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth.Value;
            if (dto.Points.HasValue) user.Points = dto.Points.Value;
            if (dto.ImageUser != null) user.ImageUser = dto.ImageUser;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

    }
}
