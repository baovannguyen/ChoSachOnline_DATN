using ShopThueBanSach.Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace ShopThueBanSach.Server.Models.CartModel
{
    public class CartItemRent
    {
        
        public string BookId { get; set; }
        public string BookTitle { get; set; }
        public decimal BookPrice { get; set; }
        public decimal RentalFee { get; set; }
        public decimal TotalFee { get; set; }
        public int Quantity { get; set; }
        public bool IsSelected { get; set; }
    }
}
