
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Area.Admin.Entities;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.AuthModel;

namespace ShopThueBanSach.Server.Area.Admin.Service
{
    public class StaffService : IStaffService
    {
        private readonly AppDBContext _context;
        private readonly UserManager<User> _userManager;

        public StaffService(AppDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _context.Staffs
                .Where(s => s.Role == "Staff")
                .ToListAsync();
        }

        public async Task<Staff> GetByIdAsync(string id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            return staff?.Role == "Staff" ? staff : null;
        }

        public async Task<Staff> AddAsync(Staff staff)
        {
            staff.Role = "Staff";

            if (string.IsNullOrEmpty(staff.StaffId))
            {
                staff.StaffId = "STF_" + Guid.NewGuid().ToString("N")[..8];
            }

            // Tạo tài khoản người dùng
            var user = new User
            {
                UserName = staff.Email,
                Email = staff.Email,
                Address = staff.Address,
                DateOfBirth = staff.DateOfBirth ?? DateTime.Now,
                EmailConfirmed = true,
                Role = "Staff" // ❗ Quan trọng
            };

            var createResult = await _userManager.CreateAsync(user, staff.Password);
            if (!createResult.Succeeded)
            {
                throw new Exception("Tạo tài khoản Staff thất bại: " +
                    string.Join("; ", createResult.Errors.Select(e => e.Description)));
            }

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task<Staff> UpdateAsync(Staff staff)
        {
            var existing = await _context.Staffs.FindAsync(staff.StaffId);
            if (existing == null || existing.Role != "Staff") return null;

            existing.FullName = staff.FullName;
            existing.Email = staff.Email;
            existing.Password = staff.Password;
            existing.PhoneNumber = staff.PhoneNumber;
            existing.Address = staff.Address;
            existing.DateOfBirth = staff.DateOfBirth;

            var user = await _userManager.FindByEmailAsync(existing.Email);
            if (user != null && user.Role == "Staff")
            {
                user.Email = existing.Email;
                user.UserName = existing.Email;
                user.Address = existing.Address;
                user.DateOfBirth = existing.DateOfBirth ?? DateTime.Now;

                if (!string.IsNullOrWhiteSpace(existing.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, existing.Password);
                }

                await _userManager.UpdateAsync(user);
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null || staff.Role != "Staff") return false;

            var user = await _userManager.FindByEmailAsync(staff.Email);
            if (user != null && user.Role == "Staff")
            {
                await _userManager.DeleteAsync(user);
            }

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string staffId)
        {
            return await _context.Staffs.AnyAsync(s => s.StaffId == staffId && s.Role == "Staff");
        }

        public async Task<string?> GetStaffIdByEmailAsync(string email)
        {
            return await _context.Staffs
                .Where(s => s.Email == email && s.Role == "Staff")
                .Select(s => s.StaffId)
                .FirstOrDefaultAsync();
        }

        // (Optional) Nội bộ dùng để xoá staff nếu Role đổi
        public async Task<bool> DeleteByEmailAsync(string email)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == email && s.Role == "Staff");
            if (staff == null) return false;

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }
        // 🔸 Lấy tất cả User có role = "Staff"
        public async Task<IEnumerable<UserDto>> GetAllStaffUsersAsync()
        {
            var users = await _userManager.Users
                                          .Where(u => u.Role == "Staff")
                                          .ToListAsync();

            return users.Select(MapToDto);
        }

        // 👉 Helper tái sử dụng hàm map DTO của UserManagerService
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
