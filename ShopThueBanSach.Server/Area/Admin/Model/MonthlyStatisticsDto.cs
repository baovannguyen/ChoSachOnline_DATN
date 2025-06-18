namespace ShopThueBanSach.Server.Area.Admin.Model
{
    // 4. Thống kê tháng này
    public class MonthlyStatisticsDto
    {
        public int RentBooksThisMonth { get; set; }
        public int SaleBooksThisMonth { get; set; }
        public decimal RentBookValueThisMonth { get; set; }
        public decimal SaleBookValueThisMonth { get; set; }
    }
}
