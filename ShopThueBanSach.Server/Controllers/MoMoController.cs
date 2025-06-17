//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ShopThueBanSach.Server.Models.CartModel;
//using ShopThueBanSach.Server.Models.PaymentMethod;
//using System.Text.Json;
//using static ShopThueBanSach.Server.Services.Interfaces.IMoMoCallbackService;

//namespace ShopThueBanSach.Server.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MoMoController : ControllerBase
//    {
//        private readonly IMoMoCallbackService _callbackService;

//        public MoMoController(IMoMoCallbackService callbackService)
//        {
//            _callbackService = callbackService;
//        }

//        [HttpPost("callback")]
//        public async Task<IActionResult> Callback([FromBody] JsonElement payload)
//        {
//            var resultCode = payload.GetProperty("resultCode").GetInt32();
//            if (resultCode != 0)
//                return Ok(new { message = "Thanh toán thất bại. Không tạo đơn hàng." });

//            var extraData = payload.GetProperty("extraData").GetString();
//            var request = JsonSerializer.Deserialize<RentOrderRequest>(extraData);

//            // gọi service để tạo đơn
//            await _rentOrderService.CreateRentOrderAfterMoMoAsync(request);

//            return Ok(new { message = "Thanh toán thành công và đã tạo đơn hàng." });
//        }

//    }
//}
