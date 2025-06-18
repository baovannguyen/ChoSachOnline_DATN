using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Area.Admin.Models;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;

namespace ShopThueBanSach.Server.Area.Admin.Service
{
    public class ReportService : IReportService
    {
        private readonly AppDBContext _context;

        public ReportService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<BookStatisticsDto> GetBookStatisticsAsync()
        {
            var rentBooks = _context.RentBooks.AsNoTracking();
            var rentBookItems = _context.RentBookItems.AsNoTracking();
            var saleBooks = _context.SaleBooks.AsNoTracking();

            return await Task.FromResult(BuildStatistics(rentBooks, rentBookItems, saleBooks));
        }

        private BookStatisticsDto BuildStatistics(
            IQueryable<RentBook> rentBooks,
            IQueryable<RentBookItem> rentBookItems,
            IQueryable<SaleBook> saleBooks)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday).Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            return new BookStatisticsDto
            {
                // Tổng thể
                TotalRentBooks = rentBooks.Sum(x => (int?)x.Quantity) ?? 0,
                TotalRentBookItems = rentBookItems.Count(),
                AvailableRentBookItems = rentBookItems.Count(x => x.Status == RentBookItemStatus.Available),
                TotalSaleBooks = saleBooks.Sum(x => (int?)x.Quantity) ?? 0,
                TotalRentBookValue = rentBooks.Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,
                TotalSaleBookValue = saleBooks.Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,

                // Hôm nay
                RentBooksToday = rentBooks.Where(x => x.CreatedDate.Date == today).Sum(x => (int?)x.Quantity) ?? 0,
                SaleBooksToday = saleBooks.Where(x => x.CreatedDate.Date == today).Sum(x => (int?)x.Quantity) ?? 0,
                RentBookValueToday = rentBooks.Where(x => x.CreatedDate.Date == today)
                                              .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,
                SaleBookValueToday = saleBooks.Where(x => x.CreatedDate.Date == today)
                                              .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,

                // Tuần này
                RentBooksThisWeek = rentBooks.Where(x => x.CreatedDate >= startOfWeek)
                                             .Sum(x => (int?)x.Quantity) ?? 0,
                SaleBooksThisWeek = saleBooks.Where(x => x.CreatedDate >= startOfWeek)
                                             .Sum(x => (int?)x.Quantity) ?? 0,
                RentBookValueThisWeek = rentBooks.Where(x => x.CreatedDate >= startOfWeek)
                                                 .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,
                SaleBookValueThisWeek = saleBooks.Where(x => x.CreatedDate >= startOfWeek)
                                                 .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,

                // Tháng này
                RentBooksThisMonth = rentBooks.Where(x => x.CreatedDate >= startOfMonth)
                                              .Sum(x => (int?)x.Quantity) ?? 0,
                SaleBooksThisMonth = saleBooks.Where(x => x.CreatedDate >= startOfMonth)
                                              .Sum(x => (int?)x.Quantity) ?? 0,
                RentBookValueThisMonth = rentBooks.Where(x => x.CreatedDate >= startOfMonth)
                                                  .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0,
                SaleBookValueThisMonth = saleBooks.Where(x => x.CreatedDate >= startOfMonth)
                                                  .Sum(x => (decimal?)(x.Price * x.Quantity)) ?? 0
            };
        }
    }
}
