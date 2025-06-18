using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShopThueBanSach.Server.Models.CartRentModel;

namespace ShopThueBanSach.Server.Entities
{
    public class RentOrder
    {
        [Key]
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        [NotMapped]
        public List<CartItemRent> Items { get; set; } = new();

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime StartDate { get; set; }  // Ngày bắt đầu thuê
        public DateTime EndDate { get; set; }    // Ngày kết thúc thuê
        public int RentalDays { get; set; }      // Số ngày thuê

        public bool HasShippingFee { get; set; } = false;
        public decimal ShippingFee { get; set; } = 0;

        public decimal TotalFee { get; set; }         // Phí thuê
        public decimal TotalDeposit { get; set; }     // Tổng tiền cọc
        public Payment? Payment { get; set; }

        public string Status { get; set; } = "Pending"; // trạng thái đơn thuê ("Pending", "Confirmed", "Delivered", "Returned", "Overdue", ...)
    }
}
