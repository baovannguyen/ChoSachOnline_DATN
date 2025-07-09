namespace ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Daily
{
    public class DailySaleBookStatisticsDto
    {
        public DateTime CreatedDate { get; set; }
        public int OrdersToday { get; set; }
        public decimal TotalValueToday { get; set; }
    }
}
