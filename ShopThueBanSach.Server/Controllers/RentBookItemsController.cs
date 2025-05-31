using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentBookItemController : ControllerBase
    {
        private readonly IRentBookItemService _rentBookItemService;

        public RentBookItemController(IRentBookItemService rentBookItemService)
        {
            _rentBookItemService = rentBookItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _rentBookItemService.GetAllAsync();
            return Ok(list.Select(x => new
            {
                x.RentBookItemId,
                x.RentBookId,
                x.RentBookTitle,
                status = x.Status.ToString(), // Convert to string
                x.Condition,
                x.IsHidden
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _rentBookItemService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(new
            {
                item.RentBookItemId,
                item.RentBookId,
                item.RentBookTitle,
                status = item.Status.ToString(),
                item.Condition,
                item.IsHidden
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RentBookItemDto dto)
        {
            if (dto == null) return BadRequest("DTO is null");
            dto.RentBookItemId = null;

            var created = await _rentBookItemService.CreateAsync(dto);
            if (created == null)
                return BadRequest("Condition must be between 0 and 100");

            return CreatedAtAction(nameof(GetById), new { id = created.RentBookItemId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RentBookItemDto dto)
        {
            if (dto == null) return BadRequest("DTO is null");

            var updated = await _rentBookItemService.UpdateAsync(id, dto);
            if (updated == null)
                return BadRequest("Update failed. Check if ID exists or condition is valid (0–100).");

            return Ok(updated);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _rentBookItemService.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

    }
}
