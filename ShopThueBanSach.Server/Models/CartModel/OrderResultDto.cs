namespace ShopThueBanSach.Server.Models.CartModel
{
    public class OrderResultDto
    {
        public string OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Message { get; set; }
    }
}
