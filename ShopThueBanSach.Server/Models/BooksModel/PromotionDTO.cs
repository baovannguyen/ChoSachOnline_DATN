namespace ShopThueBanSach.Server.Models.BooksModel
{
    public class PromotionDTO
    {
        public string? PromotionId { get; set; }
        public string PromotionName { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
