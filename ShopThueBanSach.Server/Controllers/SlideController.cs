using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.SlideModel;
using ShopThueBanSach.Server.Services.Interfaces;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlideController : ControllerBase
    {
        private readonly ISlideService _service;
        private readonly IActivityNotificationService _notificationService;
        private readonly IStaffService _staffService;

        public SlideController(
            ISlideService service,
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

        private async Task CreateNotificationIfValidAsync(string description)
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
        public async Task<IActionResult> GetById(int id)
        {
            var slide = await _service.GetByIdAsync(id);
            return slide == null ? NotFound() : Ok(slide);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SlideDto dto)
        {
            var result = await _service.CreateAsync(dto);
            await CreateNotificationIfValidAsync($"Thêm slide mới (Image: {dto.ImageUrl})");
            return CreatedAtAction(nameof(GetById), new { id = result.SlideId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SlideDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result != null)
                await CreateNotificationIfValidAsync($"Cập nhật slide ID {id} (Image: {dto.ImageUrl})");

            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (success)
                await CreateNotificationIfValidAsync($"Xóa slide ID {id}");

            return success ? NoContent() : NotFound();
        }

    }
}
