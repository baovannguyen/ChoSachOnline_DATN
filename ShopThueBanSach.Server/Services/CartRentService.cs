using Newtonsoft.Json;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Models;
using ShopThueBanSach.Server.Models.CartModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Services
{
    public class CartRentService : ICartRentService
    {
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "RentalCart";

        public CartRentService(IHttpContextAccessor httpContextAccessor, AppDBContext appDBContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = appDBContext;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public List<CartItemRent> GetCart()
        {
            var json = Session.GetString(SessionKey);
            return string.IsNullOrEmpty(json)
                ? new List<CartItemRent>()
                : JsonConvert.DeserializeObject<List<CartItemRent>>(json);
        }

        private void SaveCart(List<CartItemRent> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            Session.SetString(SessionKey, json);
        }

        public void AddToCart(string bookId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.BookId == bookId);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                var book = _context.RentBooks.FirstOrDefault(b => b.RentBookId == bookId);
                if (book == null) return;

                cart.Add(new CartItemRent
                {
                    BookId = book.RentBookId,
                    BookTitle = book.Title,
                    BookPrice = book.Price,
                    RentalFee = CalculateRentalFee(45), // Mặc định: 45 ngày thuê
                    Quantity = 1,
                    IsSelected = true
                });
            }

            SaveCart(cart);
        }

        public void UpdateQuantity(string bookId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.BookId == bookId);
            if (item != null)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }
        }

        public void ToggleSelect(string bookId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.BookId == bookId);
            if (item != null)
            {
                item.IsSelected = !item.IsSelected;
                SaveCart(cart);
            }
        }

        public void RemoveFromCart(string bookId)
        {
            var cart = GetCart();
            cart.RemoveAll(x => x.BookId == bookId);
            SaveCart(cart);
        }

        public void ClearCart()
        {
            Session.Remove(SessionKey);
        }

        public List<CartItemRent> GetSelectedItems()
        {
            return GetCart().Where(x => x.IsSelected).ToList();
        }

        private decimal CalculateRentalFee(int rentalDays)
        {
            decimal baseFee = 20000m / 45m;
            if (rentalDays <= 45)
                return baseFee * rentalDays;

            int extraDays = rentalDays - 45;
            return baseFee * 45 + extraDays * 3000;
        }

        // Gọi lại khi thay đổi ngày thuê
        public void RecalculateRentalFee(DateTime startDate, DateTime endDate)
        {
            var cart = GetCart();
            int rentalDays = (endDate - startDate).Days;
            if (rentalDays < 1) rentalDays = 1;

            foreach (var item in cart)
            {
                item.RentalFee = CalculateRentalFee(rentalDays) * item.Quantity;
            }

            SaveCart(cart);
        }
    }
}
