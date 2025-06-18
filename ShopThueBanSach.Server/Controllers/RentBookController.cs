using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentBooksController : ControllerBase
    {
        private readonly IRentBookService _service;
        private readonly IActivityNotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStaffService _staffService;

        public RentBooksController(
            IRentBookService service,
            IActivityNotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            IStaffService staffService)
        {
            _service = service;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _staffService = staffService;
        }
         // 🔍 Lấy StaffId từ JWT Claims
        private int? GetCurrentStaffId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            return _staffService.GetStaffIdByEmail(claim); // bạn cần cài hàm này
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
            var rentBook = await _service.GetByIdAsync(id);
            return rentBook == null ? NotFound() : Ok(rentBook);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRentBookDto dto)
        {
            var id = await _service.CreateAsync(dto);
            await CreateNotificationIfStaffExistsAsync($"Thêm sách thuê: {dto.Title}");
            return CreatedAtAction(nameof(GetById), new { id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RentBookDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result)
                await CreateNotificationIfStaffExistsAsync($"Cập nhật sách thuê: {dto.Title} (ID: {id})");
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteAsync(id);
            if (result)
                await CreateNotificationIfStaffExistsAsync($"🔴 Xóa sách thuê: ID {id}");
            return result ? NoContent() : NotFound();
        }
        [HttpPut("set-visibility/{id}/{isHidden}")]
        public async Task<IActionResult> SetVisibility(string id, int isHidden)
        {
            var result = await _service.SetVisibilityAsync(id, isHidden == 1);
            if (result)
                await CreateNotificationIfStaffExistsAsync($"🔁 Cập nhật hiển thị sách thuê: {id} -> {(isHidden == 1 ? "ẩn" : "hiện")}");
            return result ? NoContent() : NotFound();
        }
    }
}
