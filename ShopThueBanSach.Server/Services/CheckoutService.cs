//using ShopThueBanSach.Server.Entities;
//using ShopThueBanSach.Server.Models.CartModel;
//using ShopThueBanSach.Server.Models;
//using ShopThueBanSach.Server.Services.Interfaces;
//using ShopThueBanSach.Server.Data;
//using Microsoft.EntityFrameworkCore;

//namespace ShopThueBanSach.Server.Services
//{
//    public class CheckoutService : ICheckoutService
//    {
//        private readonly IHttpContextAccessor _accessor;
//        private readonly AppDBContext _context;

//        public CheckoutService(IHttpContextAccessor accessor, AppDBContext context)
//        {
//            _accessor = accessor;
//            _context = context;
//        }

//        public async Task<OrderResultDto> CheckoutAsync(string userId, CheckoutRequestDto request)
//        {
//            if (request.IsDelivery && (string.IsNullOrWhiteSpace(request.Address) || string.IsNullOrWhiteSpace(request.Phone)))
//                throw new ArgumentException("Cần nhập địa chỉ và số điện thoại nếu chọn giao hàng.");

//            var session = _accessor.HttpContext.Session;
//            var cart = session.GetObject<List<CartItemRent>>("cart") ?? new List<CartItemRent>();

//            var selectedItems = cart.Where(x => x.IsSelected).ToList();
//            if (!selectedItems.Any()) throw new Exception("Không có sản phẩm nào được chọn để thuê.");

//            var bookIds = selectedItems.Select(x => x.BookId).ToList();
//            var books = await _context.RentBooks.Where(b => bookIds.Contains(b.RentBookId)).ToListAsync();

//            int rentalDays = (request.EndDate - request.StartDate).Days;
//            if (rentalDays <= 0) throw new Exception("Ngày thuê không hợp lệ.");

//            decimal totalFee = 0;
//            decimal deposit = 0;

//            foreach (var item in selectedItems)
//            {
//                var book = books.FirstOrDefault(b => b.RentBookId == item.BookId);
//                if (book == null) continue;

//                decimal fee = CalculateRentalFee(book.Price, rentalDays);
//                totalFee += fee * item.Quantity;
//                deposit += book.Price * item.Quantity;
//            }

//            decimal shippingFee = request.IsDelivery ? 15000 : 0; // cố định phí giao hàng nếu giao

//            var order = new RentOrder
//            {
//                UserId = userId,
//                Items = selectedItems,
//                StartDate = request.StartDate,
//                EndDate = request.EndDate,
//                RentalDays = rentalDays,
//                HasShippingFee = request.IsDelivery,
//                ShippingFee = shippingFee,
//                TotalFee = totalFee,
//                TotalDeposit = deposit,
//                Status = "Pending"
//            };

//            await _context.RentOrders.AddAsync(order);
//            // Tạo bản ghi thanh toán
//var payment = new Payment
//{
//    OrderId = order.OrderId,
//    Method = request.PaymentMethod.ToLower(), // cash | bank
//    Status = request.PaymentMethod.ToLower() == "cash" ? "Pending" : "Paid", // giả sử bank là auto-paid
//    PaidAt = request.PaymentMethod.ToLower() == "bank" ? DateTime.UtcNow : null
//};

//await _context.Payments.AddAsync(payment);
//            await _context.SaveChangesAsync();

//            // Cập nhật session (xoá sản phẩm đã thuê)
//            cart.RemoveAll(x => x.IsSelected);
//            session.SetObject("cart", cart);

//            return new OrderResultDto
//            {
//                OrderId = order.OrderId,
//                TotalAmount = totalFee + deposit + shippingFee,
//                Message = "Đơn hàng được tạo thành công."
//            };
//        }

//        private decimal CalculateRentalFee(decimal bookPrice, int rentalDays)
//        {
//            decimal baseFee = 20000m;
//            decimal extraFeePerDay = 3000m;

//            decimal fee = bookPrice + baseFee;
//            if (rentalDays > 45)
//            {
//                fee += (rentalDays - 45) * extraFeePerDay;
//            }

//            return fee;
//        }
//    }

//}
