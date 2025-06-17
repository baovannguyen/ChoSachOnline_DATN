namespace ShopThueBanSach.Server.Entities
{
    public class Slide
    {
        public int SlideId { get; set; } // Tương ứng MaSlide
        public string ImageUrl { get; set; } = string.Empty; // HinhAnh
        public string? LinkUrl { get; set; } // DuongDan
    }
}
