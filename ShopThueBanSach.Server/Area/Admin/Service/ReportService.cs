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

        public BookStatisticsDto GetBookStatistics()
        {
            var rentBooks = _context.RentBooks.AsNoTracking();
            var rentBookItems = _context.RentBookItems.AsNoTracking();
            var saleBooks = _context.SaleBooks.AsNoTracking();

            return BuildStatistics(rentBooks, rentBookItems, saleBooks);
        }

        public BookStatisticsDto GetBookStatisticsByWeek(int year, int weekNumber)
        {
            var startDate = FirstDateOfWeekISO8601(year, weekNumber);
            var endDate = startDate.AddDays(7);

            var rentBooks = _context.RentBooks
                .AsNoTracking()
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate);

            var rentBookItems = _context.RentBookItems
                .AsNoTracking()
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate);

            var saleBooks = _context.SaleBooks
                .AsNoTracking()
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate);

            return BuildStatistics(rentBooks, rentBookItems, saleBooks);
        }

        public BookStatisticsDto GetBookStatisticsByMonth(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var rentBooks = _context.RentBooks
                .AsNoTracking()
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate);

            var rentBookItems = _context.RentBookItems
                .AsNoTracking()
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate);

            var saleBooks = _context.SaleBooks
                .AsNoTracking()
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate);

            return BuildStatistics(rentBooks, rentBookItems, saleBooks);
        }

        private BookStatisticsDto BuildStatistics(
            IQueryable<RentBook> rentBooks,
            IQueryable<RentBookItem> rentBookItems,
            IQueryable<SaleBook> saleBooks)
        {
            return new BookStatisticsDto
            {
                TotalRentBooks = rentBooks.Count(),
                TotalRentBookItems = rentBookItems.Count(),
                AvailableRentBookItems = rentBookItems.Count(x => x.Status == RentBookItemStatus.Available),
                TotalSaleBooks = saleBooks.Count(),
                TotalRentBookValue = rentBooks.Sum(x => (decimal?)(x.Price * (decimal)x.Quantity)) ?? 0,
                TotalSaleBookValue = saleBooks.Sum(x => (decimal?)(x.Price * (decimal)x.Quantity)) ?? 0
            };
        }

        private DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            var firstThursday = jan1.AddDays(daysOffset);
            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            if (firstWeek <= 1)
                weekOfYear--;

            return firstThursday.AddDays(weekOfYear * 7 - 3);
        }
    }
}
