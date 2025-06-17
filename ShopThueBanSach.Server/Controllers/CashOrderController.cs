using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.CartModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashOrderController : ControllerBase
    {
        private readonly IRentOrderService _rentOrderService;

        public CashOrderController(IRentOrderService rentOrderService)
        {
            _rentOrderService = rentOrderService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCashOrder([FromBody] RentOrderRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            request.UserId = userId;
            return await _rentOrderService.CreateRentOrderWithCashAsync(request);
        }
    }
}
