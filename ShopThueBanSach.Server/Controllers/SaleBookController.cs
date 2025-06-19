using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateSaleBookDto dto)

        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Title?.Trim().ToLower() == "string" || dto.Publisher?.Trim().ToLower() == "string")
                return BadRequest(new { message = "Tiêu đề và nhà xuất bản không được để là 'string'." });

            if (await _service.CheckTitleExistsAsync(dto.Title))
                return BadRequest(new { message = "Tiêu đề sách đã tồn tại." });

            var id = await _service.CreateAsync(dto);
            var result = await _service.GetByIdAsync(id);
            return CreatedAtAction(nameof(GetById), new { id }, result);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] UpdateSaleBookDto dto)

        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Title?.Trim().ToLower() == "string" || dto.Publisher?.Trim().ToLower() == "string")
                return BadRequest(new { message = "Tiêu đề và nhà xuất bản không được để là 'string'." });

            if (await _service.CheckTitleExistsAsync(dto.Title, id))
                return BadRequest(new { message = "Tiêu đề sách đã tồn tại." });

            var success = await _service.UpdateAsync(id, dto);
            return success ? Ok() : NotFound();
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
