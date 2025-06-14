namespace ShopThueBanSach.Server.Area.Admin.Service
{
    using ShopThueBanSach.Server.Area.Admin.Model;
    using ShopThueBanSach.Server.Area.Admin.Service.Interface;
    using ShopThueBanSach.Server.Data;
    using ShopThueBanSach.Server.Entities;
    using ShopThueBanSach.Server.Services.Interfaces;

    public class ReportService : IReportService
    {
        private readonly AppDBContext _context;

        public ReportService(AppDBContext context)
        {
            _context = context;
        }

        public BookStatisticsDto GetBookStatistics()
        {
            var totalRentBooks = _context.RentBooks.Count();
            var totalRentBookItems = _context.RentBookItems.Count();
            var availableRentBookItems = _context.RentBookItems.Count(x => x.Status == RentBookItemStatus.Available);

            var totalSaleBooks = _context.SaleBooks.Count();

            var totalRentBookValue = _context.RentBooks.Sum(x => x.Price * x.Quantity);
            var totalSaleBookValue = _context.SaleBooks.Sum(x => x.Price * x.Quantity);

            return new BookStatisticsDto
            {
                TotalRentBooks = totalRentBooks,
                TotalRentBookItems = totalRentBookItems,
                AvailableRentBookItems = availableRentBookItems,
                TotalSaleBooks = totalSaleBooks,
                TotalRentBookValue = totalRentBookValue,
                TotalSaleBookValue = totalSaleBookValue
            };
        }
    }

}
