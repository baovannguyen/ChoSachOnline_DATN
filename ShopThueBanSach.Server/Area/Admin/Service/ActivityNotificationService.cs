using ShopThueBanSach.Server.Entities;
using ShopThueBanSach.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ShopThueBanSach.Server.Data;

namespace ShopThueBanSach.Server.Services
{
    public class ActivityNotificationService : IActivityNotificationService
    {
        private readonly AppDBContext _context;

        public ActivityNotificationService(AppDBContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(int staffId, string description)
        {
            var notification = new ActivityNotification
            {
                StaffId = staffId,
                Description = description,
                CreatedDate = DateTime.Now
                // KHÔNG gán NotificationId!
            };

            _context.ActivityNotifications.Add(notification);
            await _context.SaveChangesAsync();
        }


        public async Task<List<ActivityNotification>> GetAllNotificationsAsync()
        {
            return await _context.ActivityNotifications
                                 .Include(n => n.Staff) // Đổi từ .User
                                 .OrderByDescending(n => n.CreatedDate)
                                 .ToListAsync();
        }

    }
}
