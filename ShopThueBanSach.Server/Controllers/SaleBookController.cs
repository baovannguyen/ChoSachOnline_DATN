using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleBooksController : ControllerBase
    {
        private readonly ISaleBookService _service;

        public SaleBooksController(ISaleBookService service)
        {
            _service = service;
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
        public async Task<IActionResult> Create([FromBody] CreateSaleBookDto dto)
        {
            var id = await _service.CreateAsync(dto);
            var result = await _service.GetByIdAsync(id); // ✅ Lấy lại dữ liệu đã tạo
            return CreatedAtAction(nameof(GetById), new { id }, result); // ✅ Trả về SaleBookDto
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SaleBookDto dto)
        {
            if (dto == null) return BadRequest();

            // Gán id từ route vào service, dto không chứa id
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound();

            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPut("set-visibility/{id}/{isHidden}")]
        public async Task<IActionResult> SetVisibility(string id, int isHidden)
        {
            if (isHidden != 0 && isHidden != 1) return BadRequest("isHidden must be 0 (false) or 1 (true)");
            var success = await _service.SetVisibilityAsync(id, isHidden == 1);
            if (!success) return NotFound();
            return Ok();
        }
    }
}
