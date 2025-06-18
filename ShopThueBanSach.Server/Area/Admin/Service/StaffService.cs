using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Area.Admin.Entities;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Entities;

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

        public async Task<Staff> GetByIdAsync(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            return staff?.Role == "Staff" ? staff : null;
        }

        public async Task<Staff> AddAsync(Staff staff)
        {
            staff.Role = "Staff"; // đảm bảo đúng vai trò

            var user = new User
            {
                UserName = staff.Email,
                Email = staff.Email,
                Address = staff.Address,
                DateOfBirth = staff.DateOfBirth ?? DateTime.Now,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(user, staff.Password);
            if (!createUserResult.Succeeded)
            {
                throw new Exception("Tạo tài khoản đăng nhập cho Staff thất bại: " +
                    string.Join("; ", createUserResult.Errors.Select(e => e.Description)));
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
            if (user != null)
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

        public async Task<bool> DeleteAsync(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null || staff.Role != "Staff") return false;

            var user = await _userManager.FindByEmailAsync(staff.Email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int staffId)
        {
            return await _context.Staffs.AnyAsync(s => s.StaffId == staffId);
        }

        // ✅ Thêm phương thức này để dùng khi lấy từ token
        public int? GetStaffIdByEmail(string? email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            var staff = _context.Staffs.FirstOrDefault(s => s.Email == email);
            return staff?.StaffId;
        }
    }
}
