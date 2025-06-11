//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using ShopThueBanSach.Server.Models.PaymentMethod;
//using ShopThueBanSach.Server.Services.Interfaces;

//namespace ShopThueBanSach.Server.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PaymentController : ControllerBase
//    {
//        private readonly IMoMoPaymentService _momoService;

//        public PaymentController(IMoMoPaymentService momoService)
//        {
//            _momoService = momoService;
//        }

//        [HttpPost("create")]
//        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
//        {
//            string payUrl = await _momoService.CreatePaymentUrlAsync(request.OrderId, request.Amount, request.OrderInfo);
//            return Ok(new { PayUrl = payUrl });
//        }
//    }
//}
