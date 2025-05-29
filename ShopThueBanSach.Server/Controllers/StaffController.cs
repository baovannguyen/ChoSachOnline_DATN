using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _service;

        public StaffController(IStaffService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var staff = await _service.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return Ok(staff);
        }

        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetByRole(string role)
        {
            var data = await _service.GetByRoleAsync(role);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Staff staff)
        {
            var created = await _service.CreateAsync(staff);
            return CreatedAtAction(nameof(GetById), new { id = created.StaffId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Staff staff)
        {
            if (id != staff.StaffId) return BadRequest();

            var result = await _service.UpdateAsync(staff);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }

}
