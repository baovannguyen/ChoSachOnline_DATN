namespace ShopThueBanSach.Server.Area.Admin.Model
{
    public class DailyStatisticsDto
    {
        public int RentBooksToday { get; set; }
        public int SaleBooksToday { get; set; }
        public decimal RentBookValueToday { get; set; }
        public decimal SaleBookValueToday { get; set; }
    }
}
