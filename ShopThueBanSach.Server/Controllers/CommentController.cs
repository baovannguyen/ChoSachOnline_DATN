using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.CommentModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService service)
        {
            _service = service;
        }

        // Lấy toàn bộ bình luận của 1 sách
        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetAll(string bookId)
        {
            var comments = await _service.GetAllAsync(bookId);
            return Ok(comments);
        }

        // Lấy bình luận theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var comment = await _service.GetByIdAsync(id);
            return comment == null ? NotFound() : Ok(comment);
        }

        // Thêm bình luận mới — bắt buộc đăng nhập
        [HttpPost]
        [Authorize] // ✅ đảm bảo đã đăng nhập mới được bình luận
        public async Task<IActionResult> Create([FromBody] CommentDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto); // ✅ không cần userId
                return CreatedAtAction(nameof(GetById), new { id = created.CommentId }, created);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Không xác định được người dùng.");
            }
        }

        // Xoá bình luận
        [HttpDelete("{id}")]
        [Authorize] // ✅ có thể giới hạn chỉ admin hoặc người tạo được xóa
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
