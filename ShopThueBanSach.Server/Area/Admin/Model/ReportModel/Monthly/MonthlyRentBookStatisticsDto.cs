namespace ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Monthly
{
    public class MonthlyRentBookStatisticsDto
    {
        public List<DateTime> CreatedDates { get; set; } = new();
        public int OrdersThisMonth { get; set; }
        public decimal TotalValueThisMonth { get; set; }
    }
}
