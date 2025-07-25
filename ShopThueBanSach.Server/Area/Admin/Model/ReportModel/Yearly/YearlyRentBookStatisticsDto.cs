using ShopThueBanSach.Server.Models;

namespace ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Yearly
{
    public class YearlyRentBookStatisticsDto
    {

        public List<DateTime> ActualReturnDates { get; set; }
        public int OrdersThisYear { get; set; }
        public decimal TotalValueThisYear { get; set; }
        public List<OrderStatus> Statuses { get; set; } = new(); // Thêm dòng này
    }
}
