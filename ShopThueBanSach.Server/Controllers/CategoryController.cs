using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly IActivityNotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryController(
            ICategoryService service,
            IActivityNotificationService notificationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
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
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var result = await _service.CreateAsync(dto);

            // ⚠ Tạm gán StaffId thủ công
            int staffId = 1; // hoặc lấy từ token nếu có
            var description = $"Staff added new category: {dto.CategoryName}";
            await _notificationService.CreateNotificationAsync(staffId, description);

            return CreatedAtAction(nameof(GetById), new { id = result!.CategoryId }, result);
        }

        /*[HttpPost]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var result = await _service.CreateAsync(dto);

            // 🔔 Tạo thông báo – gán userId tạm thời để kiểm tra
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Nếu không có userId thì gán thủ công để test
            if (string.IsNullOrEmpty(userId))
            {
                userId = "test-user-id"; // 👈 Gán tạm userId (bạn có thể dùng ID thực từ bảng User)
            }

            var description = $"User added new category: {dto.CategoryName}";
            await _notificationService.CreateNotificationAsync(userId, description);
            Console.WriteLine($"[Thông báo] Đã tạo thông báo: {description} (userId = {userId})");

            return CreatedAtAction(nameof(GetById), new { id = result!.CategoryId }, result);
        }
*/

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, CategoryDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
