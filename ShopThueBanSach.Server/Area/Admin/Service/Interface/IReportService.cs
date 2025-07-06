using ShopThueBanSach.Server.Area.Admin.Model;

using ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Daily;
using ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Monthly;
using ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Yearly;
using ShopThueBanSach.Server.Entities;
using System.Threading.Tasks;

namespace ShopThueBanSach.Server.Area.Admin.Service.Interface
{
    public interface IReportService
    {
        // Thống kê tổng quan
        Task<OverviewStatisticsDto> GetOverviewStatisticsAsync();
        Task<OrderSummaryDto> GetOrderSummaryAsync();

        // Thống kê theo ngày
        /*      Task<DailyRentBookStatisticsDto> GetDailyRentBookStatisticsAsync();*/
        Task<DailySaleBookStatisticsDto> GetDailySaleBookStatisticsAsync();

        // Thống kê theo tháng
        /*      Task<MonthlyRentBookStatisticsDto> GetMonthlyRentBookStatisticsAsync();*/
        Task<MonthlySaleBookStatisticsDto> GetMonthlySaleBookStatisticsAsync();

        // Thống kê theo năm
        /*    Task<YearlyRentBookStatisticsDto> GetYearlyRentBookStatisticsAsync();*/
        Task<YearlySaleBookStatisticsDto> GetYearlySaleBookStatisticsAsync();
        Task<List<SaleOrder>> GetSaleOrdersByDateAsync(DateTime date);
        Task<List<SaleOrder>> GetSaleOrdersByMonthAsync(int year, int month);
        Task<List<SaleOrder>> GetSaleOrdersByYearAsync(int year);
        // yyyy
        // Thống kê đơn thuê theo ngày
        Task<List<RentOrder>> GetRentOrdersByDateAsync(DateTime date);

        // Thống kê đơn thuê theo tháng
        Task<List<RentOrder>> GetRentOrdersByMonthAsync(int year, int month);

        // Thống kê đơn thuê theo năm
        Task<List<RentOrder>> GetRentOrdersByYearAsync(int year);
        Task<DailyRentBookStatisticsDto> GetDailyRentBookStatisticsAsync();
        Task<MonthlyRentBookStatisticsDto> GetMonthlyRentBookStatisticsAsync();
        Task<YearlyRentBookStatisticsDto> GetYearlyRentBookStatisticsAsync();

    }
}
