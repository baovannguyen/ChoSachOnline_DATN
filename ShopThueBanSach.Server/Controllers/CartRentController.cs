using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.CartModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartRentController : ControllerBase
    {
        private readonly ICartRentService _cartService;

        public CartRentController(ICartRentService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            return Ok(_cartService.GetCart());
        }

        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] AddToCartDto dto)
        {
            _cartService.AddToCart(dto.BookId);
            return Ok(_cartService.GetCart());
        }

        [HttpPut("update-quantity")]
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityDto dto)
        {
            _cartService.UpdateQuantity(dto.BookId, dto.Quantity);
            return Ok(_cartService.GetCart());
        }

        [HttpDelete("remove/{bookId}")]
        public IActionResult Remove(string bookId)
        {
            _cartService.RemoveFromCart(bookId);
            return Ok(_cartService.GetCart());
        }

        [HttpDelete("clear")]
        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return Ok();
        }
    }
}
