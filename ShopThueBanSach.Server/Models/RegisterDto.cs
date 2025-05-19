namespace ShopThueBanSach.Server.Models
{
    public class RegisterDto
    {
        public string TenNguoiDung { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string XacNhanMatKhau { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
    }
}
