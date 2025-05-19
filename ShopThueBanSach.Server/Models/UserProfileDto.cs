namespace ShopThueBanSach.Server.Models
{
    public class UserProfileDto
    {
        public string TenNguoiDung { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SDT { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public double DiemTichLuy { get; set; }
        public string? HinhDaiDien { get; set; }
    }
}
