using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.SaleModel.SaleOrderModel;
using ShopThueBanSach.Server.Services.Interfaces;
using ShopThueBanSach.Server.Services.Vnpay;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrdersController : ControllerBase
    {
		private readonly ISaleOrderService _saleOrderService;
		private readonly IVnPayService _vnPayService;

		public SaleOrdersController(ISaleOrderService saleOrderService, IVnPayService vnPayService)
		{
			_saleOrderService = saleOrderService;
			_vnPayService = vnPayService;
		}

		[Authorize]
        [HttpPost("create-cash")]
        public async Task<IActionResult> CreateOrderCash([FromBody] SaleOrderRequest request)
        {
			var user = User;
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            request.UserId = userId;

            if (request.SelectedProductIds == null || !request.SelectedProductIds.Any())
                return BadRequest("Vui lòng chọn ít nhất một sản phẩm để thanh toán.");

            try
            {
                return await _saleOrderService.CreateSaleOrderWithCashAsync(request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo đơn hàng thanh toán tiền mặt", detail = ex.Message });
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
		[Authorize]
		[HttpPost("create-vnpay")]
		public async Task<IActionResult> CreatePaymentVnPay([FromBody] SaleOrderRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId)) return Unauthorized();

			request.UserId = userId;

			try
			{
				var url = await _saleOrderService.PrepareVnPaySaleOrderAsync(request, HttpContext);
				return Ok(new { paymentUrl = url });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Lỗi tạo đơn hàng VNPAY", detail = ex.Message });
			}
		}



		[HttpGet("PaymentCallbackVnpay")]
		public async Task<IActionResult> PaymentCallbackVnpay()
		{
			var query = Request.Query;
			var vnpResponseCode = query["vnp_ResponseCode"].ToString();
			var vnpTxnStatus = query["vnp_TransactionStatus"].ToString();
			var txnRef = query["vnp_TxnRef"].ToString();

			if (vnpResponseCode == "00" && vnpTxnStatus == "00")
			{
				var result = await _saleOrderService.CreateSaleOrderAfterVnPayAsync(HttpContext);

				if (result is OkObjectResult okResult &&
					okResult.Value is { } value &&
					value.GetType().GetProperty("orderId") is { } prop)
				{
					var orderId = prop.GetValue(value)?.ToString();
					return Redirect($"http://localhost:5173/payment-success?orderId={orderId}");
				}

				return Redirect("http://localhost:5173/");
			}

			return Redirect("http://localhost:5173/payment-fail");
		}


	}
}

