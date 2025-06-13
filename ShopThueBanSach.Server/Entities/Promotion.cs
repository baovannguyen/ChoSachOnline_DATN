namespace ShopThueBanSach.Server.Entities
{
    public class Promotion
    {
        public string PromotionId { get; set; } = Guid.NewGuid().ToString();
        public string PromotionName { get; set; }
        public double DiscountPercentage { get; set; } // 0.1 = 10%
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation property
        public ICollection<SaleBook> SaleBooks { get; set; }
    }
}
