using ShopThueBanSach.Server.Area.Admin.Models;

namespace ShopThueBanSach.Server.Area.Admin.Service.Interface
{
    public interface IReportService
    {
        Task<BookStatisticsDto> GetBookStatisticsAsync();
    }
}