using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;
using ShopThueBanSach.Server.Models.BooksModel;
using ShopThueBanSach.Server.Services.Interfaces;
using System.Security.Claims;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleBooksController : ControllerBase
    {
        private readonly ISaleBookService _service;
        private readonly IActivityNotificationService _notificationService;
        private readonly IStaffService _staffService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SaleBooksController(
            ISaleBookService service,
            IActivityNotificationService notificationService,
             IHttpContextAccessor httpContextAccessor,
            IStaffService staffService)
        {
            _service = service;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _staffService = staffService;
        }
        private int? GetCurrentStaffId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            return _staffService.GetStaffIdByEmail(claim); // bạn cần cài hàm này
        }

        private async Task CreateNotificationIfStaffExistsAsync(string description)
        {
            var staffId = GetCurrentStaffId();
            if (staffId.HasValue && await _staffService.ExistsAsync(staffId.Value))
            {
                await _notificationService.CreateNotificationAsync(staffId.Value, description);
            }
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

            var saleBook = await _service.GetByIdAsync(id);
            return saleBook == null ? NotFound() : Ok(saleBook);

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSaleBookDto dto)
        {
            var id = await _service.CreateAsync(dto);
            var result = await _service.GetByIdAsync(id); // ✅ Lấy lại dữ liệu đã tạo
            await CreateNotificationIfStaffExistsAsync($"Thêm sách bán: {dto.Title}");
            return CreatedAtAction(nameof(GetById), new { id }, result); // ✅ Trả về SaleBookDto

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SaleBookDto dto)
        {

            if (dto == null) return BadRequest();

            // Gán id từ route vào service, dto không chứa id
            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                await CreateNotificationIfStaffExistsAsync($"Cập nhật sách bán: {dto.Title}");


            return success ? NoContent() : NotFound();

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                await CreateNotificationIfStaffExistsAsync($"Xóa sách bán: {id}");

            return success ? NoContent() : NotFound(); 
  
        }

        [HttpPut("set-visibility/{id}/{isHidden}")]
        public async Task<IActionResult> SetVisibility(string id, int isHidden)
        {
            if (isHidden != 0 && isHidden != 1)
                return BadRequest("Giá trị isHidden chỉ được là 0 (hiện) hoặc 1 (ẩn)");

            var success = await _service.SetVisibilityAsync(id, isHidden == 1);
            if (success)
                await CreateNotificationIfStaffExistsAsync($"Cập nhật hiển thị sách bán: {id} -> {(isHidden == 1 ? "ẩn" : "hiện")}");

            return success ? NoContent() : NotFound();
        }
    }
}
