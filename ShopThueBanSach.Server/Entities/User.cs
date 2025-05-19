using Microsoft.AspNetCore.Identity;

namespace ShopThueBanSach.Server.Entities
{
    public class User : IdentityUser
    {
        public string? VaiTro { get; set; }
        public string? DiaChi { get; set; }
        public DateTime NgaySinh { get; set; }
        public double DiemTichLuy { get; set; }
        public string? HinhDaiDien { get; set; }
    }
}
