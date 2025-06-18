using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopThueBanSach.Server.Entities
{
    public class RentOrderDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OrderId { get; set; }
        [ForeignKey("OrderId")]
        public RentOrder Order { get; set; }

        [Required]
        public string RentBookItemId { get; set; }
        public RentBookItem RentBookItem { get; set; }

        public string BookTitle { get; set; }
        public int Condition { get; set; }

        public decimal BookPrice { get; set; }
        public decimal RentalFee { get; set; }
        public decimal TotalFee { get; set; }
    }
}
