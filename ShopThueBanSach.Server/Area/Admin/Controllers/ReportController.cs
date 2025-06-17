using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;

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

        [HttpGet("book-statistics")]
        public IActionResult GetBookStatistics()
        {
            var result = _reportService.GetBookStatistics();
            return Ok(result);
        }
    }

}