namespace ShopThueBanSach.Server.Models.CartModel
{
    public class RentOrderRequest
    {
        public string UserId { get; set; } // UserId sẽ được gán từ token khi gọi API
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public bool HasShippingFee { get; set; }

        public string? Address { get; set; }
        public string? Phone { get; set; }

        public string PaymentMethod { get; set; } // e.g. "Cash", "BankTransfer"
    }
}
