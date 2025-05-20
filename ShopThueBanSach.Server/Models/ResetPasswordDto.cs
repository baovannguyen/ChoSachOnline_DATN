namespace ShopThueBanSach.Server.Models
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string MatKhauMoi { get; set; } = string.Empty;
    }
}
