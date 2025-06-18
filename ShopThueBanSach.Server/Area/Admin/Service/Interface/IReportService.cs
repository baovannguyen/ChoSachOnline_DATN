using ShopThueBanSach.Server.Area.Admin.Model;
using ShopThueBanSach.Server.Area.Admin.Models;
using System.Threading.Tasks;

namespace ShopThueBanSach.Server.Area.Admin.Service.Interface
{
    public interface IReportService
    {
        // DTO gộp (đủ 4 phần)
        Task<BookStatisticsDto> GetBookStatisticsAsync();

        // DTO tách nhỏ – phục vụ các controller / endpoint riêng
        Task<OverviewStatisticsDto> GetOverviewStatisticsAsync();
        Task<DailyStatisticsDto> GetDailyStatisticsAsync();
        Task<WeeklyStatisticsDto> GetWeeklyStatisticsAsync();
        Task<MonthlyStatisticsDto> GetMonthlyStatisticsAsync();
    }
}
