using ShopThueBanSach.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopThueBanSach.Server.Entities
{
    public class SaleOrder
    {
        [Key]
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public bool HasShippingFee { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public decimal OriginalTotalAmount { get; set; }
        [JsonIgnore]
        public List<SaleOrderDetail> Details { get; set; } = new();
    }
}
