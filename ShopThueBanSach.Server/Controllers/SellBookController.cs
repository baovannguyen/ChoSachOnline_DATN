using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Models.SellBookModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellBookController : ControllerBase
    {
        private readonly ISellBookService _service;

        public SellBookController(ISellBookService service)
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
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(SellBookDto model)
        {
            var entity = new SellBook
            {
                Title = model.Title,
                Description = model.Description,
                AuthorId = model.AuthorId,
                CategoryId = model.CategoryId,
                Publisher = model.Publisher,
                PageCount = model.PageCount,
                Translator = model.Translator,
                PackageSize = model.PackageSize,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                Quantity = model.Quantity,
                DiscountId = model.DiscountId
            };

            var created = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.SellBookId }, created);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SellBook model)
        {
            if (id != model.SellBookId)
                return BadRequest();

            var success = await _service.UpdateAsync(model);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
