using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.CartModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _checkoutService.CheckoutAsync(userId, dto);
            return Ok(result);
        }
    }
}
