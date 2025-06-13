using ShopThueBanSach.Server.Models.BooksModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<List<PromotionDTO>> GetAllPromotionsAsync();
        Task<PromotionDTO?> GetPromotionByIdAsync(string id);
        Task<bool> CreatePromotionAsync(PromotionDTO model);
        Task<bool> UpdatePromotionAsync(string id, PromotionDTO model);
        Task<bool> DeletePromotionAsync(string id);
    }
}
