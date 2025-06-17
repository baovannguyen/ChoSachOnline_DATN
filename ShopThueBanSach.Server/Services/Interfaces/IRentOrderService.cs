using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models;
using ShopThueBanSach.Server.Models.CartModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IRentOrderService
    {
        Task<IActionResult> CreateRentOrderWithCashAsync(RentOrderRequest request);
        Task<IActionResult> PrepareMoMoOrderAsync(RentOrderRequest request);
        Task CreateRentOrderAfterMoMoAsync(RentOrderRequest request);

    }
}