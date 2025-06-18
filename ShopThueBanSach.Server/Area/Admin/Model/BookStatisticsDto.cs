namespace ShopThueBanSach.Server.Area.Admin.Models
{
    public class BookStatisticsDto
    {
        // Tổng thể
        public int TotalRentBooks { get; set; }
        public int TotalSaleBooks { get; set; }
        public int TotalRentBookItems { get; set; }
        public int AvailableRentBookItems { get; set; }
        public decimal TotalRentBookValue { get; set; }
        public decimal TotalSaleBookValue { get; set; }
        //theo ngày
        public int RentBooksToday { get; set; }
        public int SaleBooksToday { get; set; }
        public decimal RentBookValueToday { get; set; }
        public decimal SaleBookValueToday { get; set; }

        // Báo cáo theo tuần
        public int RentBooksThisWeek { get; set; }
        public int SaleBooksThisWeek { get; set; }
        public decimal RentBookValueThisWeek { get; set; }
        public decimal SaleBookValueThisWeek { get; set; }

        // Báo cáo theo tháng
        public int RentBooksThisMonth { get; set; }
        public int SaleBooksThisMonth { get; set; }
        public decimal RentBookValueThisMonth { get; set; }
        public decimal SaleBookValueThisMonth { get; set; }
    }
}