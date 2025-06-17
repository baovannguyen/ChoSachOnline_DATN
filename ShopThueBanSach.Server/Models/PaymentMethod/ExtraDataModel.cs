using ShopThueBanSach.Server.Models.CartModel;

namespace ShopThueBanSach.Server.Models.PaymentMethod
{
    public class ExtraDataModel
    {
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool HasShippingFee { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public List<CartItemRent> CartItems { get; set; }
    }
}
