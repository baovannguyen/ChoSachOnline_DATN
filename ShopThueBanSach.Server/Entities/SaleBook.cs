using ShopThueBanSach.Server.Entities.Relationships;

namespace ShopThueBanSach.Server.Entities
{
    public class SaleBook
    {
        public string SaleBookId { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Publisher { get; set; }

        public string? Translator { get; set; }

        public string? Size { get; set; }

        public int Pages { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsHidden { get; set; }

        public ICollection<AuthorSaleBook> AuthorSaleBooks { get; set; }

        public ICollection<CategorySaleBook> CategorySaleBooks { get; set; }
    }
}
