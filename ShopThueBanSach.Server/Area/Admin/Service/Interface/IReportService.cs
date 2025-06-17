using ShopThueBanSach.Server.Area.Admin.Models;

namespace ShopThueBanSach.Server.Area.Admin.Service.Interface
{
    public interface IReportService
    {
        BookStatisticsDto GetBookStatistics();
        BookStatisticsDto GetBookStatisticsByWeek(int year, int weekNumber); // Theo tuần
        BookStatisticsDto GetBookStatisticsByMonth(int year, int month); // Theo tháng
    }
}