using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.SaleModel.SaleOrderModel;
using ShopThueBanSach.Server.Models;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Models.SaleModel.CartSaleModel;
using Newtonsoft.Json;
using ShopThueBanSach.Server.Models.SaleModel.SaleOrderModel;
using ShopThueBanSach.Server.Models.Vnpay;
using ShopThueBanSach.Server.Services.Vnpay;

namespace ShopThueBanSach.Server.Services
{
    public partial class SaleOrderService : ISaleOrderService
    {
		private readonly AppDBContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMoMoPaymentService _moMoPaymentService;
		private readonly IVnPayService _vnPayService;
		private readonly ISaleCartService _cartService;

		public SaleOrderService(
			AppDBContext context,
			IHttpContextAccessor httpContextAccessor,
			IMoMoPaymentService moMoPaymentService,
			IVnPayService vnPayService,
			ISaleCartService cartService)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_moMoPaymentService = moMoPaymentService;
			_vnPayService = vnPayService;
			_cartService = cartService;
		}

		public async Task<string> PrepareVnPaySaleOrderAsync(SaleOrderRequest request, HttpContext httpContext)
		{
			// 1. Lấy giỏ hàng đã chọn
			var selectedItems = _cartService.GetSelectedItems();

			if (selectedItems == null || !selectedItems.Any())
				throw new Exception("Không có sản phẩm nào được chọn trong giỏ hàng.");

			// 2. Tính tổng tiền
			var total = selectedItems.Sum(x => x.UnitPrice * x.Quantity);

			// 3. Tạo thông tin lưu session
			var sessionModel = new PaymentSessionModel
			{
				CartItems = selectedItems.Select(x => new CartItemSale
				{
					ProductId = x.ProductId,
					ProductName = x.ProductName,
					Quantity = x.Quantity,
					UnitPrice = x.UnitPrice
				}).ToList(),
				UserId = request.UserId,
				Amount = total,
				OrderDescription = $"Thanh toán đơn hàng thuê sách lúc {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
			};

			// 4. Lưu vào Session
			httpContext.Session.SetString("VNPayOrderSession", System.Text.Json.JsonSerializer.Serialize(sessionModel));

			// 5. Tạo thông tin thanh toán cho VNPay
			var paymentInfo = new PaymentInformationModel
			{
				OrderType = "book_rental",
				Amount = (decimal)total,
				OrderDescription = sessionModel.OrderDescription,
				Name = request.UserId ?? "unknown"
			};

			// 6. Trả về URL thanh toán
			return _vnPayService.CreatePaymentUrl(paymentInfo, httpContext);
		}



		public async Task<IActionResult> CreateSaleOrderAfterVnPayAsync(HttpContext httpContext)
		{
			// 1. Lấy thông tin từ Session
			var sessionData = httpContext.Session.GetString("VNPayOrderSession");

			if (string.IsNullOrEmpty(sessionData))
				return new BadRequestObjectResult(new { success = false, message = "Session thanh toán đã hết hạn." });

			var model = System.Text.Json.JsonSerializer.Deserialize<PaymentSessionModel>(sessionData);

			if (model == null || model.CartItems == null || !model.CartItems.Any())
				return new BadRequestObjectResult("Không có sản phẩm trong đơn hàng.");

			// 2. Tạo danh sách chi tiết đơn hàng
			var details = model.CartItems.Select(item => new SaleOrderDetail
			{
				ProductId = item.ProductId,
				ProductName = item.ProductName,
				Quantity = item.Quantity,
				UnitPrice = item.UnitPrice,
				SubTotal = item.Quantity * item.UnitPrice
			}).ToList();

			// 3. Tạo đơn hàng
			var order = new SaleOrder
			{
				OrderId = Guid.NewGuid().ToString(),
				UserId = model.UserId,
				OrderDate = DateTime.UtcNow,
				TotalAmount = model.Amount,
				OriginalTotalAmount = model.Amount,
				DiscountAmount = 0,
				PaymentMethod = "VNPAY",
				Status = OrderStatus.Pending,
				HasShippingFee = false,
				ShippingFee = 0
			};

			foreach (var detail in details)
				detail.OrderId = order.OrderId;

			var result = new SaleOrderResult
			{
				Order = order,
				Details = details,
				VoucherCode = null
			};

			// 4. Lưu đơn hàng và cập nhật hệ thống
			await SaveOrderAsync(result);

			// 5. Xoá session
			httpContext.Session.Remove("VNPayOrderSession");

			return new OkObjectResult(new
			{
				success = true,
				message = "Thanh toán và tạo đơn hàng thành công",
				orderId = order.OrderId
			});
		}

		public async Task<IActionResult> CreateSaleOrderWithCashAsync(SaleOrderRequest request)
        {
            var result = await BuildOrderFromSessionCartAsync(request);
            if (result is IActionResult error) return error;

            var orderResult = (SaleOrderResult)result;
            await SaveOrderAsync(orderResult);

            return new OkObjectResult(new
            {
                Message = "Tạo đơn hàng thành công",
                orderResult.Order.OrderId,
                orderResult.Order.TotalAmount
            });
        }

        public async Task<IActionResult> PrepareMoMoSaleOrderAsync(SaleOrderRequest request)
        {
            var result = await BuildOrderFromSessionCartAsync(request);
            if (result is IActionResult error) return error;

            var orderResult = (SaleOrderResult)result;
            var extraData = System.Text.Json.JsonSerializer.Serialize(request);

            var payUrl = await _moMoPaymentService.CreatePaymentUrlAsync(
                orderId: orderResult.Order.OrderId,
                amount: orderResult.Order.TotalAmount,
                returnUrl: "https://localhost:7003/momo-return",
                notifyUrl: "https://localhost:7003/api/momoorder/callback",
                extraData: extraData
            );

            return new OkObjectResult(new
            {
                Message = "Chuyển hướng đến MoMo",
                PaymentUrl = payUrl
            });
        }

        public async Task CreateSaleOrderAfterMoMoAsync(SaleOrderRequest request)
        {
            var result = await BuildOrderFromSessionCartAsync(request);
            if (result is not SaleOrderResult orderResult) return;
            await SaveOrderAsync(orderResult);
        }

        private async Task<object> BuildOrderFromSessionCartAsync(SaleOrderRequest request)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return new BadRequestObjectResult("No HttpContext");

            var session = context.Session;
            var cartJson = session.GetString("SaleCart");
            if (string.IsNullOrEmpty(cartJson)) return new BadRequestObjectResult("Giỏ hàng trống");

            var cart = System.Text.Json.JsonSerializer.Deserialize<List<CartItemSale>>(cartJson);
            if (cart == null || !cart.Any()) return new BadRequestObjectResult("Không có sản phẩm nào trong giỏ hàng");

            var selectedItems = cart
                .Where(c => request.SelectedProductIds.Contains(c.ProductId))
                .ToList();

            if (!selectedItems.Any())
                return new BadRequestObjectResult("Bạn chưa chọn sản phẩm nào để thanh toán");

            var productIds = selectedItems.Select(c => c.ProductId).ToList();
            var products = await _context.SaleBooks
                .Where(p => productIds.Contains(p.SaleBookId))
                .ToDictionaryAsync(p => p.SaleBookId);

            var orderDetails = new List<SaleOrderDetail>();
            decimal totalAmount = 0;

            foreach (var item in selectedItems)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    continue;

                if (item.Quantity > product.Quantity)
                    return new BadRequestObjectResult($"Số lượng vượt quá tồn kho của sản phẩm {product.Title}");

                decimal lineTotal = item.Quantity * product.FinalPrice;
                totalAmount += lineTotal;

                orderDetails.Add(new SaleOrderDetail
                {
                    ProductId = product.SaleBookId,
                    ProductName = product.Title,
                    UnitPrice = product.FinalPrice,
                    Quantity = item.Quantity,
                    SubTotal = lineTotal
                });
            }

            // ✅ Tính phí vận chuyển
            const decimal shippingFee = 20000;
            if (request.HasShippingFee)
            {
                if (string.IsNullOrWhiteSpace(request.Address) || string.IsNullOrWhiteSpace(request.Phone))
                    return new BadRequestObjectResult("Vui lòng nhập địa chỉ và số điện thoại");

                totalAmount += shippingFee;
            }

            // Áp dụng mã giảm giá
            decimal discountAmount = 0;
            if (!string.IsNullOrWhiteSpace(request.VoucherCode))
            {
                var voucher = await _context.Vouchers
                    .Include(v => v.DiscountCode)
                    .FirstOrDefaultAsync(v => v.Code == request.VoucherCode &&
                                              v.UserId == request.UserId &&
                                              !v.IsUsed);

                if (voucher == null)
                    return new BadRequestObjectResult("Mã giảm giá không hợp lệ hoặc đã sử dụng");

                var discountPercent = voucher.DiscountCode.DiscountValue;

                

                discountAmount = (decimal)totalAmount * (decimal)discountPercent / 100m;

                if (discountAmount > totalAmount)
                    discountAmount = totalAmount;
            }

            var order = new SaleOrder
            {
                OrderId = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                OrderDate = DateTime.Now,
                PaymentMethod = request.PaymentMethod,
				OriginalTotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount - discountAmount,
                Status = OrderStatus.Pending,
                HasShippingFee = request.HasShippingFee,
                ShippingFee = request.HasShippingFee ? shippingFee : 0,
                Address = request.Address,
                Phone = request.Phone
            };

            foreach (var detail in orderDetails)
                detail.OrderId = order.OrderId;

            return new SaleOrderResult
            {
                Order = order,
                Details = orderDetails,
                VoucherCode = request.VoucherCode
            };
        }


        private async Task SaveOrderAsync(SaleOrderResult result)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            var session = context.Session;
            var cartJson = session.GetString("SaleCart");
            if (!string.IsNullOrEmpty(cartJson))
            {
                var cart = JsonConvert.DeserializeObject<List<CartItemSale>>(cartJson);
                var remaining = cart?.Where(x => !result.Details.Any(d => d.ProductId == x.ProductId)).ToList();
                session.SetString("SaleCart", JsonConvert.SerializeObject(remaining));
            }

            await _context.SaleOrders.AddAsync(result.Order);
            await _context.SaleOrderDetails.AddRangeAsync(result.Details);

            foreach (var detail in result.Details)
            {
                var product = await _context.SaleBooks.FirstOrDefaultAsync(p => p.SaleBookId == detail.ProductId);
                if (product != null)
                {
                    product.Quantity -= detail.Quantity;
                    if (product.Quantity < 0) product.Quantity = 0;
                }
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == result.Order.UserId);
            if (user != null)
            {
                double points = (double)(result.Order.TotalAmount / 1000);
                user.Points += points;
            }

            // Đánh dấu voucher đã dùng
            if (!string.IsNullOrEmpty(result.VoucherCode))
            {
                var voucher = await _context.Vouchers
                    .FirstOrDefaultAsync(v => v.Code == result.VoucherCode &&
                                              v.UserId == result.Order.UserId &&
                                              !v.IsUsed);
                if (voucher != null)
                    voucher.IsUsed = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
