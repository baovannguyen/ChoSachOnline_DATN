namespace ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Yearly
{
    public class YearlySaleBookStatisticsDto
    {
        public List<DateTime> CreatedDates { get; set; } = new();
        public int OrdersThisYear { get; set; }
        public decimal TotalValueThisYear { get; set; }
    }
}
