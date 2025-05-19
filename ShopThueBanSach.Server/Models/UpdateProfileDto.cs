namespace ShopThueBanSach.Server.Models
{
    public class UpdateProfileDto
    {
        public string TenNguoiDung { get; set; } = string.Empty;
        public string? SDT { get; set; }
        public string? DiaChi { get; set; }
        public DateTime NgaySinh { get; set; }
        public IFormFile? HinhDaiDien { get; set; }
    }
}
