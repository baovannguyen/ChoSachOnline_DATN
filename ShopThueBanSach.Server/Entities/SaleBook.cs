using ShopThueBanSach.Server.Entities.Relationships;
using System.Text.Json.Serialization;

namespace ShopThueBanSach.Server.Entities
{
    public class SaleBook
    {
        public string SaleBookId { get; set; } = Guid.NewGuid().ToString(); // Mã sách bán (PK)

        public string Title { get; set; } // Tên sách

        public string? Description { get; set; } // Mô tả sách

        public string? Publisher { get; set; } // Nhà xuất bản

        public string? Translator { get; set; } // Dịch giả

        public string? Size { get; set; } // Kích thước

        public int Pages { get; set; } // Số trang

        public decimal Price { get; set; } // Giá gốc
        public decimal FinalPrice { get; set; } // Giá sau khuyến mãi


        public int Quantity { get; set; } // Số lượng tồn kho

        public string? ImageUrl { get; set; } // Ảnh

        public bool IsHidden { get; set; } // Có ẩn sách không

        // Quan hệ nhiều-nhiều với tác giả
        public ICollection<AuthorSaleBook> AuthorSaleBooks { get; set; }

        // Quan hệ nhiều-nhiều với thể loại
        public ICollection<CategorySaleBook> CategorySaleBooks { get; set; }

        // FK và Navigation Property cho Promotion (1 sách có 1 khuyến mãi)
        // Add this to SaleBook class
        public string? PromotionId { get; set; } // FK
        public Promotion? Promotion { get; set; } // Navigation property

        // Trong SaleBook.cs
        public ICollection<FavoriteBook> FavoriteBooks { get; set; }



    }
}
