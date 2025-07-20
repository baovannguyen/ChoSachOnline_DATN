using ShopThueBanSach.Server.Models.SaleModel.CartSaleModel;

namespace ShopThueBanSach.Server.Models.Vnpay
{
	public class PaymentSessionModel
	{
		public string UserId { get; set; }
		public List<CartItemSale> CartItems { get; set; }
		public decimal Amount { get; set; }
		public string OrderDescription { get; set; }
		public string Tick { get; set; }

		public string TxnRef { get; set; } = string.Empty; // BẮT BUỘC có
	}
}