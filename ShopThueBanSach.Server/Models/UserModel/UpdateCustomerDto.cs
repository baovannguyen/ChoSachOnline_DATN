namespace ShopThueBanSach.Server.Models.UserModel
{
    public class UpdateCustomerDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? Points { get; set; }
        public string? ImageUser { get; set; }
    }
}
