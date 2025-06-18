using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.RentOrderModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IRentOrderService
    {
        Task<IActionResult> CreateRentOrderWithCashAsync(RentOrderRequest request);
        Task<IActionResult> PrepareMoMoOrderAsync(RentOrderRequest request);
        Task CreateRentOrderAfterMoMoAsync(RentOrderRequest request);
    }
}
