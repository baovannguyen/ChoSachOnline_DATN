using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Entities;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;

namespace ShopThueBanSach.Server.Area.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var staffs = await _staffService.GetAllAsync();
            return Ok(staffs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            return staff == null ? NotFound() : Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Staff staff)
        {
            var result = await _staffService.AddAsync(staff);
            return CreatedAtAction(nameof(Get), new { id = result.StaffId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Staff staff)
        {
            if (id != staff.StaffId) return BadRequest();
            var updated = await _staffService.UpdateAsync(staff);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _staffService.DeleteAsync(id);
            return result ? Ok() : NotFound();
        }
    }

}
