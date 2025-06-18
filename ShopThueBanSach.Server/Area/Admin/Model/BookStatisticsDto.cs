using ShopThueBanSach.Server.Area.Admin.Model;

namespace ShopThueBanSach.Server.Area.Admin.Models
{
    // 5. DTO gốc – gom cả 4 phần
    public class BookStatisticsDto
    {
        public OverviewStatisticsDto Overview { get; set; } = default!;
        public DailyStatisticsDto Daily { get; set; } = default!;
        public WeeklyStatisticsDto Weekly { get; set; } = default!;
        public MonthlyStatisticsDto Monthly { get; set; } = default!;
    }
}