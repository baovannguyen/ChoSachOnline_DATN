namespace ShopThueBanSach.Server.Models.PaymentMethod
{
    public class CreatePaymentRequest
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
    }
}
