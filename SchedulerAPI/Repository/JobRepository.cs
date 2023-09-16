using Microsoft.EntityFrameworkCore;
using SchedulerAPI.Data;
using SchedulerAPI.Data.Models;

namespace SchedulerAPI.Repository
{
    public class JobRepository : GenericRepository<Job>
    {
        public JobRepository(ApplicationDbContext context) : base(context)
        {
        }

        async public Task<ICollection<Job>> GetJobsByUserIdAsync(string? name)
        {
            return await _context.Set<Job>().Where(p => p.UserId == name).ToListAsync();
        }

        async public Task<ICollection<Job>> GetUserJobsByStatusOrPriorityAsync(string? userId, JobStatus? status, Priority? priority)
        {
            if (status != null && priority != null)
            {
                return await _context.Set<Job>().Where(p => p.UserId == userId && p.Status == status && p.Priority == priority).ToListAsync();
            }
            if (status != null)
            {
                return await _context.Set<Job>().Where(p => p.UserId == userId && p.Status == status).ToListAsync();
            }
            return await _context.Set<Job>().Where(p => p.UserId == userId && p.Priority == priority).ToListAsync();
        }

        async public Task<ICollection<Job>> GetJobsByStatusOrPriorityAsync(JobStatus? status, Priority? priority)
        {
            if (status != null)
            {
                return await _context.Set<Job>().Where(p => p.Status == status).ToListAsync();
            }
            return await _context.Set<Job>().Where(p => p.Priority == priority).ToListAsync();
        }



        async public Task<ICollection<Job>> GetJobsDueThisWeekAsync()
        {
            return await _context.Set<Job>().Where(p => p.DueDate <= DateTime.Now.AddDays(7)).ToListAsync();
        }

        async public Task<ICollection<Job>> GetUserJobsDueThisWeekAsync(string userId)
        {
            return await _context.Set<Job>().Where(p => p.DueDate <= DateTime.Now.AddDays(7) && p.UserId == userId).ToListAsync();
        }

        async public Task<ICollection<Job>> GetJobsDueIn48HoursOrMarkedAsCompletedAsync()
        {
            var jobs = await _context.Set<Job>().Where(p => p.DueDate <= DateTime.Now.AddDays(2) || p.Status == JobStatus.Completed).ToListAsync();
            return jobs;
        }
    }
}
