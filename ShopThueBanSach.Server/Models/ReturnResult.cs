namespace ShopThueBanSach.Server.Models
{
    public class ReturnResult
    {
        public bool IsLate { get; set; }
        public int DaysLate { get; set; }
        public decimal ExtraFee { get; set; }
        public decimal CompensationFee { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
