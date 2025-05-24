using ShopThueBanSach.Server.Entities;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface ISellBookService
    {
        Task<List<SellBook>> GetAllAsync();
        Task<SellBook?> GetByIdAsync(int id);
        Task<SellBook> CreateAsync(SellBook sachBan);
        Task<bool> UpdateAsync(SellBook sachBan);
        Task<bool> DeleteAsync(int id);
    }

}
