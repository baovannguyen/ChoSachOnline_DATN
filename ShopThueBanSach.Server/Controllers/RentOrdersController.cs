//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using ShopThueBanSach.Server.Models.CartModel;
//using ShopThueBanSach.Server.Services.Interfaces;
//using System.Security.Claims;

//namespace ShopThueBanSach.Server.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize] // Đảm bảo người dùng đã xác thực
//    public class RentOrdersController : ControllerBase
//    {
//        private readonly IRentOrderService _rentOrderService;

//        public RentOrdersController(IRentOrderService rentOrderService)
//        {
//            _rentOrderService = rentOrderService;
//        }

//        [HttpPost("create")]
//        public async Task<IActionResult> Create([FromBody] RentOrderRequest request)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (userId == null)
//                return Unauthorized();

//            request.UserId = userId;

//            try
//            {
//                return await _rentOrderService.CreateRentOrderAsync(request);
//            }
//            catch (Exception ex)
//            {
//                // Ghi log nếu cần
//                return BadRequest(new
//                {
//                    message = "Tạo đơn thuê thất bại",
//                    detail = ex.Message
//                });
//            }
//        }

//    }
//}
