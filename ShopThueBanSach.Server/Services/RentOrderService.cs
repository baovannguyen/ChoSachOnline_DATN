using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.CartModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Text.Json;

namespace ShopThueBanSach.Server.Services
{
    public class RentOrderService : IRentOrderService
    {
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMoMoPaymentService _moMoPaymentService;

        public RentOrderService(AppDBContext context, IHttpContextAccessor httpContextAccessor, IMoMoPaymentService momo)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _moMoPaymentService = momo;
        }

        public async Task<IActionResult> CreateRentOrderWithCashAsync(RentOrderRequest request)
        {
            var result = await BuildOrderFromSessionCartAsync(request);
            if (result is IActionResult error) return error;

            var orderData = (RentOrderResult)result;
            await SaveOrderAsync(orderData);

            return new OkObjectResult(new
            {
                Message = "Tạo đơn hàng thành công",
                OrderId = orderData.Order.OrderId,
                TotalAmount = orderData.Order.TotalFee
            });
        }

        public async Task<IActionResult> PrepareMoMoOrderAsync(RentOrderRequest request)
        {
            var result = await BuildOrderFromSessionCartAsync(request);
            if (result is IActionResult error) return error;

            var orderData = (RentOrderResult)result;

            var extraData = JsonSerializer.Serialize(request);

            string payUrl = await _moMoPaymentService.CreatePaymentUrlAsync(
                orderId: orderData.Order.OrderId,
                amount: orderData.Order.TotalFee,
                returnUrl: "https://localhost:7003/momo-return",
                notifyUrl: "https://localhost:7003/api/momoorder/callback",
                extraData: extraData
            );

            return new OkObjectResult(new
            {
                Message = "Chuyển đến thanh toán MoMo",
                PaymentUrl = payUrl
            });
        }

        public async Task CreateRentOrderAfterMoMoAsync(RentOrderRequest request)
        {
            var result = await BuildOrderFromSessionCartAsync(request);
            if (result is not RentOrderResult orderData) return;

            await SaveOrderAsync(orderData);
        }

        private async Task<object> BuildOrderFromSessionCartAsync(RentOrderRequest request)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return new BadRequestObjectResult("No HttpContext");

            var session = context.Session;
            var json = session.GetString("RentalCart");
            if (string.IsNullOrEmpty(json)) return new BadRequestObjectResult("Giỏ hàng trống");

            var cart = JsonSerializer.Deserialize<List<CartItemRent>>(json);
            if (cart == null || !cart.Any()) return new BadRequestObjectResult("Không có sản phẩm trong giỏ");

            int rentalDays = (request.EndDate - request.StartDate).Days;
            if (rentalDays <= 0) return new BadRequestObjectResult("Ngày thuê không hợp lệ");

            const decimal baseRentalFee = 20000;
            const int baseRentalDays = 45;
            const decimal extraFeePerDay = 3000;
            const decimal shippingFee = 20000;

            decimal totalFee = 0;
            decimal totalDeposit = 0;
            var orderId = Guid.NewGuid().ToString();

            var orderDetails = new List<RentOrderDetail>();

            foreach (var item in cart)
            {
                var rentBookItem = await _context.RentBookItems
                    .Include(r => r.RentBook)
                    .FirstOrDefaultAsync(x => x.RentBookItemId == item.RentBookItemId);

                if (rentBookItem == null || rentBookItem.RentBook == null)
                    continue;

                if (rentBookItem.Status != RentBookItemStatus.Available)
                    continue;

                decimal rentalFee = baseRentalFee;
                if (rentalDays > baseRentalDays)
                    rentalFee += (rentalDays - baseRentalDays) * extraFeePerDay;

                totalFee += rentalFee;
                totalDeposit += rentBookItem.RentBook.Price;

                orderDetails.Add(new RentOrderDetail
                {
                    OrderId = orderId,
                    RentBookItemId = rentBookItem.RentBookItemId,
                    BookTitle = rentBookItem.RentBook.Title,
                    BookPrice = rentBookItem.RentBook.Price,
                    RentalFee = rentalFee,
                    TotalFee = rentalFee,
                    Condition = rentBookItem.Condition
                });
            }

            if (!orderDetails.Any())
                return new BadRequestObjectResult("Không có sách hợp lệ trong giỏ hàng");

            if (request.HasShippingFee)
            {
                if (string.IsNullOrWhiteSpace(request.Address) || string.IsNullOrWhiteSpace(request.Phone))
                    return new BadRequestObjectResult("Vui lòng nhập địa chỉ và số điện thoại");

                totalFee += shippingFee;
            }

            var order = new RentOrder
            {
                OrderId = orderId,
                UserId = request.UserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RentalDays = rentalDays,
                HasShippingFee = request.HasShippingFee,
                ShippingFee = request.HasShippingFee ? shippingFee : 0,
                TotalDeposit = totalDeposit,
                TotalFee = totalFee + totalDeposit,
                OrderDate = DateTime.Now,
                Status = "Pending"
            };

            return new RentOrderResult
            {
                Order = order,
                Details = orderDetails
            };
        }

        private async Task SaveOrderAsync(RentOrderResult data)
        {
            await _context.RentOrders.AddAsync(data.Order);
            await _context.RentOrderDetails.AddRangeAsync(data.Details);

            // Đánh dấu RentBookItem đã được thuê
            foreach (var detail in data.Details)
            {
                var item = await _context.RentBookItems.FindAsync(detail.RentBookItemId);
                if (item != null)
                    item.Status = RentBookItemStatus.Rented;
            }

            await _context.SaveChangesAsync();
            _httpContextAccessor.HttpContext?.Session.Remove("RentalCart");
        }

        private class RentOrderResult
        {
            public RentOrder Order { get; set; }
            public List<RentOrderDetail> Details { get; set; }
        }
    }
}
