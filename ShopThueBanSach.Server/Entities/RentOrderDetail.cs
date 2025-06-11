using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopThueBanSach.Server.Entities
{
    public class RentOrderDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OrderId { get; set; } // Foreign Key
        [ForeignKey("OrderId")]
        public RentOrder Order { get; set; }

        [Required]
        public string BookId { get; set; } // Foreign Key
        public RentBook Book { get; set; }

        public string BookTitle { get; set; }

        public int Quantity { get; set; }

        public decimal BookPrice { get; set; }

        public decimal RentalFee { get; set; }

        public decimal TotalFee { get; set; }
    }
}
