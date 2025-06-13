using ShopThueBanSach.Server.Models.BooksModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IDiscountCodeService
    {
        Task<List<DiscountCodeDTO>> GetAllAsync();
        Task<DiscountCodeDTO?> GetByIdAsync(string id);
        Task<bool> CreateAsync(DiscountCodeDTO model);
        Task<bool> UpdateAsync(string id, DiscountCodeDTO model);
        Task<bool> DeleteAsync(string id);
        Task<List<DiscountCodeDTO>> GetAvailableForExchangeAsync();

    }
}
