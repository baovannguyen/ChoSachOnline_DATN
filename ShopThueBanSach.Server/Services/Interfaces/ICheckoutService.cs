using ShopThueBanSach.Server.Models.CartModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<OrderResultDto> CheckoutAsync(string userId, CheckoutRequestDto request);
    }
}
