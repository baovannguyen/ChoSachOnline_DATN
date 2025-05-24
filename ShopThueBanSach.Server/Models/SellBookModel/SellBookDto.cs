namespace ShopThueBanSach.Server.Models.SellBookModel
{
    public class SellBookDto
    {
        public int SellBookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public string Publisher { get; set; }
        public int PageCount { get; set; }
        public string Translator { get; set; }
        public string PackageSize { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public int? DiscountId { get; set; }
    }
}
