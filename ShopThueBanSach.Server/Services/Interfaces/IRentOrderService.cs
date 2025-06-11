using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models;
using ShopThueBanSach.Server.Models.CartModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface IRentOrderService
    {
        Task<IActionResult> CreateRentOrderAsync(RentOrderRequest request);

    }
}