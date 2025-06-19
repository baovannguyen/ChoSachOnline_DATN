using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;
using ShopThueBanSach.Server.Data;

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

        /* ------------------ Helper lấy StaffId ------------------ */
        private async Task<string?> GetCurrentStaffIdAsync()
        {
            // Giả định ClaimTypes.Name chứa email
            var userEmail = _httpContextAccessor.HttpContext?.User?
                                         .FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userEmail)) return null;

            return await _dbContext.Staffs
                                   .Where(s => s.Email == userEmail && s.Role == "Staff")
                                   .Select(s => s.StaffId)
                                   .FirstOrDefaultAsync();
        }

        private async Task NotifyAsync(string description)
        {
            var staffId = await GetCurrentStaffIdAsync();
            if (!string.IsNullOrEmpty(staffId))
            {
                await _notificationService.CreateNotificationAsync(staffId, description);
            }
        }
        /* -------------------------------------------------------- */

        /* ----------------------- CRUD --------------------------- */
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
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            var result = await _service.CreateAsync(dto);
            await NotifyAsync($"🟢 Thêm thể loại mới: {dto.CategoryName}");

            return CreatedAtAction(nameof(GetById), new { id = result!.CategoryId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, CategoryDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result == null) return NotFound();

            await NotifyAsync($"🟡 Cập nhật thể loại: {dto.CategoryName} (ID: {id})");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();

            await NotifyAsync($"🔴 Xóa thể loại: {id}");
            return NoContent();
        }
        /* -------------------------------------------------------- */
    }
}
