using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Models.SlideModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlideController : ControllerBase
    {
        private readonly ISlideService _service;
        private readonly IActivityNotificationService _notificationService;
        private readonly IStaffService _staffService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SlideController(
            ISlideService service,
            IActivityNotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            IStaffService staffService)
        {
            _service = service;
            _notificationService = notificationService;
            _staffService = staffService;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ Lấy StaffId theo kiểu string
        private async Task<string?> GetCurrentStaffIdAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            return string.IsNullOrEmpty(email)
                ? null
                : await _staffService.GetStaffIdByEmailAsync(email);
        }

        private async Task CreateNotificationIfValidAsync(string description)
        {
            var staffId = await GetCurrentStaffIdAsync();
            if (!string.IsNullOrEmpty(staffId) && await _staffService.ExistsAsync(staffId))
            {
                await _notificationService.CreateNotificationAsync(staffId, description);
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
