using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using System.Threading.Tasks;
using ShopThueBanSach.Server.Area.Admin.Model.Request;
namespace ShopThueBanSach.Server.Area.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /* // Tổng quan số lượng sách & tổng giá trị tồn
         [HttpGet("overview")]
         public async Task<IActionResult> GetOverview()
         {
             var result = await _reportService.GetOverviewStatisticsAsync();
             return Ok(result);
         }*/

        // Tổng hợp đơn hàng Sale
        [HttpGet]
        public async Task<IActionResult> GetOrdersSummary()
        {
            var result = await _reportService.GetOrderSummaryAsync();
            return Ok(result);
        }

        // Thống kê Sale theo ngày
        [HttpGet("sale/daily")]
        public async Task<IActionResult> GetDailySaleStatistics()
        {
            var result = await _reportService.GetDailySaleBookStatisticsAsync();
            return Ok(result);
        }
        [HttpPost("sale/orders/by-date")]
        public async Task<IActionResult> PostOrdersByDate([FromBody] SaleOrderByDateRequest request)
        {
            var result = await _reportService.GetSaleOrdersByDateAsync(request.Date);
            return Ok(result);
        }
        // Thống kê Sale theo tháng
        [HttpGet("sale/monthly")]
        public async Task<IActionResult> GetMonthlySaleStatistics()
        {
            var result = await _reportService.GetMonthlySaleBookStatisticsAsync();
            return Ok(result);
        }
        [HttpPost("sale/orders/by-month")]
        public async Task<IActionResult> PostOrdersByMonth([FromBody] SaleOrderByMonthRequest request)
        {
            var result = await _reportService.GetSaleOrdersByMonthAsync(request.Year, request.Month);
            return Ok(result);
        }
        // Thống kê Sale theo năm
        [HttpGet("sale/yearly")]
        public async Task<IActionResult> GetYearlySaleStatistics()
        {
            var result = await _reportService.GetYearlySaleBookStatisticsAsync();
            return Ok(result);
        }

        [HttpPost("sale/orders/by-year")]
        public async Task<IActionResult> PostOrdersByYear([FromBody] SaleOrderByYearRequest request)
        {
            var result = await _reportService.GetSaleOrdersByYearAsync(request.Year);
            return Ok(result);
        }
        // Thống kê Rent theo ngày
        [HttpGet("rent/daily")]
        public async Task<IActionResult> GetDailyRentStatistics()
        {
            var result = await _reportService.GetDailyRentBookStatisticsAsync();
            return Ok(result);
        }
        [HttpPost("rent-orders-by-date")]
        public async Task<IActionResult> GetRentOrdersByDate([FromBody] RentOrderByDateRequest request)
        {
            var result = await _reportService.GetRentOrdersByDateAsync(request.Date);
            return Ok(result);
        }
        // Thống kê Rent theo tháng
        [HttpGet("rent/monthly")]
        public async Task<IActionResult> GetMonthlyRentStatistics()
        {
            var result = await _reportService.GetMonthlyRentBookStatisticsAsync();
            return Ok(result);
        }

        [HttpPost("rent-orders-by-month")]
        public async Task<IActionResult> GetRentOrdersByMonth([FromBody] RentOrderByMonthRequest request)
        {
            var result = await _reportService.GetRentOrdersByMonthAsync(request.Year, request.Month);
            return Ok(result);
        }
        // Thống kê Rent theo năm
        [HttpGet("rent/yearly")]
        public async Task<IActionResult> GetYearlyRentStatistics()
        {
            var result = await _reportService.GetYearlyRentBookStatisticsAsync();
            return Ok(result);
        }


        [HttpPost("rent-orders-by-year")]
        public async Task<IActionResult> GetRentOrdersByYear([FromBody] RentOrderByYearRequest request)
        {
            var result = await _reportService.GetRentOrdersByYearAsync(request.Year);
            return Ok(result);
        }







    }
}
