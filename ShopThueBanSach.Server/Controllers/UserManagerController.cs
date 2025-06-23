using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Models.AuthModel;
using ShopThueBanSach.Server.Models.UserModel;
using ShopThueBanSach.Server.Services.Interfaces;

namespace ShopThueBanSach.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserManagerService _userService;

        public UserManagerController(IUserManagerService userService)
        {
            _userService = userService;
        }

        // GET api/usermanager
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();   // đã lọc trong service
            return Ok(users);
        }

        // GET api/usermanager/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id); // service tự trả null nếu không đúng role
            return user == null ? NotFound() : Ok(user);
        }

        // PUT api/usermanager
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateUserDto dto)
        {
            var success = await _userService.UpdateAsync(dto);
            return success ? Ok() : NotFound();
        }
        // PUT api/usermanager/customer
        [HttpPut("customer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerDto dto)
        {
            var success = await _userService.UpdateCustomerAsync(dto);
            return success ? Ok() : NotFound();
        }


        // DELETE api/usermanager/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _userService.DeleteAsync(id);
            return success ? Ok() : NotFound();
        }
    }
}
