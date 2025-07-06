using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Area.Admin.Model;

using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Daily;
using ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Monthly;
using ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Yearly;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models;
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

        // Tổng quan số lượng và giá trị sách
        public async Task<OverviewStatisticsDto> GetOverviewStatisticsAsync()
        {
            var saleBooks = await _context.SaleBooks.AsNoTracking().ToListAsync();

            return new OverviewStatisticsDto
            {
                TotalSaleBooks = saleBooks.Sum(x => x.Quantity),
                TotalSaleBookValue = saleBooks.Sum(x => x.Price * x.Quantity)
            };
        }

        // Tổng đơn hàng và tổng doanh thu Sale
        public async Task<OrderSummaryDto> GetOrderSummaryAsync()
        {
            var saleOrders = _context.SaleOrders
                                     .AsNoTracking()
                                     .Where(o => o.Status != OrderStatus.Canceled);

            var rentOrders = _context.RentOrders
                                     .AsNoTracking()
                                     .Where(o => o.Status != OrderStatus.Canceled);

            return new OrderSummaryDto
            {
                TotalSaleOrders = await saleOrders.CountAsync(),
                TotalSaleAmount = await saleOrders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0,

                TotalRentOrders = await rentOrders.CountAsync(),
                TotalRentAmount = await rentOrders.SumAsync(o => (decimal?)o.TotalFee) ?? 0
            };
        }


        // Sale theo ngày
        public async Task<DailySaleBookStatisticsDto> GetDailySaleBookStatisticsAsync()
        {
            var today = DateTime.Today;
            var saleOrders = _context.SaleOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate.Date == today);

            return new DailySaleBookStatisticsDto
            {
                OrdersToday = await saleOrders.CountAsync(),
                TotalValueToday = await saleOrders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0
            };
        }

        // Sale theo tháng
        public async Task<MonthlySaleBookStatisticsDto> GetMonthlySaleBookStatisticsAsync()
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var saleOrders = _context.SaleOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate >= startOfMonth);

            return new MonthlySaleBookStatisticsDto
            {
                OrdersThisMonth = await saleOrders.CountAsync(),
                TotalValueThisMonth = await saleOrders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0
            };
        }

        // Sale theo năm
        public async Task<YearlySaleBookStatisticsDto> GetYearlySaleBookStatisticsAsync()
        {
            var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
            var saleOrders = _context.SaleOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate >= startOfYear);

            return new YearlySaleBookStatisticsDto
            {
                OrdersThisYear = await saleOrders.CountAsync(),
                TotalValueThisYear = await saleOrders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0
            };
        }
        public async Task<List<SaleOrder>> GetSaleOrdersByDateAsync(DateTime date)
        {
            return await _context.SaleOrders
                .Include(o => o.SaleOrderDetails)
                .ThenInclude(d => d.SaleBook)
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate.Date == date.Date)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SaleOrder>> GetSaleOrdersByMonthAsync(int year, int month)
        {
            return await _context.SaleOrders
                .Include(o => o.SaleOrderDetails)
                .ThenInclude(d => d.SaleBook)
                .Where(o => o.Status != OrderStatus.Canceled &&
                            o.OrderDate.Year == year &&
                            o.OrderDate.Month == month)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SaleOrder>> GetSaleOrdersByYearAsync(int year)
        {
            return await _context.SaleOrders
                .Include(o => o.SaleOrderDetails)
                .ThenInclude(d => d.SaleBook)
                .Where(o => o.Status != OrderStatus.Canceled &&
                            o.OrderDate.Year == year)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<RentOrder>> GetRentOrdersByDateAsync(DateTime date)
        {
            return await _context.RentOrders
                .Include(o => o.RentOrderDetails)
                .ThenInclude(d => d.RentBookItem)
                .ThenInclude(item => item.RentBook)
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate.Date == date.Date)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<RentOrder>> GetRentOrdersByMonthAsync(int year, int month)
        {
            return await _context.RentOrders
                .Include(o => o.RentOrderDetails)
                .ThenInclude(d => d.RentBookItem)
                .ThenInclude(item => item.RentBook)
                .Where(o => o.Status != OrderStatus.Canceled &&
                            o.OrderDate.Year == year &&
                            o.OrderDate.Month == month)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<RentOrder>> GetRentOrdersByYearAsync(int year)
        {
            return await _context.RentOrders
                .Include(o => o.RentOrderDetails)
                .ThenInclude(d => d.RentBookItem)
                .ThenInclude(item => item.RentBook)
                .Where(o => o.Status != OrderStatus.Canceled &&
                            o.OrderDate.Year == year)
                .AsNoTracking()
                .ToListAsync();
        }
        // Thống kê theo ngày
        public async Task<DailyRentBookStatisticsDto> GetDailyRentBookStatisticsAsync()
        {
            var today = DateTime.Today;
            var rentOrders = _context.RentOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate.Date == today);

            return new DailyRentBookStatisticsDto
            {
                OrdersToday = await rentOrders.CountAsync(),
                TotalValueToday = await rentOrders.SumAsync(o => (decimal?)o.TotalFee) ?? 0
            };
        }

        // Thống kê theo tháng
        public async Task<MonthlyRentBookStatisticsDto> GetMonthlyRentBookStatisticsAsync()
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var rentOrders = _context.RentOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate >= startOfMonth);

            return new MonthlyRentBookStatisticsDto
            {
                OrdersThisMonth = await rentOrders.CountAsync(),
                TotalValueThisMonth = await rentOrders.SumAsync(o => (decimal?)o.TotalFee) ?? 0
            };
        }

        // Thống kê theo năm
        public async Task<YearlyRentBookStatisticsDto> GetYearlyRentBookStatisticsAsync()
        {
            var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
            var rentOrders = _context.RentOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate >= startOfYear);

            return new YearlyRentBookStatisticsDto
            {
                OrdersThisYear = await rentOrders.CountAsync(),
                TotalValueThisYear = await rentOrders.SumAsync(o => (decimal?)o.TotalFee) ?? 0
            };
        }


    }
}
