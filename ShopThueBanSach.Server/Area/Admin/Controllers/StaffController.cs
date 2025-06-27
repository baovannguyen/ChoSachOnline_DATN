﻿// Controllers/StaffController.cs
using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Area.Admin.Entities;
using ShopThueBanSach.Server.Area.Admin.Service.Interface;

namespace ShopThueBanSach.Server.Area.Admin.Controllers
{
    // [Authorize(Roles = "Admin")] // ⬅️ Bật nếu chỉ Admin được quản lý Staff
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        /* ---------- READ ---------- */
        // GET: api/Staff
        // GET: api/Staff/users
        [HttpGet("users")]
        public async Task<IActionResult> GetStaffUsers()
        {
            var staffUsers = await _staffService.GetAllStaffUsersAsync(); // IEnumerable<UserDto>
            return Ok(staffUsers);
        }

      
        // GET: api/Staff/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var staff = await _staffService.GetByIdAsync(id);    // service sẽ trả về null nếu Role != "Staff"
            return staff == null ? NotFound() : Ok(staff);
        }

        /* ---------- CREATE ---------- */
        // POST: api/Staff
        /*  [HttpPost]
          public async Task<IActionResult> Create([FromBody] Staff staff)
          {
              // đảm bảo Role luôn là "Staff"
              staff.Role = "Staff";

              var created = await _staffService.AddAsync(staff);
              return CreatedAtAction(nameof(Get), new { id = created.StaffId }, created);
          }*/

        /* ---------- UPDATE ---------- */
        // PUT: api/Staff/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Staff staff)
        {
            if (id != staff.StaffId) return BadRequest("Id mismatch");

            // Service sẽ trả null nếu staff không tồn tại hoặc Role != "Staff"
            var updated = await _staffService.UpdateAsync(staff);
            return updated == null ? NotFound() : Ok(updated);
        }

        /* ---------- DELETE ---------- */
        // DELETE: api/Staff/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // Service sẽ trả false nếu không phải Staff hoặc không tồn tại
            var result = await _staffService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
