using ShopThueBanSach.Server.Area.Admin.Entities;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.AuthModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopThueBanSach.Server.Area.Admin.Service.Interface
{
    public interface IStaffService
    {
        // Lấy toàn bộ nhân viên có Role = "Staff"
        Task<IEnumerable<Staff>> GetAllAsync();

        // Lấy chi tiết một nhân viên theo StaffId
        Task<Staff> GetByIdAsync(string id);

        // Thêm mới một nhân viên
        Task<Staff> AddAsync(Staff staff);

        // Cập nhật thông tin nhân viên
        Task<Staff> UpdateAsync(Staff staff);

        // Xóa nhân viên
        Task<bool> DeleteAsync(string id);

        // Kiểm tra sự tồn tại của StaffId
        Task<bool> ExistsAsync(string staffId);

        // Lấy StaffId thông qua email (trả về null nếu không tìm thấy)

        Task<bool> DeleteByIdAsync(string id);

        Task<string?> GetStaffIdByIdAsync(string userId);

        Task<string?> GetStaffIdByEmailAsync(string email);
        Task<bool> DeleteByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllStaffUsersAsync();
    }
}
