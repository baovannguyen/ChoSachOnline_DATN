using Microsoft.AspNetCore.Identity;

namespace ShopThueBanSach.Server.Entities
{
    public class User : IdentityUser
    {
        public string? Role { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public double Points { get; set; }
        public string? ImageUser { get; set; }
    }
}
