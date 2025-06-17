namespace ShopThueBanSach.Server.Models.AuthModel
{
    public class UpdateUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Role { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? Points { get; set; }
        public string? ImageUser { get; set; }
        public string? Email { get; set; }
    }
}
