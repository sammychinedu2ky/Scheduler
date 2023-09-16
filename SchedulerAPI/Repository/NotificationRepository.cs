using Microsoft.EntityFrameworkCore;
using SchedulerAPI.Data;
using SchedulerAPI.Data.Models;

namespace SchedulerAPI.Repository
{
    public class NotificationRepository : GenericRepository<Notification>
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        async public Task<ICollection<Notification>> GetNotificationsByUserIdAsync(string? name)
        {
            return await _context.Set<Notification>().Where(p => p.UserId == name).ToListAsync();
        }
    }

}
