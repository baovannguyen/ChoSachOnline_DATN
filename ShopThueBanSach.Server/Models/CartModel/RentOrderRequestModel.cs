namespace ShopThueBanSach.Server.Models.CartModel
{
    public class RentOrderRequestModel
    {
        public List<CartItemRent> CartItems { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool HasShippingFee { get; set; }
    }
}
