using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDBContext _context;

        public StaffService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _context.Staffs.ToListAsync();
        }

        public async Task<Staff> GetByIdAsync(int id)
        {
            return await _context.Staffs.FindAsync(id);
        }

        public async Task<IEnumerable<Staff>> GetByRoleAsync(string role)
        {
            return await _context.Staffs
                .Where(s => s.Role == role)
                .ToListAsync();
        }

        public async Task<Staff> CreateAsync(Staff staff)
        {
            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task<bool> UpdateAsync(Staff staff)
        {
            var existing = await _context.Staffs.FindAsync(staff.StaffId);
            if (existing == null) return false;

            existing.FullName = staff.FullName;
            existing.Role = staff.Role;
            existing.Email = staff.Email;
            existing.Password = staff.Password;
            existing.PhoneNumber = staff.PhoneNumber;
            existing.Address = staff.Address;
            existing.DateOfBirth = staff.DateOfBirth;
            existing.LoyaltyPoints = staff.LoyaltyPoints;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return false;

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
