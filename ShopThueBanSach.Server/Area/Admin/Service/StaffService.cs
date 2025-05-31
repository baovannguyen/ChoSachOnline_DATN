using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Area.Admin.Entities;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;

namespace ShopThueBanSach.Server.Area.Admin.Service
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
            staff.Role = "Staff"; // ensure
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

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null || staff.Role != "Staff") return false;

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
