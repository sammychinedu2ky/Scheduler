using Microsoft.EntityFrameworkCore;
using SchedulerAPI.Data;

namespace SchedulerAPI.Repository
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        async public Task<T?> GetByIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        async public Task<ICollection<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }



        async public Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        async public Task UpdateAsync(T entity)
        {
            var item = await _context.Set<T>().FirstOrDefaultAsync();
            if (item is not null)
            {
                _context.Entry(item).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Item not found");
            }
        }

        async public Task DeleteAsync(string id)
        {

            var item = await _context.Set<T>().FindAsync(id);
            if (item is not null)
            {
                _context.Set<T>().Remove(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Item not found");
            }
        }


    }
}
