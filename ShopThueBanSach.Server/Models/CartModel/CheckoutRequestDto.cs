namespace ShopThueBanSach.Server.Models.CartModel
{
    public class CheckoutRequestDto
    {
        public bool IsDelivery { get; set; }

        public string? Address { get; set; }
        public string? Phone { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string PaymentMethod { get; set; } // "cash" | "bank"
    }
}
