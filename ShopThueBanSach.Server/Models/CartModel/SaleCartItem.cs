namespace ShopThueBanSach.Server.Models.CartModel
{
    public class SaleCartItem
    {
        public string SaleBookId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }

        public decimal TotalPrice => Price * Quantity;
    }
}
