using System.ComponentModel.DataAnnotations;

namespace ShopThueBanSach.Server.Entities
{
    public class SellBook
    {
        public int SellBookId { get; set; } // MaSachBan
        public string Title { get; set; }   // TenSachBan
        public string Description { get; set; } // MoTa
        public int AuthorId { get; set; }   // MaTacGia
        public int CategoryId { get; set; } // MaTheLoai
        public string Publisher { get; set; } // NhaXuatBan
        public int PageCount { get; set; }  // SoTrang
        public string Translator { get; set; } // NguoiDich
        public string PackageSize { get; set; } // KichThuocBaoBi
        public decimal Price { get; set; }  // Gia
        public string ImageUrl { get; set; } // HinhAnh
        public int Quantity { get; set; }   // SoLuong
        public int? DiscountId { get; set; } // MaKhuyenMai (nullable)

        // Navigation properties
        public Author Author { get; set; }
        public Category Category { get; set; }
    /*    public Discount? Discount { get; set; }*/
    }

}
