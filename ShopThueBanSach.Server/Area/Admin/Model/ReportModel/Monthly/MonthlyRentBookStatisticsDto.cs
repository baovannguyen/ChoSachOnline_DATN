using ShopThueBanSach.Server.Models;

namespace ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Monthly
{
    public class MonthlyRentBookStatisticsDto
    {

        public List<DateTime> ActualReturnDates { get; set; }
        public int OrdersThisMonth { get; set; }
        public decimal TotalValueThisMonth { get; set; }
        public List<OrderStatus> Statuses { get; set; } = new(); // Thêm dòng này
    }
}
