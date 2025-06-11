using ShopThueBanSach.Server.Models.CartModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface ISaleCartService
    {
        Task<List<SaleCartItem>> GetCartAsync();
        Task AddToCartAsync(SaleCartItem item);
        Task IncreaseQuantityAsync(string saleBookId);
        Task RemoveFromCartAsync(string saleBookId);
        Task DecreaseQuantityAsync(string saleBookId);
        Task ClearCartAsync();
        Task<decimal> GetTotalAsync();
    }
}
