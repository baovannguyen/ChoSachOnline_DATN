using ShopThueBanSach.Server.Entities;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<Staff> GetByIdAsync(int id);
        Task<IEnumerable<Staff>> GetByRoleAsync(string role);
        Task<Staff> CreateAsync(Staff staff);
        Task<bool> UpdateAsync(Staff staff);
        Task<bool> DeleteAsync(int id);
    }

}
