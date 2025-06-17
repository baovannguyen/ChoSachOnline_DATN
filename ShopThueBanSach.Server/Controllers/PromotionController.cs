using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _service;

        public PromotionController(IPromotionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllPromotionsAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetPromotionByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PromotionDTO model)
        {
            var result = await _service.CreatePromotionAsync(model);
            return result ? Ok() : BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, PromotionDTO model)
        {
            var result = await _service.UpdatePromotionAsync(id, model);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeletePromotionAsync(id);
            return result ? Ok() : NotFound();
        }
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyPromotionToBooks([FromBody] ApplyPromotionDTO dto)
        {
            var result = await _service.ApplyPromotionToBooksAsync(dto);
            return result
                ? Ok(new { message = "Áp dụng khuyến mãi thành công!" })
                : BadRequest(new { message = "Không thể áp dụng khuyến mãi." });
        }
    }
}
