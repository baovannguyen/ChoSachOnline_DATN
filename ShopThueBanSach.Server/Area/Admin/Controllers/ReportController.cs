using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using System.Threading.Tasks;

namespace ShopThueBanSach.Server.Area.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // Tổng hợp tất cả thống kê
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _reportService.GetBookStatisticsAsync();
            return Ok(result);
        }

        // Tổng quan
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var result = await _reportService.GetOverviewStatisticsAsync();
            return Ok(result);
        }

        // Theo ngày
        [HttpGet("daily")]
        public async Task<IActionResult> GetDaily()
        {
            var result = await _reportService.GetDailyStatisticsAsync();
            return Ok(result);
        }

        // Theo tuần
        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeekly()
        {
            var result = await _reportService.GetWeeklyStatisticsAsync();
            return Ok(result);
        }

        // Theo tháng
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthly()
        {
            var result = await _reportService.GetMonthlyStatisticsAsync();
            return Ok(result);
        }
    }
}
