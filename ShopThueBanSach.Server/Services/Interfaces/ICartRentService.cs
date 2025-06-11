using ShopThueBanSach.Server.Models.CartModel;

namespace ShopThueBanSach.Server.Services.Interfaces
{
    public interface ICartRentService
    {
        List<CartItemRent> GetCart();
        void AddToCart(string bookId);
        void UpdateQuantity(string bookId, int quantity);
        void ToggleSelect(string bookId);
        void RemoveFromCart(string bookId);
        void ClearCart();
        List<CartItemRent> GetSelectedItems();
        void RecalculateRentalFee(DateTime startDate, DateTime endDate);
    }
}
