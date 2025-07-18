using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
/*using iText.Kernel.Pdf;*/
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
using System.Reflection.Metadata;
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
		public async Task<byte[]> ExportSaleReportToExcelAsync(DateTime? fromDate, DateTime? toDate)
		{
			// Kiểm tra ngày nhập vào
			if (fromDate.HasValue && toDate.HasValue)
			{
				if (fromDate.Value > toDate.Value)
				{
					throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc.");
				}

				if (fromDate.Value == DateTime.Today || toDate.Value == DateTime.Today)
				{
					throw new ArgumentException("Ngày không hợp lệ.");
				}
			}

			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("Báo cáo bán sách");

			// Header
			worksheet.Cell(1, 1).Value = "STT";
			worksheet.Cell(1, 2).Value = "Mã đơn hàng";
			worksheet.Cell(1, 3).Value = "Ngày đặt";
			worksheet.Cell(1, 4).Value = "Khách hàng";
			worksheet.Cell(1, 5).Value = "Số lượng sách";
			worksheet.Cell(1, 6).Value = "Tổng tiền";
			worksheet.Cell(1, 7).Value = "Phí vận chuyển"; // Thêm cột phí vận chuyển
			worksheet.Cell(1, 8).Value = "Giảm giá"; // Thêm cột giảm giá

			// Ghi thời gian xuất file
			worksheet.Cell(2, 1).Value = $"Thời gian xuất file: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
			worksheet.Range(2, 1, 2, 8).Merge(); // Gộp ô từ A2 đến H2
			worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Canh giữa

			// Query data
			var query = _context.SaleOrders
				.Include(o => o.SaleOrderDetails)
				.Where(o => o.Status != OrderStatus.Canceled);

			if (fromDate.HasValue)
				query = query.Where(o => o.OrderDate >= fromDate.Value);

			if (toDate.HasValue)
				query = query.Where(o => o.OrderDate <= toDate.Value);

			var orders = await query.ToListAsync();

			// Fill data
			for (int i = 0; i < orders.Count; i++)
			{
				var row = i + 3; // Bắt đầu từ hàng 3
				var order = orders[i];

				worksheet.Cell(row, 1).Value = i + 1;
				worksheet.Cell(row, 2).Value = order.OrderId; // Sử dụng OrderId
				worksheet.Cell(row, 3).Value = order.OrderDate.ToString("dd/MM/yyyy");
				worksheet.Cell(row, 4).Value = order.UserId; // Giả sử UserId là tên khách hàng
				worksheet.Cell(row, 5).Value = order.SaleOrderDetails.Sum(d => d.Quantity);
				worksheet.Cell(row, 6).Value = order.TotalAmount;
				worksheet.Cell(row, 7).Value = order.HasShippingFee ? order.ShippingFee : 0; // Phí vận chuyển
				worksheet.Cell(row, 8).Value = order.DiscountAmount; // Giảm giá
			}

			// Format currency
			worksheet.Column(6).Style.NumberFormat.Format = "#,##0";
			worksheet.Column(7).Style.NumberFormat.Format = "#,##0"; // Định dạng phí vận chuyển
			worksheet.Column(8).Style.NumberFormat.Format = "#,##0"; // Định dạng giảm giá

			// Auto fit columns
			worksheet.Columns().AdjustToContents();

			// Save to memory stream
			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			return stream.ToArray();
		}


		public async Task<byte[]> ExportRentReportToExcelAsync(DateTime? fromDate, DateTime? toDate)
		{
			if (fromDate.HasValue && toDate.HasValue)
			{
				if (fromDate.Value > toDate.Value)
				{
					throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc.");
				}

				if (fromDate.Value == DateTime.Today || toDate.Value == DateTime.Today)
				{
					throw new ArgumentException("Ngày không hợp lệ.");
				}
			}
			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("Báo cáo thuê sách");

			// Header
			worksheet.Cell(1, 1).Value = "STT";
			worksheet.Cell(1, 2).Value = "Mã đơn hàng";
			worksheet.Cell(1, 3).Value = "Ngày đặt";
			worksheet.Cell(1, 4).Value = "Khách hàng";
			worksheet.Cell(1, 5).Value = "Số lượng sách";
			worksheet.Cell(1, 6).Value = "Tổng tiền";
			worksheet.Cell(1, 7).Value = "Phí vận chuyển"; // Thêm cột phí vận chuyển

			// Ghi thời gian xuất file
			worksheet.Cell(2, 1).Value = $"Thời gian xuất file: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
			worksheet.Range(2, 1, 2, 7).Merge(); // Gộp ô từ A2 đến G2
			worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Canh giữa

			// Query data
			var query = _context.RentOrders
				.Include(o => o.RentOrderDetails)
				.Where(o => o.Status != OrderStatus.Canceled);

			if (fromDate.HasValue)
				query = query.Where(o => o.OrderDate >= fromDate.Value);

			if (toDate.HasValue)
				query = query.Where(o => o.OrderDate <= toDate.Value);

			var orders = await query.ToListAsync();

			// Fill data
			for (int i = 0; i < orders.Count; i++)
			{
				var row = i + 3; // Bắt đầu từ hàng 3
				var order = orders[i];

				worksheet.Cell(row, 1).Value = i + 1;
				worksheet.Cell(row, 2).Value = order.OrderId; // Sử dụng OrderId
				worksheet.Cell(row, 3).Value = order.OrderDate.ToString("dd/MM/yyyy");
				worksheet.Cell(row, 4).Value = order.UserId; // Giả sử UserId là tên khách hàng
				worksheet.Cell(row, 5).Value = order.RentOrderDetails.Count; // Số lượng sách
				worksheet.Cell(row, 6).Value = order.TotalFee; // Tổng tiền
				worksheet.Cell(row, 7).Value = order.HasShippingFee ? order.ShippingFee : 0; // Phí vận chuyển
			}

			// Format currency
			worksheet.Column(6).Style.NumberFormat.Format = "#,##0"; // Định dạng tổng tiền
			worksheet.Column(7).Style.NumberFormat.Format = "#,##0"; // Định dạng phí vận chuyển

			// Auto fit columns
			worksheet.Columns().AdjustToContents();

			// Save to memory stream
			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			return stream.ToArray();
		}

		/*  public async Task<byte[]> ExportSaleReportToPdfAsync(DateTime? fromDate, DateTime? toDate)
		  {
			  // Kiểm tra ngày nhập vào
			  if (fromDate.HasValue && toDate.HasValue)
			  {
				  if (fromDate.Value > toDate.Value)
				  {
					  throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc.");
				  }

				  if (fromDate.Value > DateTime.Today || toDate.Value > DateTime.Today)
				  {
					  throw new ArgumentException("Ngày không được nằm trong tương lai.");
				  }
			  }

			  using var memoryStream = new MemoryStream();
			  using (var writer = new PdfWriter(memoryStream))
			  using (var pdf = new PdfDocument(writer))
			  {
				  var document = new Document(pdf);
				  document.Add(new Paragraph("Báo cáo bán sách")
					  .SetTextAlignment(TextAlignment.CENTER)
					  .SetFontSize(20));
  // Ghi thời gian xuất file
				  document.Add(new Paragraph($"Thời gian xuất file: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
					  .SetTextAlignment(TextAlignment.CENTER));

				  // Header
				  var table = new Table(8);
				  table.AddHeaderCell("STT");
				  table.AddHeaderCell("Mã đơn hàng");
				  table.AddHeaderCell("Ngày đặt");
				  table.AddHeaderCell("Khách hàng");
				  table.AddHeaderCell("Số lượng sách");
				  table.AddHeaderCell("Tổng tiền");
				  table.AddHeaderCell("Phí vận chuyển");
				  table.AddHeaderCell("Giảm giá");

				  // Query data
				  var query = _context.SaleOrders
					  .Include(o => o.SaleOrderDetails)
					  .Where(o => o.Status != OrderStatus.Canceled);

				  if (fromDate.HasValue)
					  query = query.Where(o => o.OrderDate >= fromDate.Value);

				  if (toDate.HasValue)
					  query = query.Where(o => o.OrderDate <= toDate.Value);

				  var orders = await query.ToListAsync();

				  // Fill data
				  for (int i = 0; i < orders.Count; i++)
				  {
					  var order = orders[i];
					  table.AddCell((i + 1).ToString());
					  table.AddCell(order.OrderId);
					  table.AddCell(order.OrderDate.ToString("dd/MM/yyyy"));
					  table.AddCell(order.UserId); // Giả sử UserId là tên khách hàng
					  table.AddCell(order.SaleOrderDetails.Sum(d => d.Quantity).ToString());
					  table.AddCell(order.TotalAmount.ToString("N0"));
					  table.AddCell(order.HasShippingFee ? order.ShippingFee.ToString("N0") : "0");
					  table.AddCell(order.DiscountAmount.ToString("N0"));
				  }

				  document.Add(table);
				  document.Close();
			  }

			  return memoryStream.ToArray();
		  }
		  public async Task<byte[]> ExportRentReportToPdfAsync(DateTime? fromDate, DateTime? toDate)
  {
	  // Kiểm tra ngày nhập vào
	  if (fromDate.HasValue && toDate.HasValue)
	  {
		  if (fromDate.Value > toDate.Value)
		  {
			  throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc.");
		  }

		  if (fromDate.Value > DateTime.Today || toDate.Value > DateTime.Today)
		  {
			  throw new ArgumentException("Ngày không được nằm trong tương lai.");
		  }
	  }

	  using var memoryStream = new MemoryStream();
	  using (var writer = new PdfWriter(memoryStream))
	  using (var pdf = new PdfDocument(writer))
	  {
		  var document = new Document(pdf);
		  document.Add(new Paragraph("Báo cáo thuê sách")
			  .SetTextAlignment(TextAlignment.CENTER)
			  .SetFontSize(20));

		  // Ghi thời gian xuất file
  document.Add(new Paragraph($"Thời gian xuất file: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
			  .SetTextAlignment(TextAlignment.CENTER));

		  // Header
		  var table = new Table(7);
		  table.AddHeaderCell("STT");
		  table.AddHeaderCell("Mã đơn hàng");
		  table.AddHeaderCell("Ngày đặt");
		  table.AddHeaderCell("Khách hàng");
		  table.AddHeaderCell("Số lượng sách");
		  table.AddHeaderCell("Tổng tiền");
		  table.AddHeaderCell("Phí vận chuyển");

		  // Query data
		  var query = _context.RentOrders
			  .Include(o => o.RentOrderDetails)
			  .Where(o => o.Status != OrderStatus.Canceled);

		  if (fromDate.HasValue)
			  query = query.Where(o => o.OrderDate >= fromDate.Value);

		  if (toDate.HasValue)
			  query = query.Where(o => o.OrderDate <= toDate.Value);

		  var orders = await query.ToListAsync();

		  // Fill data
		  for (int i = 0; i < orders.Count; i++)
		  {
			  var order = orders[i];
			  table.AddCell((i + 1).ToString());
			  table.AddCell(order.OrderId);
			  table.AddCell(order.OrderDate.ToString("dd/MM/yyyy"));
			  table.AddCell(order.UserId); // Giả sử UserId là tên khách hàng
			  table.AddCell(order.RentOrderDetails.Count.ToString());
			  table.AddCell(order.TotalFee.ToString("N0"));
			  table.AddCell(order.HasShippingFee ? order.ShippingFee.ToString("N0") : "0");
		  }

		  document.Add(table);
		  document.Close();
	  }

	  return memoryStream.ToArray();
  }*/

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