using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.SaleModel.SaleOrderModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrdersController : ControllerBase
    {
        private readonly ISaleOrderService _saleOrderService;

        public SaleOrdersController(ISaleOrderService saleOrderService)
        {
            _saleOrderService = saleOrderService;
        }

		[Authorize]
		[HttpPost("create-cash")]
		public async Task<IActionResult> CreateOrderCash([FromBody] SaleOrderRequest request)
		{
			var user = User;

			var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
					  ?? user.FindFirstValue("sub");

			var userName = user.FindFirstValue(ClaimTypes.Name)
						 ?? user.FindFirstValue("name")
						 ?? user.FindFirstValue("unique_name");

			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
				return Unauthorized("Không xác định được thông tin người dùng.");

			// Gán thông tin người dùng vào request trước khi gọi service
			request.UserId = userId;
			request.UserName = userName;

			if (request.SelectedProductIds == null || !request.SelectedProductIds.Any())
				return BadRequest("Vui lòng chọn ít nhất một sản phẩm để thanh toán.");

			try
			{
				return await _saleOrderService.CreateSaleOrderWithCashAsync(request);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "Lỗi khi tạo đơn hàng thanh toán tiền mặt",
					detail = ex.Message
				});
			}
		}


		[Authorize]
        [HttpPost("create-momo")]
        public async Task<IActionResult> CreateOrderMoMo([FromBody] SaleOrderRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            request.UserId = userId;

            if (request.SelectedProductIds == null || !request.SelectedProductIds.Any())
                return BadRequest("Vui lòng chọn ít nhất một sản phẩm để thanh toán.");

            try
            {
                return await _saleOrderService.PrepareMoMoSaleOrderAsync(request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo đơn hàng Momo", detail = ex.Message });
            }
        }


        [HttpPost("momo/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> MoMoCallback([FromBody] SaleOrderRequest request)
        {
            await _saleOrderService.CreateSaleOrderAfterMoMoAsync(request);
            return Ok(new { message = "Xử lý đơn hàng MoMo thành công" });
        }
    }
}

