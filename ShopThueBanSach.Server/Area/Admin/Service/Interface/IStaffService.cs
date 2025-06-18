using ShopThueBanSach.Server.Area.Admin.Entities;

namespace ShopThueBanSach.Server.Area.Admin.Service.Interface
{
    public interface IStaffService
    {
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<Staff> GetByIdAsync(int id);
        Task<Staff> AddAsync(Staff staff);
        Task<Staff> UpdateAsync(Staff staff);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int staffId);

        // 🆕 Thêm hàm này
        int? GetStaffIdByEmail(string? email);
    }


}
