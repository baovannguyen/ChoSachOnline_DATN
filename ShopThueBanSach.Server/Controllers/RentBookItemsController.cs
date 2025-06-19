using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentBookItemController : ControllerBase
    {
        private readonly IRentBookItemService _rentBookItemService;
        private readonly IActivityNotificationService _notificationService;
        private readonly IStaffService _staffService;

        public RentBookItemController(
            IRentBookItemService rentBookItemService,
            IActivityNotificationService notificationService,
            IStaffService staffService)
        {
            _rentBookItemService = rentBookItemService;
            _notificationService = notificationService;
            _staffService = staffService;
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
        private int? GetCurrentStaffId()
        {
            var claim = User.FindFirst("StaffId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        private async Task CreateNotificationIfStaffExistsAsync(string description)
        {
            var staffId = GetCurrentStaffId();
            if (staffId.HasValue && await _staffService.ExistsAsync(staffId.Value))
            {
                await _notificationService.CreateNotificationAsync(staffId.Value, description);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _rentBookItemService.GetByIdAsync(id);
            if (item == null) return NotFound();

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Condition < 80 || dto.Condition > 100)
                return BadRequest(new { message = "Tình trạng sách phải từ 80 đến 100 mới có thể cho thuê." });

            dto.RentBookItemId = null;

            try
            {
                var created = await _rentBookItemService.CreateAsync(dto);
                if (created == null)
                    return BadRequest("Không thể tạo RentBookItem.");

                return CreatedAtAction(nameof(GetById), new { id = created.RentBookItemId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RentBookItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Condition < 80 || dto.Condition > 100)
                return BadRequest(new { message = "Tình trạng sách phải từ 80 đến 100 mới có thể cho thuê." });

            var updated = await _rentBookItemService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = "Không tìm thấy hoặc không thể cập nhật RentBookItem." });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _rentBookItemService.DeleteAsync(id);
            if (success)
                await CreateNotificationIfStaffExistsAsync($"Xóa bản sao sách thuê: {id}");

            return success ? NoContent() : NotFound();
        }
    }
}
