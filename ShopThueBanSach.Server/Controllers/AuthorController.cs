using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly IActivityNotificationService _notificationService;
        private readonly IStaffService _staffService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthorController(
            IAuthorService authorService,
            IActivityNotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            IStaffService staffService)
        {
            _authorService = authorService;
            _notificationService = notificationService;
            _staffService = staffService;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<string?> GetCurrentStaffIdAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            return email != null ? await _staffService.GetStaffIdByEmailAsync(email) : null;
        }

        private async Task CreateNotificationIfValidAsync(string description)
        {
            var staffId = await GetCurrentStaffIdAsync();
            if (!string.IsNullOrEmpty(staffId) && await _staffService.ExistsAsync(staffId))
            {
                await _notificationService.CreateNotificationAsync(staffId, description);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuthorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra nếu người dùng nhập "string" cho Name hoặc Description
            if (dto.Name?.Trim().ToLower() == "string" || dto.Description?.Trim().ToLower() == "string")
            {
                return BadRequest(new
                {
                    message = "Tên tác giả và mô tả không được để là 'string'. Vui lòng nhập nội dung hợp lệ."
                });
            }

            var created = await _authorService.CreateAsync(dto);
            await CreateNotificationIfValidAsync($"Thêm tác giả: {dto.Name}");
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AuthorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra nếu người dùng nhập "string" cho Name hoặc Description
            if (dto.Name?.Trim().ToLower() == "string" || dto.Description?.Trim().ToLower() == "string")
            {
                return BadRequest(new
                {
                    message = "Tên tác giả và mô tả không được để là 'trống'. Vui lòng nhập nội dung hợp lệ."
                });
            }

            var updated = await _authorService.UpdateAsync(id, dto);
            if (updated != null)
                await CreateNotificationIfValidAsync($"Cập nhật tác giả: {dto.Name}");

            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _authorService.DeleteAsync(id);
            if (result)
                await CreateNotificationIfValidAsync($"Xóa tác giả: {id}");
            return result ? Ok() : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _authorService.GetAllAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var author = await _authorService.GetByIdAsync(id);
            return author == null ? NotFound() : Ok(author);
        }
    }
}
