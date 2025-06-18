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

            var staffId = await GetCurrentStaffIdAsync();
            if (staffId != null)
            {
                var description = $"🟢 Staff đã thêm 1 thể loại mới: {dto.CategoryName}";
                await _notificationService.CreateNotificationAsync(staffId.Value, description);
            }

            return CreatedAtAction(nameof(GetById), new { id = result!.CategoryId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, CategoryDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result == null) return NotFound();

            var staffId = await GetCurrentStaffIdAsync();
            if (staffId != null)
            {
                var description = $"🟡 Staff đã cập nhật thể loại: {dto.CategoryName} (ID: {id})";
                await _notificationService.CreateNotificationAsync(staffId.Value, description);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();

            var staffId = await GetCurrentStaffIdAsync();
            if (staffId != null)
            {
                var description = $"🔴 Staff đã sửa thể loại : {id}";
                await _notificationService.CreateNotificationAsync(staffId.Value, description);
            }

            return NoContent();
        }

        private async Task<int?> GetCurrentStaffIdAsync()
        {
            // ✅ Sử dụng ClaimTypes.Name thay vì Email
            var userEmail = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

            Console.WriteLine($"🔍 Email from token: {userEmail}");

            if (!string.IsNullOrEmpty(userEmail))
            {
                // 🔍 Đảm bảo email trong bảng Staff là duy nhất
                var staff = await _dbContext.Staffs.FirstOrDefaultAsync(s => s.Email == userEmail);
                if (staff != null)
                {
                    Console.WriteLine($"✅ Found StaffId: {staff.StaffId} for email {userEmail}");
                    return staff.StaffId;
                }
            }

            Console.WriteLine("⚠ Không tìm thấy Staff tương ứng với email hiện tại.");
            return null;
        }

    }
}
