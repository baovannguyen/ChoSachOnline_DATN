using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Area.Admin.Model;
using ShopThueBanSach.Server.Area.Admin.Models;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopThueBanSach.Server.Area.Admin.Service
{
    public class ReportService : IReportService
    {
        private readonly AppDBContext _context;

        public ReportService(AppDBContext context)
        {
            _context = context;
        }

        // ===== 1. DTO gộp (đủ 4 phần) =====
        public async Task<BookStatisticsDto> GetBookStatisticsAsync()
        {
            var rentBooks = _context.RentBooks.AsNoTracking();
            var rentBookItems = _context.RentBookItems.AsNoTracking();
            var saleBooks = _context.SaleBooks.AsNoTracking();

            return await Task.FromResult(BuildStatistics(rentBooks, rentBookItems, saleBooks));
        }

        // ===== 2. Từng DTO riêng lẻ =====
        public async Task<OverviewStatisticsDto> GetOverviewStatisticsAsync()
            => (await GetBookStatisticsAsync()).Overview;

        public async Task<DailyStatisticsDto> GetDailyStatisticsAsync()
            => (await GetBookStatisticsAsync()).Daily;

        public async Task<WeeklyStatisticsDto> GetWeeklyStatisticsAsync()
            => (await GetBookStatisticsAsync()).Weekly;

        public async Task<MonthlyStatisticsDto> GetMonthlyStatisticsAsync()
            => (await GetBookStatisticsAsync()).Monthly;

        // ===== PRIVATE: tính toán toàn bộ =====
        private BookStatisticsDto BuildStatistics(
            IQueryable<RentBook> rentBooks,
            IQueryable<RentBookItem> rentBookItems,
            IQueryable<SaleBook> saleBooks)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday).Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // 1. Tổng quan
            var overview = new OverviewStatisticsDto
            {
                TotalRentBooks = rentBooks.Sum(x => (int?)x.Quantity) ?? 0,
                TotalSaleBooks = saleBooks.Sum(x => (int?)x.Quantity) ?? 0,
                TotalRentBookItems = rentBookItems.Count(),
                AvailableRentBookItems = rentBookItems.Count(x => x.Status == RentBookItemStatus.Available),
                TotalRentBookValue = rentBooks.Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,
                TotalSaleBookValue = saleBooks.Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0
            };

            // 2. Hôm nay
            var daily = new DailyStatisticsDto
            {
                RentBooksToday = rentBooks.Where(x => x.CreatedDate.Date == today).Sum(x => (int?)x.Quantity) ?? 0,
                SaleBooksToday = saleBooks.Where(x => x.CreatedDate.Date == today).Sum(x => (int?)x.Quantity) ?? 0,
                RentBookValueToday = rentBooks.Where(x => x.CreatedDate.Date == today)
                                              .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,
                SaleBookValueToday = saleBooks.Where(x => x.CreatedDate.Date == today)
                                              .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0
            };

            // 3. Tuần này
            var weekly = new WeeklyStatisticsDto
            {
                RentBooksThisWeek = rentBooks.Where(x => x.CreatedDate >= startOfWeek)
                                                 .Sum(x => (int?)x.Quantity) ?? 0,
                SaleBooksThisWeek = saleBooks.Where(x => x.CreatedDate >= startOfWeek)
                                                 .Sum(x => (int?)x.Quantity) ?? 0,
                RentBookValueThisWeek = rentBooks.Where(x => x.CreatedDate >= startOfWeek)
                                                 .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,
                SaleBookValueThisWeek = saleBooks.Where(x => x.CreatedDate >= startOfWeek)
                                                 .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0
            };

            // 4. Tháng này
            var monthly = new MonthlyStatisticsDto
            {
                RentBooksThisMonth = rentBooks.Where(x => x.CreatedDate >= startOfMonth)
                                                  .Sum(x => (int?)x.Quantity) ?? 0,
                SaleBooksThisMonth = saleBooks.Where(x => x.CreatedDate >= startOfMonth)
                                                  .Sum(x => (int?)x.Quantity) ?? 0,
                RentBookValueThisMonth = rentBooks.Where(x => x.CreatedDate >= startOfMonth)
                                                  .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,
                SaleBookValueThisMonth = saleBooks.Where(x => x.CreatedDate >= startOfMonth)
                                                  .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0
            };

            return new BookStatisticsDto
            {
                Overview = overview,
                Daily = daily,
                Weekly = weekly,
                Monthly = monthly
            };
        }
    }
}
