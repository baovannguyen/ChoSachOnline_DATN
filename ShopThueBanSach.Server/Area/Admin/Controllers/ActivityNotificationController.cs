using Microsoft.AspNetCore.Mvc;
using ShopThueBanSach.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ShopThueBanSach.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityNotificationController : ControllerBase
    {
        private readonly IActivityNotificationService _notificationService;

        public ActivityNotificationController(IActivityNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
     /*   [Authorize(Roles = "Admin")]*/
        public async Task<IActionResult> GetAll()
        {
            var result = await _notificationService.GetAllNotificationsAsync();
            return Ok(result);
        }

    }
}
