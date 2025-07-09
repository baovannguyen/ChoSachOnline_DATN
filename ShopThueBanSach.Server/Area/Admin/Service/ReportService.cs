
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShopThueBanSach.Server.Area.Admin.Model;
using ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Daily;
using ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Monthly;
using ShopThueBanSach.Server.Area.Admin.Model.ReportModel.Yearly;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Data;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopThueBanSach.Server.Area.Admin.Service
{
    public class ReportService : IReportService
    {
        private readonly AppDBContext _context;
        private readonly IMemoryCache _cache;

        public ReportService(AppDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public Task SetDailySaleDateAsync(DateTime date)
        {
            if (date.Date > DateTime.Today)
                throw new ArgumentException("Ngày không hợp lệ vui lòng nhập lại");

            _cache.Set("DailySaleDate", date.Date, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task SetDailyRentDateAsync(DateTime date)
        {
            if (date.Date > DateTime.Today)
                throw new ArgumentException("Ngày không hợp lệ vui lòng nhập lại");

            _cache.Set("DailyRentDate", date.Date, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task SetMonthlySaleDateAsync(int year, int month)
        {
            var now = DateTime.Today;
            if (year > now.Year || (year == now.Year && month > now.Month))
                throw new ArgumentException("Tháng hoặc năm không hợp lệ");

            _cache.Set("MonthlySaleDate", (year, month), TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task SetYearlySaleDateAsync(int year)
        {
            if (year > DateTime.Today.Year)
                throw new ArgumentException("Năm không hợp lệ");

            _cache.Set("YearlySaleDate", year, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task SetMonthlyRentDateAsync(int year, int month)
        {
            var now = DateTime.Today;
            if (year > now.Year || (year == now.Year && month > now.Month))
                throw new ArgumentException("Tháng hoặc năm không hợp lệ");

            _cache.Set("MonthlyRentDate", (year, month), TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task SetYearlyRentDateAsync(int year)
        {
            if (year > DateTime.Today.Year)
                throw new ArgumentException("Năm không hợp lệ");

            _cache.Set("YearlyRentDate", year, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public async Task<OverviewStatisticsDto> GetOverviewStatisticsAsync()
        {
            var saleBooks = await _context.SaleBooks.AsNoTracking().ToListAsync();

            return new OverviewStatisticsDto
            {
                TotalSaleBooks = saleBooks.Sum(x => x.Quantity),
                TotalSaleBookValue = saleBooks.Sum(x => x.Price * x.Quantity)
            };
        }

        public async Task<OrderSummaryDto> GetOrderSummaryAsync()
        {
            var saleOrders = _context.SaleOrders.AsNoTracking().Where(o => o.Status != OrderStatus.Canceled);
            var rentOrders = _context.RentOrders.AsNoTracking().Where(o => o.Status != OrderStatus.Canceled);

            return new OrderSummaryDto
            {
                TotalSaleOrders = await saleOrders.CountAsync(),
                TotalSaleAmount = await saleOrders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
                TotalRentOrders = await rentOrders.CountAsync(),
                TotalRentAmount = await rentOrders.SumAsync(o => (decimal?)o.TotalFee) ?? 0
            };
        }

        public async Task<DailySaleBookStatisticsDto> GetDailySaleBookStatisticsAsync()
        {
            var selectedDate = _cache.TryGetValue("DailySaleDate", out DateTime cachedDate)
                ? cachedDate
                : DateTime.Today;

            var saleOrders = _context.SaleOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate.Date == selectedDate);

            return new DailySaleBookStatisticsDto
            {
                CreatedDate = selectedDate,
                OrdersToday = await saleOrders.CountAsync(),
                TotalValueToday = await saleOrders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0
            };
        }

        public async Task<MonthlySaleBookStatisticsDto> GetMonthlySaleBookStatisticsAsync()
        {
            var (year, month) = _cache.TryGetValue("MonthlySaleDate", out ValueTuple<int, int> yAndM)
                ? yAndM
                : (DateTime.Today.Year, DateTime.Today.Month);

            var saleOrders = await _context.SaleOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled &&
                            o.OrderDate.Year == year &&
                            o.OrderDate.Month == month)
                .ToListAsync();

            var createdDates = saleOrders
                .Select(o => o.OrderDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return new MonthlySaleBookStatisticsDto
            {
                CreatedDates = createdDates,
                OrdersThisMonth = saleOrders.Count,
                TotalValueThisMonth = saleOrders.Sum(o => o.TotalAmount)
            };
        }

        public async Task<YearlySaleBookStatisticsDto> GetYearlySaleBookStatisticsAsync()
        {
            int year = _cache.TryGetValue("YearlySaleDate", out int cachedYear)
                ? cachedYear
                : DateTime.Today.Year;

            var saleOrders = await _context.SaleOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate.Year == year)
                .ToListAsync();

            var createdDates = saleOrders
                .Select(o => o.OrderDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return new YearlySaleBookStatisticsDto
            {
                CreatedDates = createdDates,
                OrdersThisYear = saleOrders.Count,
                TotalValueThisYear = saleOrders.Sum(o => o.TotalAmount)
            };
        }

        public async Task<List<SaleOrder>> GetSaleOrdersByDateAsync(DateTime date)
        {
            return await _context.SaleOrders
                .Include(o => o.SaleOrderDetails)
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate.Date == date.Date)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SaleOrder>> GetSaleOrdersByMonthAsync(int year, int month)
        {
            var now = DateTime.Today;
            if (year > now.Year || (year == now.Year && month > now.Month))
                throw new ArgumentException("Tháng hoặc năm không hợp lẹ. Vui lòng nhập lại");

            return await _context.SaleOrders
                .Include(o => o.SaleOrderDetails)
                .Where(o => o.Status != OrderStatus.Canceled &&
                            o.OrderDate.Year == year &&
                            o.OrderDate.Month == month)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SaleOrder>> GetSaleOrdersByYearAsync(int year)
        {
            if (year > DateTime.Today.Year)
                throw new ArgumentException("Năm không hợp lệ vui lòng nhập lại");

            return await _context.SaleOrders
                .Include(o => o.SaleOrderDetails)
                .Where(o => o.Status != OrderStatus.Canceled &&
                            o.OrderDate.Year == year)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DailyRentBookStatisticsDto> GetDailyRentBookStatisticsAsync()
        {
            var selectedDate = _cache.TryGetValue("DailyRentDate", out DateTime cachedDate)
                ? cachedDate
                : DateTime.Today;

            var rentOrders = _context.RentOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate.Date == selectedDate);

            return new DailyRentBookStatisticsDto
            {
                CreatedDate = selectedDate,
                OrdersToday = await rentOrders.CountAsync(),
                TotalValueToday = await rentOrders.SumAsync(o => (decimal?)o.TotalFee) ?? 0
            };
        }

        public async Task<MonthlyRentBookStatisticsDto> GetMonthlyRentBookStatisticsAsync()
        {
            if (!_cache.TryGetValue("MonthlyRentDate", out (int year, int month) selectedDate))
            {
                selectedDate = (DateTime.Today.Year, DateTime.Today.Month);
            }

            var rentOrders = await _context.RentOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled &&
                            o.OrderDate.Year == selectedDate.year &&
                            o.OrderDate.Month == selectedDate.month)
                .ToListAsync();

            var createdDates = rentOrders
                .Select(o => o.OrderDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return new MonthlyRentBookStatisticsDto
            {
                CreatedDates = createdDates,
                OrdersThisMonth = rentOrders.Count,
                TotalValueThisMonth = rentOrders.Sum(o => o.TotalFee)
            };
        }

        public async Task<YearlyRentBookStatisticsDto> GetYearlyRentBookStatisticsAsync()
        {
            if (!_cache.TryGetValue("YearlyRentDate", out int year))
            {
                year = DateTime.Today.Year;
            }

            var rentOrders = await _context.RentOrders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Canceled && o.OrderDate.Year == year)
                .ToListAsync();

            var createdDates = rentOrders
                .Select(o => o.OrderDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return new YearlyRentBookStatisticsDto
            {
                CreatedDates = createdDates,
                OrdersThisYear = rentOrders.Count,
                TotalValueThisYear = rentOrders.Sum(o => o.TotalFee)
            };
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
            var now = DateTime.Today;
            if (year > now.Year || (year == now.Year && month > now.Month))
                throw new ArgumentException("Tháng hoặc năm không hợp lệ. Vui lòng nhập lại");

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
            if (year > DateTime.Today.Year)
                throw new ArgumentException("Năm không hợp lệ vui lòng nhập lại.");

            return await _context.RentOrders
                .Include(o => o.RentOrderDetails)
                .ThenInclude(d => d.RentBookItem)
                .ThenInclude(item => item.RentBook)
                .Where(o => o.Status != OrderStatus.Canceled &&
                            o.OrderDate.Year == year)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
