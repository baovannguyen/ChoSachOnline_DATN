using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.CartModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleCartController : ControllerBase
    {
        private readonly ISaleCartService _cartService;

        public SaleCartController(ISaleCartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsync();
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] SaleCartItem item)
        {
            await _cartService.AddToCartAsync(item);
            return Ok();
        }

        [HttpPost("decrease/{id}")]
        public async Task<IActionResult> DecreaseQuantity(string id)
        {
            await _cartService.DecreaseQuantityAsync(id);
            return Ok();
        }

        [HttpPost("increase/{id}")]
        public async Task<IActionResult> IncreaseQuantity(string id)
        {
            await _cartService.IncreaseQuantityAsync(id);
            return Ok();
        }

        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveItem(string id)
        {
            await _cartService.RemoveFromCartAsync(id);
            return Ok();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync();
            return Ok();
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotal()
        {
            var total = await _cartService.GetTotalAsync();
            return Ok(total);
        }
    }
}
