using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleBooksController : ControllerBase
    {
        private readonly ISaleBookService _service;
        private readonly IActivityNotificationService _notificationService;
        private readonly IStaffService _staffService;

        public SaleBooksController(
            ISaleBookService service,
            IActivityNotificationService notificationService,
            IStaffService staffService)
        {
            _service = service;
            _notificationService = notificationService;
            _staffService = staffService;
        }

        private int? GetCurrentStaffId()
        {
            var claim = User.FindFirst("StaffId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        private async Task CreateNotificationIfStaffExistsAsync(string description)
        {
            var staffId = GetCurrentStaffId();
            if (staffId.HasValue && await _staffService.ExistsAsync(staffId.Value))
            {
                await _notificationService.CreateNotificationAsync(staffId.Value, description);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var saleBook = await _service.GetByIdAsync(id);
            return saleBook == null ? NotFound() : Ok(saleBook);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSaleBookDto dto)
        {
            var id = await _service.CreateAsync(dto);
            await CreateNotificationIfStaffExistsAsync($"Thêm sách bán: {dto.Title}");
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SaleBookDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (success)
                await CreateNotificationIfStaffExistsAsync($"Cập nhật sách bán: {dto.Title}");

            return success ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (success)
                await CreateNotificationIfStaffExistsAsync($"Xóa sách bán: {id}");

            return success ? NoContent() : NotFound();
        }

        [HttpPut("set-visibility/{id}/{isHidden}")]
        public async Task<IActionResult> SetVisibility(string id, int isHidden)
        {
            if (isHidden != 0 && isHidden != 1)
                return BadRequest("isHidden phải là 0 hoặc 1");

            var success = await _service.SetVisibilityAsync(id, isHidden == 1);
            if (success)
                await CreateNotificationIfStaffExistsAsync($"Thay đổi hiển thị sách bán: {id} -> {(isHidden == 1 ? "ẩn" : "hiện")}");

            return success ? Ok() : NotFound();
        }
    }
}
