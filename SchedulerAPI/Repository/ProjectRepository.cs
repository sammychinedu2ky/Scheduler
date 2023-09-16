using Microsoft.EntityFrameworkCore;
using SchedulerAPI.Data;
namespace SchedulerAPI.Repository
{
    public class ProjectRepository : GenericRepository<Data.Models.Project>
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        async public Task<ICollection<Data.Models.Project>> GetProjectsByUserIdAsync(string? name)
        {
            return await _context.Set<Data.Models.Project>().Where(p => p.UserId == name).ToListAsync();
        }
    }
}
