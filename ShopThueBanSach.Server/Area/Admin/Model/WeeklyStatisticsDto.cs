namespace ShopThueBanSach.Server.Area.Admin.Model
{
    // 3. Thống kê tuần này
    public class WeeklyStatisticsDto
    {
        public int RentBooksThisWeek { get; set; }
        public int SaleBooksThisWeek { get; set; }
        public decimal RentBookValueThisWeek { get; set; }
        public decimal SaleBookValueThisWeek { get; set; }
    }
}
