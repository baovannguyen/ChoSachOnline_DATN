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
                throw new Exception("Thiếu StaffId. Cần gán bằng User.Id để đồng bộ.");

            var existingUser = await _userManager.FindByIdAsync(staff.StaffId);
            if (existingUser == null)
            {
                var user = new User
                {
                    Id = staff.StaffId,
                    UserName = staff.FullName,
                    Email = $"{Guid.NewGuid()}@placeholder.local", // email giả nếu không có
                    Address = staff.Address,
                    DateOfBirth = staff.DateOfBirth ?? DateTime.Now,
                    EmailConfirmed = true,
                    Role = "Staff"
                };

                var result = await _userManager.CreateAsync(user, staff.Password);
                if (!result.Succeeded)
                {
                    throw new Exception("Tạo tài khoản Staff thất bại: " +
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                if (existingUser.Role != "Staff")
                {
                    existingUser.Role = "Staff";
                    await _userManager.UpdateAsync(existingUser);
                }
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
            existing.Password = staff.Password;
            existing.PhoneNumber = staff.PhoneNumber;
            existing.Address = staff.Address;
            existing.DateOfBirth = staff.DateOfBirth;

            var user = await _userManager.FindByIdAsync(existing.StaffId);
            if (user != null && user.Role == "Staff")
            {
                user.UserName = existing.FullName;
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

            var user = await _userManager.FindByIdAsync(staff.StaffId);
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

        public async Task<string?> GetStaffIdByIdAsync(string userId)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == userId);
            return staff?.StaffId;
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == id);
            if (staff == null) return false;

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllStaffUsersAsync()
        {
            var users = await _userManager.Users
                                          .Where(u => u.Role == "Staff")
                                          .ToListAsync();

            return users.Select(MapToDto);
        }

        private static UserDto MapToDto(User u) => new()
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            Role = u.Role,
            PhoneNumber = u.PhoneNumber,
            Address = u.Address,
            DateOfBirth = u.DateOfBirth,
            Points = u.Points,
            ImageUser = u.ImageUser
        };
        public async Task<string?> GetStaffIdByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.StaffId == user.Id);
            return staff?.StaffId;
        }
        public async Task<bool> DeleteByEmailAsync(string email)
        {
            var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Email == email && s.Role == "Staff");
            if (staff == null) return false;

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
