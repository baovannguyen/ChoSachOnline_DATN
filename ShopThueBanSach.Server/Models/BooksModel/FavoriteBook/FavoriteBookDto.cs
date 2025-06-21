namespace ShopThueBanSach.Server.Models.BooksModel.FavoriteBook
{
    public class FavoriteBookDto
    {
        public string SaleBookId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }

        public string? PromotionName { get; set; }
        public double? DiscountPercentage { get; set; }
        public string UserName { get; set; } // 👈 Thêm dòng này
    }
}
