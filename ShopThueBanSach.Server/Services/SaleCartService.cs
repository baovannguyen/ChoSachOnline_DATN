using Newtonsoft.Json;
using ShopThueBanSach.Server.Models.CartModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Text.Json;

namespace ShopThueBanSach.Server.Services
{
    public class SaleCartService : ISaleCartService
    {
        private const string CartSessionKey = "SaleCart";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SaleCartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public async Task<List<SaleCartItem>> GetCartAsync()
        {
            var json = Session.GetString(CartSessionKey);
            return json != null
                ? JsonConvert.DeserializeObject<List<SaleCartItem>>(json)!
                : new List<SaleCartItem>();
        }
        public async Task IncreaseQuantityAsync(string saleBookId)
        {
            var cart = await GetCartAsync();
            var item = cart.FirstOrDefault(x => x.SaleBookId == saleBookId);

            if (item != null)
            {
                item.Quantity++;
                SaveCart(cart);
            }
        }

        private void SaveCart(List<SaleCartItem> cart)
        {
            var json = JsonConvert.SerializeObject(cart);
            Session.SetString(CartSessionKey, json);
        }

        public async Task AddToCartAsync(SaleCartItem item)
        {
            var cart = await GetCartAsync();

            var existing = cart.FirstOrDefault(x => x.SaleBookId == item.SaleBookId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                existing.Price = item.Price; // Optional: update latest price
            }
            else
            {
                cart.Add(item);
            }

            SaveCart(cart);
        }

        public async Task DecreaseQuantityAsync(string saleBookId)
        {
            var cart = await GetCartAsync();
            var item = cart.FirstOrDefault(x => x.SaleBookId == saleBookId);

            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    cart.Remove(item);

                SaveCart(cart);
            }
        }

        public async Task RemoveFromCartAsync(string saleBookId)
        {
            var cart = await GetCartAsync();
            var item = cart.FirstOrDefault(x => x.SaleBookId == saleBookId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
        }

        public async Task ClearCartAsync()
        {
            SaveCart(new List<SaleCartItem>());
        }

        public async Task<decimal> GetTotalAsync()
        {
            var cart = await GetCartAsync();
            return cart.Sum(x => x.Price * x.Quantity);
        }
    }
}
