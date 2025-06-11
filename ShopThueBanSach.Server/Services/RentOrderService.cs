using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models;
using ShopThueBanSach.Server.Models.CartModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace ShopThueBanSach.Server.Services
{
    public class RentOrderService : IRentOrderService
    {
        private readonly IMoMoPaymentService _moMoPaymentService;
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RentOrderService(AppDBContext context, IHttpContextAccessor httpContextAccessor, IMoMoPaymentService moMoPaymentService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _moMoPaymentService = moMoPaymentService;
        }

        public async Task<IActionResult> CreateRentOrderAsync(RentOrderRequest request)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return new BadRequestObjectResult("No HttpContext");

            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return new UnauthorizedResult();

            var session = httpContext.Session;
            var cartJson = session.GetString("RentalCart");
            if (string.IsNullOrEmpty(cartJson)) return new BadRequestObjectResult("Giỏ hàng trống");

            var sessionCart = JsonSerializer.Deserialize<List<CartItemRent>>(cartJson);
            if (sessionCart == null || !sessionCart.Any()) return new BadRequestObjectResult("Không có sản phẩm nào trong giỏ");

            // Tính số ngày thuê
            int rentalDays = (request.EndDate - request.StartDate).Days;
            if (rentalDays <= 0) return new BadRequestObjectResult("Ngày thuê không hợp lệ");

            const decimal baseRentalFee = 20000;
            const int baseRentalDays = 45;
            const decimal extraFeePerDay = 3000;
            const decimal shippingFlatFee = 20000;

            decimal totalFee = 0;
            decimal totalDeposit = 0;
            decimal shippingFee = 0;

            var bookIds = sessionCart.Select(i => i.BookId).ToList();
            var books = await _context.RentBooks
                .Where(b => bookIds.Contains(b.RentBookId))
                .ToListAsync();

            var items = new List<CartItemRent>();
            var orderDetails = new List<RentOrderDetail>();

            foreach (var cartItem in sessionCart)
            {
                var book = books.FirstOrDefault(b => b.RentBookId == cartItem.BookId);
                if (book == null) continue;

                decimal rentalFee = baseRentalFee;
                if (rentalDays > baseRentalDays)
                {
                    int extraDays = rentalDays - baseRentalDays;
                    rentalFee += extraDays * extraFeePerDay;
                }

                totalFee += rentalFee;
                totalDeposit += book.Price;

                var cartItemRent = new CartItemRent
                {
                    BookId = book.RentBookId,
                    BookTitle = book.Title,
                    BookPrice = book.Price,
                    RentalFee = rentalFee,
                    Quantity = 1,
                    IsSelected = true,
                    TotalFee = rentalFee
                };
                items.Add(cartItemRent);

                orderDetails.Add(new RentOrderDetail
                {
                    BookId = book.RentBookId,
                    BookTitle = book.Title,
                    Quantity = 1,
                    BookPrice = book.Price,
                    RentalFee = rentalFee,
                    TotalFee = rentalFee
                });
            }

            if (request.HasShippingFee)
            {
                if (string.IsNullOrWhiteSpace(request.Address) || string.IsNullOrWhiteSpace(request.Phone))
                    return new BadRequestObjectResult("Địa chỉ và số điện thoại là bắt buộc khi chọn vận chuyển.");

                shippingFee = shippingFlatFee;
                totalFee += shippingFee;
            }

            var orderId = Guid.NewGuid().ToString();

            var rentOrder = new RentOrder
            {
                OrderId = orderId,
                UserId = userId,
                Items = items,
                StartDate = request.StartDate = DateTime.Now,
                EndDate = request.EndDate,
                RentalDays = rentalDays,
                HasShippingFee = request.HasShippingFee,
                ShippingFee = shippingFee,
                TotalFee = totalFee + totalDeposit,
                TotalDeposit = totalDeposit,
                OrderDate = DateTime.Now,
                Status = "Pending"
            };

            await _context.RentOrders.AddAsync(rentOrder);

            foreach (var detail in orderDetails)
            {
                detail.OrderId = orderId;
                _context.RentOrderDetails.Add(detail);
            }

            await _context.SaveChangesAsync();
            session.Remove("RentalCart");

            if (request.PaymentMethod?.ToLower() == "momo")
            {
                string returnUrl = "https://webhook.site/return-url";
                string notifyUrl = "https://webhook.site/return-url";

                string momoUrl = await _moMoPaymentService.CreatePaymentUrlAsync(orderId, rentOrder.TotalFee, returnUrl, notifyUrl);

                return new OkObjectResult(new
                {
                    Message = "Chuyển sang thanh toán MoMo",
                    PaymentUrl = momoUrl,
                    OrderId = orderId
                });
            }

            // Trường hợp không chọn momo
            return new OkObjectResult(new
            {
                Message = "Tạo đơn hàng thành công",
                OrderId = orderId,
                TotalAmount = rentOrder.TotalFee
            });
        }


    }
}
