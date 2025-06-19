using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;
using ShopThueBanSach.Server.Data; // <-- Add
using Microsoft.EntityFrameworkCore; // <-- Add

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly IActivityNotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDBContext _dbContext;

        public CategoryController(
            ICategoryService service,
            IActivityNotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            AppDBContext dbContext)
        {
            _service = service;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.CategoryName?.Trim().ToLower() == "string" || dto.Description?.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Tên thể loại và mô tả không được để là 'string'." });
            }

            // ❗ Kiểm tra trùng tên
            bool isDuplicate = await _dbContext.Categories
    .AnyAsync(c => c.Name.ToLower() == dto.CategoryName.Trim().ToLower());

            if (isDuplicate)
            {
                return BadRequest(new { message = "Tên thể loại đã tồn tại. Vui lòng nhập tên khác." });
            }

            var result = await _service.CreateAsync(dto);

            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

            int? staffId = null;
            if (!string.IsNullOrEmpty(userEmail))
            {
                var staff = await _dbContext.Staffs.FirstOrDefaultAsync(s => s.Email == userEmail);
                if (staff != null)
                {
                    staffId = staff.StaffId;
                }
            }

            if (staffId != null)
            {
                var description = $"Staff added new category: {dto.CategoryName}";
                await _notificationService.CreateNotificationAsync(staffId.Value, description);
            }
            else
            {
                Console.WriteLine("⚠ Không tìm thấy Staff tương ứng với email hiện tại.");
            }

            return CreatedAtAction(nameof(GetById), new { id = result!.CategoryId }, result);
        }

        // ... các phương thức khác giữ nguyên
    
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
